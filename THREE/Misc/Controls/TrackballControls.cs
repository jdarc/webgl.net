using System.Windows.Forms;
using WebGL;

namespace THREE
{
	public class TrackballControls : JSEventDispatcher
	{
		public Object3D obj;
		public Control control;
		public bool enabled;
		public Screen screen;
		public double radius;
		public double rotateSpeed;
		public double zoomSpeed;
		public double panSpeed;
		public bool noRotate;
		public bool noZoom;
		public bool noPan;
		public bool staticMoving;
		public double dynamicDampingFactor;
		public double minDistance;
		public double maxDistance;
		public int[] typeKeys;
		public Vector3 target;
		public Vector3 lastPosition;

		private STATE _state;
		private STATE _prevState;
		private readonly Vector3 _eye;
		private Vector3 _rotateStart;
		private Vector3 _rotateEnd;
		private Vector2 _zoomStart;
		private Vector2 _zoomEnd;
		private int _touchZoomDistanceStart;
		private int _touchZoomDistanceEnd;
		private Vector2 _panStart;
		private Vector2 _panEnd;
		private readonly string changeEvent;

		public struct Screen
		{
			public int width;
			public int height;
			public int offsetLeft;
			public int offsetTop;
		}

		private enum STATE
		{
			NONE = -1,
			ROTATE = MouseButtons.Left,
			ZOOM = MouseButtons.Middle,
			PAN = MouseButtons.Right,
			TOUCH_ROTATE = 3,
			TOUCH_ZOOM = 4,
			TOUCH_PAN = 5
		}

		public TrackballControls(Object3D obj, Control control)
		{
			this.obj = obj;
			this.control = control;

			enabled = true;

			screen = new Screen {width = 0, height = 0, offsetLeft = 0, offsetTop = 0};
			radius = (screen.width + screen.height) / 4.0;

			rotateSpeed = 1.0;
			zoomSpeed = 1.2;
			panSpeed = 0.3;

			noRotate = false;
			noZoom = false;
			noPan = false;

			staticMoving = false;
			dynamicDampingFactor = 0.2;

			minDistance = 0;
			maxDistance = double.PositiveInfinity;

			typeKeys = new[] {65 /*A*/, 83 /*S*/, 68 /*D*/};

			target = new Vector3();

			lastPosition = new Vector3();

			_state = STATE.NONE;
			_prevState = STATE.NONE;
			_eye = new Vector3();
			_rotateStart = new Vector3();
			_rotateEnd = new Vector3();
			_zoomStart = new Vector2();
			_zoomEnd = new Vector2();
			_touchZoomDistanceStart = 0;
			_touchZoomDistanceEnd = 0;
			_panStart = new Vector2();
			_panEnd = new Vector2();

			changeEvent = "change";

			control.MouseDown += mousedown;
			control.MouseMove += mousemove;
			control.MouseUp += mouseup;

			control.MouseWheel += mousewheel;

			handleResize();
		}

		public void handleResize()
		{
			screen.width = control.ClientSize.Width;
			screen.height = control.ClientSize.Height;

			screen.offsetLeft = 0;
			screen.offsetTop = 0;

			radius = (screen.width + screen.height) / 4.0;
		}

		public void handleEvent(JSEvent @event)
		{
			if (this[@event.type] != null)
			{
				this[@event.type]();
			}
		}

		public Vector2 getMouseOnScreen(int clientX, int clientY)
		{
			return new Vector2((clientX - screen.offsetLeft) / radius * 0.5,
			                   (clientY - screen.offsetTop) / radius * 0.5);
		}

		public Vector3 getMouseProjectionOnBall(int clientX, int clientY)
		{
			var mouseOnBall = new Vector3(
				(clientX - screen.width * 0.5 - screen.offsetLeft) / radius,
				(screen.height * 0.5 + screen.offsetTop - clientY) / radius, 0);

			var length = mouseOnBall.length();

			if (length > 1.0)
			{
				mouseOnBall.normalize();
			}
			else
			{
				mouseOnBall.z = System.Math.Sqrt(1.0 - length * length);
			}

			_eye.copy(obj.position).sub(target);

			var projection = obj.up.clone().setLength(mouseOnBall.y);
			projection.add(obj.up.clone().cross(_eye).setLength(mouseOnBall.x));
			projection.add(_eye.setLength(mouseOnBall.z));

			return projection;
		}

