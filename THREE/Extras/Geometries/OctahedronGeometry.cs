using WebGL;

namespace THREE
{
	public class OctahedronGeometry : PolyhedronGeometry
	{
		public OctahedronGeometry(double radius, int detail = 0)
		{
			call(this,
			     new JSArray(
			     	new[] {1.0, 0.0, 0.0}, new[] {-1.0, 0.0, 0.0}, new[] {0.0, 1.0, 0.0},
			     	new[] {0.0, -1.0, 0.0}, new[] {0.0, 0.0, 1.0}, new[] {0.0, 0.0, -1.0}),
			     new JSArray(
			     	new[] {0, 2, 4}, new[] {0, 4, 3}, new[] {0, 3, 5}, new[] {0, 5, 2},
			     	new[] {1, 2, 5}, new[] {1, 5, 3}, new[] {1, 3, 4}, new[] {1, 4, 2}),
			     radius,
			     detail);
		}
	}
}
