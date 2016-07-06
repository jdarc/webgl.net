using System;
using WebGL;

namespace THREE
{
	public class Path : CurvePath
	{
		public static class PathActions
		{
			public const string MOVE_TO = "moveTo";
			public const string LINE_TO = "lineTo";
			public const string QUADRATIC_CURVE_TO = "quadraticCurveTo";
			public const string BEZIER_CURVE_TO = "bezierCurveTo";
			public const string CSPLINE_THRU = "splineThru";
			public const string ARC = "arc";
			public const string ELLIPSE = "ellipse";
		}

		public JSArray actions;
		public bool useSpacedPoints;

		public Path(JSArray points = null)
		{
			actions = new JSArray();

			if (points != null)
			{
				fromPoints(points);
			}
		}

		public void fromPoints(JSArray vectors)
		{
			this.moveTo(vectors[0].x, vectors[0].y);
			for (dynamic v = 1, vlen = vectors.length; v < vlen; v++)
			{
				this.lineTo(vectors[v].x, vectors[v].y);
			}
		}

		public void moveTo(double x, double y)
		{
			actions.push(JSObject.create(new {action = PathActions.MOVE_TO, args = new JSArray(x, y)}));
		}

		public void lineTo(double x, double y)
		{
			var args = new JSArray(x, y);

			var lastargs = actions[actions.length - 1].args;
			double x0 = lastargs[lastargs.length - 2];
			double y0 = lastargs[lastargs.length - 1];
			curves.push(new LineCurve(new Vector2(x0, y0), new Vector2(x, y)));

			actions.push(JSObject.create(new {action = PathActions.LINE_TO, args}));
		}

		public void quadraticCurveTo(double aCPx, double aCPy, double aX, double aY)
		{
			var args = new JSArray(aCPx, aCPy, aX, aY);

			var lastargs = actions[actions.length - 1].args;
			double x0 = lastargs[lastargs.length - 2];
			double y0 = lastargs[lastargs.length - 1];
			curves.push(new QuadraticBezierCurve(new Vector2(x0, y0), new Vector2(aCPx, aCPy), new Vector2(aX, aY)));

			actions.push(JSObject.create(new {action = PathActions.QUADRATIC_CURVE_TO, args}));
		}

		public void bezierCurveTo(double aCP1x, double aCP1y, double aCP2x, double aCP2y, double aX, double aY)
		{
			var args = new JSArray(aCP1x, aCP1y, aCP2x, aCP2y, aX, aY);

			var lastargs = actions[actions.length - 1].args;
			double x0 = lastargs[lastargs.length - 2];
			double y0 = lastargs[lastargs.length - 1];
			curves.push(new CubicBezierCurve(new Vector2(x0, y0), new Vector2(aCP1x, aCP1y), new Vector2(aCP2x, aCP2y), new Vector2(aX, aY)));

			actions.push(JSObject.create(new {action = PathActions.BEZIER_CURVE_TO, args}));
		}

		public void splineThru(JSArray pts)
		{
			var args = new JSArray(pts);

			var lastargs = actions[actions.length - 1].args;

			var x0 = lastargs[lastargs.length - 2];
			var y0 = lastargs[lastargs.length - 1];

			var npts = new JSArray(new Vector2(x0, y0));
			JSArray.prototype.push.apply(npts, pts);

			curves.push(new SplineCurve(npts));

			actions.push(JSObject.create(new {action = PathActions.CSPLINE_THRU, args}));
		}

		public void arc(double aX, double aY, double aRadius, double aStartAngle, double aEndAngle, bool aClockwise)
		{
			var lastargs = actions[actions.length - 1].args;
			double x0 = lastargs[lastargs.length - 2];
			double y0 = lastargs[lastargs.length - 1];

			absarc(aX + x0, aY + y0, aRadius, aStartAngle, aEndAngle, aClockwise);
		}

		public void absarc(double aX, double aY, double aRadius, double aStartAngle, double aEndAngle, bool aClockwise)
		{
			absellipse(aX, aY, aRadius, aRadius, aStartAngle, aEndAngle, aClockwise);
		}

