using WebGL;

namespace THREE
{
	public class IcosahedronGeometry : PolyhedronGeometry
	{
		public IcosahedronGeometry(double radius = 1, int detail = 0)
		{
			var t = (1.0 + System.Math.Sqrt(5.0)) / 2.0;

			call(this,
			     new JSArray(
			     	new[] {-1, t, 0}, new[] {1, t, 0}, new[] {-1, -t, 0}, new[] {1, -t, 0},
			     	new[] {0, -1, t}, new[] {0, 1, t}, new[] {0, -1, -t}, new[] {0, 1, -t},
			     	new[] {t, 0, -1}, new[] {t, 0, 1}, new[] {-t, 0, -1}, new[] {-t, 0, 1}),
			     new JSArray(
			     	new[] {0, 11, 5}, new[] {0, 5, 1}, new[] {0, 1, 7}, new[] {0, 7, 10}, new[] {0, 10, 11},
			     	new[] {1, 5, 9}, new[] {5, 11, 4}, new[] {11, 10, 2}, new[] {10, 7, 6}, new[] {7, 1, 8},
			     	new[] {3, 9, 4}, new[] {3, 4, 2}, new[] {3, 2, 6}, new[] {3, 6, 8}, new[] {3, 8, 9},
			     	new[] {4, 9, 5}, new[] {2, 4, 11}, new[] {6, 2, 10}, new[] {8, 6, 7}, new[] {9, 8, 1}),
			     radius,
			     detail);
		}
	}
}
