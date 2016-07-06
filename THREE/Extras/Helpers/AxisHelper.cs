using WebGL;

namespace THREE
{
	public class AxisHelper : Line
	{
		public AxisHelper(double size = 1)
			: base(createGeometry(size), createMaterial(), THREE.LinePieces)
		{
		}

		private static Geometry createGeometry(double size)
		{
			var geom = new Geometry();

			geom.vertices.push(new Vector3(), new Vector3(size, 0, 0),
			                   new Vector3(), new Vector3(0, size, 0),
			                   new Vector3(), new Vector3(0, 0, size));

			geom.colors.push(new Color(0xff0000), new Color(0xffaa00),
			                 new Color(0x00ff00), new Color(0xaaff00),
			                 new Color(0x0000ff), new Color(0x00aaff));
			return geom;
		}

		private static Material createMaterial()
		{
			return new LineBasicMaterial(JSObject.create(new {vertexColors = THREE.VertexColors}));
		}
	}
}
