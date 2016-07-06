namespace THREE
{
	public class Triangle
	{
		private static readonly Vector3 __v0 = new Vector3();
		private static readonly Vector3 __v1 = new Vector3();
		private static readonly Vector3 __v2 = new Vector3();
		private static readonly Vector3 __v3 = new Vector3();

		public static Vector3 normal(Vector3 a, Vector3 b, Vector3 c, Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();

			result.subVectors(c, b);
			__v0.subVectors(a, b);
			result.cross(__v0);

			var resultLengthSq = result.lengthSq();
			if (resultLengthSq > 0)
			{
				return result.multiplyScalar(1 / System.Math.Sqrt(resultLengthSq));
			}

			return result.set(0, 0, 0);
		}

		public static Vector3 barycoordFromPoint(Vector3 point, Vector3 a, Vector3 b, Vector3 c, Vector3 optionalTarget = null)
		{
			__v0.subVectors(c, a);
			__v1.subVectors(b, a);
			__v2.subVectors(point, a);

			var dot00 = __v0.dot(__v0);
			var dot01 = __v0.dot(__v1);
			var dot02 = __v0.dot(__v2);
			var dot11 = __v1.dot(__v1);
			var dot12 = __v1.dot(__v2);

			var denom = (dot00 * dot11 - dot01 * dot01);

			var result = optionalTarget ?? new Vector3();

			if (denom == 0)
			{
				return result.set(-2, -1, -1);
			}

			var invDenom = 1 / denom;
			var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

			return result.set(1 - u - v, v, u);
		}

		public static bool containsPoint(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
		{
			var result = barycoordFromPoint(point, a, b, c, __v3);

			return (result.x >= 0) && (result.y >= 0) && ((result.x + result.y) <= 1);
		}

		public Vector3 a;
		public Vector3 b;
		public Vector3 c;

		public Triangle(Vector3 a = null, Vector3 b = null, Vector3 c = null)
		{
			this.a = a ?? new Vector3();
			this.b = b ?? new Vector3();
			this.c = c ?? new Vector3();
		}

		public Triangle set(Vector3 a, Vector3 b, Vector3 c)
		{
			this.a.copy(a);
			this.b.copy(b);
			this.c.copy(c);

			return this;
		}

		public Triangle setFromPointsAndIndices(Vector3[] points, int i0, int i1, int i2)
		{
			a.copy(points[i0]);
			b.copy(points[i1]);
			c.copy(points[i2]);

			return this;
		}

		public Triangle copy(Triangle triangle)
		{
			a.copy(triangle.a);
			b.copy(triangle.b);
			c.copy(triangle.c);

			return this;
		}

		public double area()
		{
			__v0.subVectors(c, b);
			__v1.subVectors(a, b);

			return __v0.cross(__v1).length() * 0.5;
		}

		public Vector3 midpoint(Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();
			return result.addVectors(a, b).add(c).multiplyScalar(1 / 3);
		}

		public Vector3 normal(Vector3 optionalTarget = null)
		{
			return normal(a, b, c, optionalTarget);
		}

		public Plane plane(Plane optionalTarget = null)
		{
			var result = optionalTarget ?? new Plane();

			return result.setFromCoplanarPoints(a, b, c);
		}

		public Vector3 barycoordFromPoint(Vector3 point, Vector3 optionalTarget = null)
		{
			return barycoordFromPoint(point, a, b, c, optionalTarget);
		}

		public bool containsPoint(Vector3 point)
		{
			return containsPoint(point, a, b, c);
		}

		public bool equals(Triangle triangle)
		{
			return triangle.a.equals(a) && triangle.b.equals(b) && triangle.c.equals(c);
		}

		public Triangle clone()
		{
			return new Triangle().copy(this);
		}
	}
}