		public void rotateCamera()
		{
			var angle = System.Math.Acos(_rotateStart.dot(_rotateEnd) / _rotateStart.length() / _rotateEnd.length());

			if (!double.IsNaN(angle))
			{
				var axis = (new Vector3()).crossVectors(_rotateStart, _rotateEnd).normalize();
				var quaternion = new Quaternion();

				angle *= rotateSpeed;

				quaternion.setFromAxisAngle(axis, -angle);

				_eye.applyQuaternion(quaternion);
				obj.up.applyQuaternion(quaternion);

				_rotateEnd.applyQuaternion(quaternion);

				if (staticMoving)
				{
					_rotateStart.copy(_rotateEnd);
				}
				else
				{
					quaternion.setFromAxisAngle(axis, angle * (dynamicDampingFactor - 1.0));
					_rotateStart.applyQuaternion(quaternion);
				}
			}
		}

		public void zoomCamera()
		{
			if (_state == STATE.TOUCH_ZOOM)
			{
				var factor = _touchZoomDistanceStart / _touchZoomDistanceEnd;
				_touchZoomDistanceStart = _touchZoomDistanceEnd;
				_eye.multiplyScalar(factor);
			}
			else
			{
				var factor = 1.0 + (_zoomEnd.y - _zoomStart.y) * zoomSpeed;

				if (factor != 1.0 && factor > 0.0)
				{
					_eye.multiplyScalar(factor);

					if (staticMoving)
					{
						_zoomStart.copy(_zoomEnd);
					}
					else
					{
						_zoomStart.y += (_zoomEnd.y - _zoomStart.y) * dynamicDampingFactor;
					}
				}
			}
		}

		public void panCamera()
		{
			var mouseChange = _panEnd.clone().sub(_panStart);

			if (mouseChange.lengthSq() != 0)
			{
				mouseChange.multiplyScalar(_eye.length() * panSpeed);

				var pan = _eye.clone().cross(obj.up).setLength(mouseChange.x);
				pan.add(obj.up.clone().setLength(mouseChange.y));

				obj.position.add(pan);
				target.add(pan);

				if (staticMoving)
				{
					_panStart = _panEnd;
				}
				else
				{
					_panStart.add(mouseChange.subVectors(_panEnd, _panStart).multiplyScalar(dynamicDampingFactor));
				}
			}
		}

		public void checkDistances()
		{
			if (!noZoom || !noPan)
			{
				if (obj.position.lengthSq() > maxDistance * maxDistance)
				{
					obj.position.setLength(maxDistance);
				}

				if (_eye.lengthSq() < minDistance * minDistance)
				{
					obj.position.addVectors(target, _eye.setLength(minDistance));
				}
			}
		}

		public void update(double delta = 0.0)
		{
			_eye.subVectors(obj.position, target);

			if (!noRotate)
			{
				rotateCamera();
			}

			if (!noZoom)
			{
				zoomCamera();
			}

			if (!noPan)
			{
				panCamera();
			}

			obj.position.addVectors(target, _eye);

			checkDistances();

			obj.lookAt(target);

			if (lastPosition.distanceToSquared(obj.position) > 0)
			{
				dispatchEvent(new JSEvent(this, "changeEvent"));
			}

			lastPosition.copy(obj.position);
		}

		private void keydown(dynamic @event)
		{
			if (enabled == false)
			{
				return;
			}

			_prevState = _state;

			if (_state != STATE.NONE)
			{
				return;
			}
			else if (@event.keyCode == typeKeys[(int)STATE.ROTATE] && !noRotate)
			{
				_state = STATE.ROTATE;
			}
			else if (@event.keyCode == typeKeys[(int)STATE.ZOOM] && !noZoom)
			{
				_state = STATE.ZOOM;
			}
			else if (@event.keyCode == typeKeys[(int)STATE.PAN] && !noPan)
			{
				_state = STATE.PAN;
			}
		}

		private void keyup(dynamic @event)
		{
			if (enabled == false)
			{
				return;
			}

			_state = _prevState;
		}

