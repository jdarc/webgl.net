using System;
using WebGL;

namespace THREE
{
	public class Object3D : WebGL.JSObject
	{
		private static readonly Matrix4 __m1 = new Matrix4();
		private const string defaultEulerOrder = "XYZ";

		public int id;
		public string name;
		public JSObject properties;
		public Object3D parent;
		public JSArray children;
		public Vector3 up;
		public Vector3 position;
		public string eulerOrder;
		public Vector3 scale;
		public double renderDepth;
		public bool rotationAutoUpdate;
		public Matrix4 matrix;
		public Matrix4 matrixWorld;
		public Matrix4 matrixRotationWorld;
		public bool matrixAutoUpdate;
		public bool matrixWorldNeedsUpdate;
		public Quaternion quaternion;
		public bool useQuaternion;
		public bool visible;
		public bool castShadow;
		public bool receiveShadow;
		public bool frustumCulled;
		public dynamic material;
		public dynamic rotation;
		public dynamic geometry;

		public Vector3 _vector;
		public Matrix4 _modelViewMatrix;
		public Matrix3 _normalMatrix;

		public bool __webglInit;
		public bool __webglActive;
		public bool immediateRenderCallback;

		public Object3D()
		{
			id = THREE.Object3DIdCount++;

			name = "";
			properties = new JSObject();

			parent = null;
			children = new JSArray();

			up = new Vector3(0, 1, 0);

			position = new Vector3();
			rotation = new Vector3();
			eulerOrder = defaultEulerOrder;
			scale = new Vector3(1, 1, 1);

			renderDepth = double.NaN;

			rotationAutoUpdate = true;

			matrix = new Matrix4();
			matrixWorld = new Matrix4();
			matrixRotationWorld = new Matrix4();

			matrixAutoUpdate = true;
			matrixWorldNeedsUpdate = true;

			quaternion = new Quaternion();
			useQuaternion = false;

			visible = true;

			castShadow = false;
			receiveShadow = false;

			frustumCulled = true;

			_vector = new Vector3();
		}

		public virtual void applyMatrix(Matrix4 matrixArg)
		{
			matrix.multiplyMatrices(matrixArg, matrix);

			scale.getScaleFromMatrix(matrix);

			var mat = new Matrix4().extractRotation(matrix);
			rotation.setEulerFromRotationMatrix(mat, eulerOrder);

			position.getPositionFromMatrix(matrix);
		}

		public virtual void translate(double distance, Vector3 axis)
		{
			matrix.rotateAxis(axis);
			position.add(axis.multiplyScalar(distance));
		}

		public virtual void translateX(double distance)
		{
			translate(distance, _vector.set(1, 0, 0));
		}

		public virtual void translateY(double distance)
		{
			translate(distance, _vector.set(0, 1, 0));
		}

		public virtual void translateZ(double distance)
		{
			translate(distance, _vector.set(0, 0, 1));
		}

		public virtual Vector3 localToWorld(Vector3 vector)
		{
			return vector.applyMatrix4(matrixWorld);
		}

		public virtual Vector3 worldToLocal(Vector3 vector)
		{
			return vector.applyMatrix4(__m1.getInverse(matrixWorld));
		}

		public virtual void lookAt(Vector3 vector)
		{
			matrix.lookAt(vector, position, up);

			if (rotationAutoUpdate)
			{
				if (useQuaternion == false)
				{
					rotation.setEulerFromRotationMatrix(matrix, eulerOrder);
				}
				else
				{
					quaternion.copy(matrix.decompose()[1]);
				}
			}
		}

		public void add(Object3D obj)
		{
			if (obj == this)
			{
				JSConsole.warn("Object3D.add: An obj can\'t be added as a child of itself.");
				return;
			}
			
			if (obj.parent != null)
			{
				obj.parent.remove(obj);
			}

			obj.parent = this;
			children.push(obj);

			var scene = this as dynamic;

			while (scene.parent != null)
			{
				scene = scene.parent;
			}

			if (scene != null && scene is Scene)
			{
				scene.__addObject(obj);
			}
		}

		public virtual void remove(Object3D @object)
		{
			var index = children.indexOf(@object);

			if (index != -1)
			{
				@object.parent = null;
				children.splice(index, 1);

				// remove from scene

				var sceneObj = this;

				while (sceneObj.parent != null)
				{
					sceneObj = sceneObj.parent;
				}

				var scene = sceneObj as Scene;
				if (scene != null)
				{
					scene.__removeObject(@object);
				}
			}
		}

		public virtual void traverse(Action<Object3D> callback)
		{
			callback(this);

			var length = children.length;
			for (var i = 0; i < length; i++)
			{
				children[i].traverse(callback);
			}
		}

		public virtual dynamic getChildByName(string searchName, bool recursive = false)
		{
			for (int i = 0, l = children.length; i < l; i++)
			{
				var child = children[i];

				if (child.name == searchName)
				{
					return child;
				}

				if (recursive)
				{
					child = child.getChildByName(searchName, recursive);

					if (child != null)
					{
						return child;
					}
				}
			}

			return null;
		}

		public virtual dynamic getDescendants(JSArray array = null)
		{
			if (array == null)
			{
				array = new JSArray();
			}

			JSArray.prototype.push.apply(array, children);

			for (int i = 0, l = children.length; i < l; i++)
			{
				children[i].getDescendants(array);
			}

			return array;
		}

		public virtual void updateMatrix()
		{
			matrix.setPosition(position);

			if (useQuaternion == false)
			{
				matrix.setRotationFromEuler(rotation, eulerOrder);
			}
			else
			{
				matrix.setRotationFromQuaternion(quaternion);
			}

			if (scale.x != 1.0 || scale.y != 1.0 || scale.z != 1.0)
			{
				matrix.scale(scale);
			}

			matrixWorldNeedsUpdate = true;
		}

		public virtual void updateMatrixWorld(bool force = false)
		{
			if (matrixAutoUpdate)
			{
				updateMatrix();
			}

			if (matrixWorldNeedsUpdate || force)
			{
				if (parent == null)
				{
					matrixWorld.copy(matrix);
				}
				else
				{
					matrixWorld.multiplyMatrices(parent.matrixWorld, matrix);
				}

				matrixWorldNeedsUpdate = false;

				force = true;
			}

			for (var i = 0; i < children.length; i++)
			{
				children[i].updateMatrixWorld(force);
			}
		}

		public virtual Object3D clone(Object3D obj = null)
		{
			if (obj == null)
			{
				obj = new Object3D();
			}

			obj.name = name;

			obj.up.copy(up);

			obj.position.copy(position);
			if (obj.rotation is Vector3)
			{
				obj.rotation.copy(rotation); // because of Sprite madness
			}
			obj.eulerOrder = eulerOrder;
			obj.scale.copy(scale);

			obj.renderDepth = renderDepth;

			obj.rotationAutoUpdate = rotationAutoUpdate;

			obj.matrix.copy(matrix);
			obj.matrixWorld.copy(matrixWorld);
			obj.matrixRotationWorld.copy(matrixRotationWorld);

			obj.matrixAutoUpdate = matrixAutoUpdate;
			obj.matrixWorldNeedsUpdate = matrixWorldNeedsUpdate;

			obj.quaternion.copy(quaternion);
			obj.useQuaternion = useQuaternion;

			obj.visible = visible;

			obj.castShadow = castShadow;
			obj.receiveShadow = receiveShadow;

			obj.frustumCulled = frustumCulled;

			for (var i = 0; i < children.length; i++)
			{
				var child = children[i];
				obj.add(child.clone());
			}

			return obj;
		}
	}
}
