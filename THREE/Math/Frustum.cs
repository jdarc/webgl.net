namespace THREE
{
	public class Frustum
	{
		public readonly Plane[] planes;

		public Frustum(Plane p0 = null, Plane p1 = null, Plane p2 = null,
		               Plane p3 = null, Plane p4 = null, Plane p5 = null)
		{
			planes = new[]
			{
				p0 ?? new Plane(),
				p1 ?? new Plane(),
				p2 ?? new Plane(),
				p3 ?? new Plane(),
				p4 ?? new Plane(),
				p5 ?? new Plane()
			};
		}

		public Frustum set(Plane p0, Plane p1, Plane p2, Plane p3, Plane p4, Plane p5)
		{
			planes[0].copy(p0);
			planes[1].copy(p1);
			planes[2].copy(p2);
			planes[3].copy(p3);
			planes[4].copy(p4);
			planes[5].copy(p5);

			return this;
		}

		public Frustum copy(Frustum frustum)
		{
			var planes = this.planes;

			for (var i = 0; i < 6; i++)
			{
				planes[i].copy(frustum.planes[i]);
			}

			return this;
		}

		public Frustum setFromMatrix(Matrix4 m)
		{
			var me = m.elements;
			double me0 = me[0], me1 = me[1], me2 = me[2], me3 = me[3];
			double me4 = me[4], me5 = me[5], me6 = me[6], me7 = me[7];
			double me8 = me[8], me9 = me[9], me10 = me[10], me11 = me[11];
			double me12 = me[12], me13 = me[13], me14 = me[14], me15 = me[15];

			planes[0].setComponents(me3 - me0, me7 - me4, me11 - me8, me15 - me12).normalize();
			planes[1].setComponents(me3 + me0, me7 + me4, me11 + me8, me15 + me12).normalize();
			planes[2].setComponents(me3 + me1, me7 + me5, me11 + me9, me15 + me13).normalize();
			planes[3].setComponents(me3 - me1, me7 - me5, me11 - me9, me15 - me13).normalize();
			planes[4].setComponents(me3 - me2, me7 - me6, me11 - me10, me15 - me14).normalize();
			planes[5].setComponents(me3 + me2, me7 + me6, me11 + me10, me15 + me14).normalize();

			return this;
		}

		public bool intersectsObject(Object3D obj)
		{
			var matrix = obj.matrixWorld;
			var center = matrix.getPosition();
			var negRadius = - obj.geometry.boundingSphere.radius * matrix.getMaxScaleOnAxis();

			for (var i = 0; i < 6; i++)
			{
				var distance = planes[i].distanceToPoint(center);

				if (distance < negRadius)
				{
					return false;
				}
			}

			return true;
		}

		public bool intersectsSphere(Sphere sphere)
		{
			var center = sphere.center;
			var negRadius = -sphere.radius;

			for (var i = 0; i < 6; i++)
			{
				var distance = planes[i].distanceToPoint(center);

				if (distance < negRadius)
				{
					return false;
				}
			}

			return true;
		}

		public bool containsPoint(Vector3 point)
		{
			for (var i = 0; i < 6; i++)
			{
				if (planes[i].distanceToPoint(point) < 0)
				{
					return false;
				}
			}

			return true;
		}

		public Frustum clone()
		{
			return new Frustum().copy(this);
		}
	}
}