		private void mousedown(object sender, MouseEventArgs mouseEventArgs)
		{
			if (enabled == false)
			{
				return;
			}

			if (_state == STATE.NONE)
			{
				_state = (STATE)mouseEventArgs.Button;
			}

			if (_state == STATE.ROTATE && !noRotate)
			{
				_rotateStart = _rotateEnd = getMouseProjectionOnBall(mouseEventArgs.X, mouseEventArgs.Y);
			}
			else if (_state == STATE.ZOOM && !noZoom)
			{
				_zoomStart = _zoomEnd = getMouseOnScreen(mouseEventArgs.X, mouseEventArgs.Y);
			}
			else if (_state == STATE.PAN && !noPan)
			{
				_panStart = _panEnd = getMouseOnScreen(mouseEventArgs.X, mouseEventArgs.Y);
			}
		}

		private void mousemove(object sender, MouseEventArgs mouseEventArgs)
		{
			if (enabled == false)
			{
				return;
			}

			if (_state == STATE.ROTATE && !noRotate)
			{
				_rotateEnd = getMouseProjectionOnBall(mouseEventArgs.X, mouseEventArgs.Y);
			}
			else if (_state == STATE.ZOOM && !noZoom)
			{
				_zoomEnd = getMouseOnScreen(mouseEventArgs.X, mouseEventArgs.Y);
			}
			else if (_state == STATE.PAN && !noPan)
			{
				_panEnd = getMouseOnScreen(mouseEventArgs.X, mouseEventArgs.Y);
			}
		}

		private void mouseup(object sender, MouseEventArgs mouseEventArgs)
		{
			if (enabled == false)
			{
				return;
			}

			_state = STATE.NONE;
		}

		private void mousewheel(object sender, MouseEventArgs mouseEventArgs)
		{
			if (enabled == false)
			{
				return;
			}

			var delta = 0.0;

			if (mouseEventArgs.Delta != 0)
			{
				delta = mouseEventArgs.Delta / 40.0;
			}

			_zoomStart.y += (1.0 / delta) * 0.05;
		}

		private void touchstart(dynamic @event)
		{
			if (enabled == false)
			{
				return;
			}

			switch ((int)@event.touches.length)
			{
				case 1:
					_state = STATE.TOUCH_ROTATE;
					_rotateStart = _rotateEnd = this.getMouseProjectionOnBall(@event.touches[0].pageX, @event.touches[0].pageY);
					break;

				case 2:
					_state = STATE.TOUCH_ZOOM;
					var dx = @event.touches[0].pageX - @event.touches[1].pageX;
					var dy = @event.touches[0].pageY - @event.touches[1].pageY;
					_touchZoomDistanceEnd = _touchZoomDistanceStart = System.Math.Sqrt(dx * dx + dy * dy);
					break;

				case 3:
					_state = STATE.TOUCH_PAN;
					_panStart = _panEnd = this.getMouseOnScreen(@event.touches[0].pageX, @event.touches[0].pageY);
					break;

				default:
					_state = STATE.NONE;
					break;
			}
		}

		private void touchmove(dynamic @event)
		{
			if (enabled == false)
			{
				return;
			}

			@event.preventDefault();
			@event.stopPropagation();

			switch ((int)@event.touches.length)
			{
				case 1:
					_rotateEnd = this.getMouseProjectionOnBall(@event.touches[0].pageX, @event.touches[0].pageY);
					break;

				case 2:
					var dx = @event.touches[0].pageX - @event.touches[1].pageX;
					var dy = @event.touches[0].pageY - @event.touches[1].pageY;
					_touchZoomDistanceEnd = System.Math.Sqrt(dx * dx + dy * dy);
					break;

				case 3:
					_panEnd = this.getMouseOnScreen(@event.touches[0].pageX, @event.touches[0].pageY);
					break;

				default:
					_state = STATE.NONE;
					break;
			}
		}

		private void touchend(dynamic @event)
		{
			if (enabled == false)
			{
				return;
			}

			switch ((int)@event.touches.length)
			{
				case 1:
					_rotateStart = _rotateEnd = this.getMouseProjectionOnBall(@event.touches[0].pageX, @event.touches[0].pageY);
					break;

				case 2:
					_touchZoomDistanceStart = _touchZoomDistanceEnd = 0;
					break;

				case 3:
					_panStart = _panEnd = this.getMouseOnScreen(@event.touches[0].pageX, @event.touches[0].pageY);
					break;
			}

			_state = STATE.NONE;
		}
	}
}