		public void ellipse(double aX, double aY, double xRadius, double yRadius, double aStartAngle, double aEndAngle, bool aClockwise)
		{
			var lastargs = actions[actions.length - 1].args;
			var x0 = lastargs[lastargs.length - 2];
			var y0 = lastargs[lastargs.length - 1];

			this.absellipse(aX + x0, aY + y0, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise);
		}

		public void absellipse(double aX, double aY, double xRadius, double yRadius, double aStartAngle, double aEndAngle, bool aClockwise)
		{
			var args = new JSArray(aX, aY, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise);

			var curve = new EllipseCurve(aX, aY, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise);
			curves.push(curve);

			var lastPoint = curve.getPoint(aClockwise ? 1 : 0);
			args.push(lastPoint.x);
			args.push(lastPoint.y);

			actions.push(JSObject.create(new {action = PathActions.ELLIPSE, args}));
		}

		public override JSArray getSpacedPoints(double divisions = double.NaN, bool closedPath = false)
		{
			if (double.IsNaN(divisions))
			{
				divisions = 40.0;
			}

			var points = new JSArray();

			for (double i = 0; i < divisions; i++)
			{
				points.push(getPoint(i / divisions));
			}

			return points;
		}

		public override JSArray getPoints(double divisions = double.NaN, bool closedPath = false)
		{
			if (useSpacedPoints)
			{
				JSConsole.log("tata");
				return getSpacedPoints(divisions, closedPath);
			}

			if (double.IsNaN(divisions))
			{
				divisions = 12.0;
			}

			var points = new JSArray();

			for (var i = 0; i < actions.length; i++)
			{
				var item = actions[i];
				var action = item.action;
				var args = item.args;

				if (action == PathActions.MOVE_TO)
				{
					points.push(new Vector2((double)args[0], (double)args[1]));
				}
				else if (action == PathActions.LINE_TO)
				{
					points.push(new Vector2((double)args[0], (double)args[1]));
				}
				else
				{
					double cpx1;
					double cpy;
					double cpy1;
					double cpx;
					double cpx0;
					double cpy0;
					double t;
					double tx;
					double ty;
					if (action == PathActions.QUADRATIC_CURVE_TO)
					{
						cpx = args[2];
						cpy = args[3];

						cpx1 = args[0];
						cpy1 = args[1];

						if (points.length > 0)
						{
							var laste = points[points.length - 1];

							cpx0 = laste.x;
							cpy0 = laste.y;
						}
						else
						{
							var laste = actions[i - 1].args;

							cpx0 = laste[laste.length - 2];
							cpy0 = laste[laste.length - 1];
						}

						for (var j = 1; j <= divisions; j++)
						{
							t = j / divisions;

							tx = Shape.Utils.b2(t, cpx0, cpx1, cpx);
							ty = Shape.Utils.b2(t, cpy0, cpy1, cpy);

							points.push(new Vector2(tx, ty));
						}
					}
					else if (action == PathActions.BEZIER_CURVE_TO)
					{
						cpx = args[4];
						cpy = args[5];

						cpx1 = args[0];
						cpy1 = args[1];

						double cpx2 = args[2];
						double cpy2 = args[3];

						if (points.length > 0)
						{
							var laste = points[points.length - 1];

							cpx0 = laste.x;
							cpy0 = laste.y;
						}
						else
						{
							var laste = actions[i - 1].args;

							cpx0 = laste[laste.length - 2];
							cpy0 = laste[laste.length - 1];
						}

						for (var j = 1; j <= divisions; j++)
						{
							t = j / divisions;

							tx = Shape.Utils.b3(t, cpx0, cpx1, cpx2, cpx);
							ty = Shape.Utils.b3(t, cpy0, cpy1, cpy2, cpy);

							points.push(new Vector2(tx, ty));
						}
					}
					else if (action == PathActions.CSPLINE_THRU)
					{
						var laste = actions[i - 1].args;

						var last = new Vector2(laste[laste.length - 2], laste[laste.length - 1]);
						var spts = new JSArray(last);

						var n = divisions * args[0].length;

						spts = spts.concat(args[0]);

						var spline = new SplineCurve(spts);

						for (var j = 1; j <= n; j++)
						{
							points.push(spline.getPointAt(j / (double)n));
						}
					}
					else if (action == PathActions.ARC)
					{
						throw new NotImplementedException();
						dynamic aX = args[0], aY = args[1],
						        aRadius = args[2],
						        aStartAngle = args[3], aEndAngle = args[4],
						        aClockwise = !!args[5];

						var deltaAngle = aEndAngle - aStartAngle;
						dynamic angle;
						var tdivisions = divisions * 2;

						for (var j = 1; j <= tdivisions; j++)
						{
							t = j / tdivisions;

							if (! aClockwise)
							{
								t = 1 - t;
							}

							angle = aStartAngle + t * deltaAngle;

							tx = aX + aRadius * System.Math.Cos((double)angle);
							ty = aY + aRadius * System.Math.Sin((double)angle);

							points.push(new Vector2(tx, ty));
						}
					}
					else if (action == PathActions.ELLIPSE)
					{
						double aX = args[0];
						double aY = args[1];
						double xRadius = args[2];
						double yRadius = args[3];
						double aStartAngle = args[4];
						double aEndAngle = args[5];
						bool aClockwise = args[6];

						var deltaAngle = aEndAngle - aStartAngle;
						var tdivisions = divisions * 2;

						for (var j = 1; j <= tdivisions; j++)
						{
							t = j / tdivisions;

							if (!aClockwise)
							{
								t = 1.0 - t;
							}

							var angle = aStartAngle + t * deltaAngle;

							tx = aX + xRadius * System.Math.Cos(angle);
							ty = aY + yRadius * System.Math.Sin(angle);

							points.push(new Vector2(tx, ty));
						}
					}
				}
			}

			var lastPoint = points[points.length - 1];
			var EPSILON = 0.0000000001;
			if (System.Math.Abs(lastPoint.x - points[0].x) < EPSILON && System.Math.Abs(lastPoint.y - points[0].y) < EPSILON)
			{
				points.splice(points.length - 1, 1);
			}

			if (closedPath)
			{
				points.push(points[0]);
			}

			return points;
		}

