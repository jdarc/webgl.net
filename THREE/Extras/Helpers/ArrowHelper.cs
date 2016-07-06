namespace THREE
{
	public class ArrowHelper : Object3D
	{
		private static readonly Vector3 __v1 = new Vector3();
		private static readonly Vector3 __v2 = new Vector3();
		private static readonly Quaternion __q1 = new Quaternion();

		public Line line;
		public Mesh cone;

		public ArrowHelper(Vector3 dir, Vector3 origin, double length = 20, int hex = 0xffff00)
		{
			var lineGeometry = new Geometry();
			lineGeometry.vertices.push(new Vector3(0, 0, 0));
			lineGeometry.vertices.push(new Vector3(0, 1, 0));

			line = new Line(lineGeometry, new LineBasicMaterial(create(new {color = hex})));
			add(line);

			var coneGeometry = new CylinderGeometry(0, 0.05, 0.25, 5, 1);

			cone = new Mesh(coneGeometry, new MeshBasicMaterial(create(new {color = hex})));
			cone.position.set(0, 1, 0);
			add(cone);

			position = origin;

			setDirection(dir);
			setLength(length);
		}

		public void setDirection(Vector3 dir)
		{
			var d = __v1.copy(dir).normalize();

			if (d.y > 0.999)
			{
				rotation.set(0, 0, 0);
			}
			else if (d.y < -0.999)
			{
				rotation.set(System.Math.PI, 0, 0);
			}
			else
			{
				var axis = __v2.set(d.z, 0, -d.x).normalize();
				var radians = System.Math.Acos(d.y);
				rotation.setEulerFromQuaternion(__q1.setFromAxisAngle(axis, radians), eulerOrder);
			}
		}

		public void setLength(double length)
		{
			scale.set(length, length, length);
		}

		public void setColor(int hex)
		{
			line.material.color.setHex(hex);
			cone.material.color.setHex(hex);
		}
	}
}
