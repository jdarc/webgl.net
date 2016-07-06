using WebGL;

namespace THREE
{
	public class TetrahedronGeometry : PolyhedronGeometry
	{
		public TetrahedronGeometry(double radius, int detail = 0)
		{
			call(this, new JSArray(new[] {1.0, 1.0, 1.0}, new[] {-1.0, -1.0, 1.0}, new[] {-1.0, 1.0, -1.0}, new[] {1.0, -1.0, -1.0}),
			     new JSArray(new[] {2, 1, 0}, new[] {0, 3, 2}, new[] {1, 3, 0}, new[] {2, 3, 1}), radius, detail);
		}
	}
}