		public JSArray toShapes()
		{
			throw new NotImplementedException();
			dynamic i, il, item, action, args;

			dynamic subPaths = new JSArray(), lastPath = new Path();

			for (i = 0, il = actions.length; i < il; i++)
			{
				item = actions[i];

				args = item.args;
				action = item.action;

				if (action == PathActions.MOVE_TO)
				{
					if (lastPath.actions.length != 0)
					{
						subPaths.push(lastPath);
						lastPath = new Path();
					}
				}

				lastPath[action].apply(lastPath, args);
			}

			if (lastPath.actions.length != 0)
			{
				subPaths.push(lastPath);
			}

			if (subPaths.length == 0)
			{
				return new JSArray();
			}

			dynamic tmpPath, tmpShape = null, shapes = new JSArray();

			var holesFirst = !Shape.Utils.isClockWise(subPaths[0].getPoints());

			if (subPaths.length == 1)
			{
				tmpPath = subPaths[0];
				tmpShape = new Shape();
				tmpShape.actions = tmpPath.actions;
				tmpShape.curves = tmpPath.curves;
				shapes.push(tmpShape);
				return shapes;
			}

			if (holesFirst)
			{
				tmpShape = new Shape();

				for (i = 0, il = subPaths.length; i < il; i++)
				{
					tmpPath = subPaths[i];

					if (Shape.Utils.isClockWise(tmpPath.getPoints()))
					{
						tmpShape.actions = tmpPath.actions;
						tmpShape.curves = tmpPath.curves;

						shapes.push(tmpShape);
						tmpShape = new Shape();
					}
					else
					{
						tmpShape.holes.push(tmpPath);
					}
				}
			}
			else
			{
				for (i = 0, il = subPaths.length; i < il; i++)
				{
					tmpPath = subPaths[i];

					if (Shape.Utils.isClockWise(tmpPath.getPoints()))
					{
						if (tmpShape != null)
						{
							shapes.push(tmpShape);
						}

						tmpShape = new Shape();
						tmpShape.actions = tmpPath.actions;
						tmpShape.curves = tmpPath.curves;
					}
					else
					{
						tmpShape.holes.push(tmpPath);
					}
				}

				shapes.push(tmpShape);
			}

			return shapes;
		}
	}
}
