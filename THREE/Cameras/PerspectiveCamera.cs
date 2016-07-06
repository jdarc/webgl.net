namespace THREE
{
	public class PerspectiveCamera : Camera
	{
		public double fov;
		public double aspect;
		public double near;
		public double far;

		public dynamic fullWidth;
		public double fullHeight;
		public double x;
		public double y;
		public double width;
		public double height;

		public PerspectiveCamera(double fov = 50.0, double aspect = 1.0, double near = 0.1, double far = 2000.0)
		{
			this.fov = fov;
			this.aspect = aspect;
			this.near = near;
			this.far = far;

			updateProjectionMatrix();
		}

		public void setLens(double focalLength, double frameHeight = 24.0)
		{
			fov = 2.0 * Math.radToDeg(System.Math.Atan(frameHeight / (focalLength * 2.0)));
			updateProjectionMatrix();
		}

		public void setViewOffset(double fullWidth, double fullHeight, double x, double y, double width, double height)
		{
			this.fullWidth = fullWidth;
			this.fullHeight = fullHeight;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;

			updateProjectionMatrix();
		}

		public void updateProjectionMatrix()
		{
			if (fullWidth != null)
			{
				var aspect = fullWidth / fullHeight;
				var top = System.Math.Tan(Math.degToRad(fov * 0.5)) * near;
				var bottom = -top;
				var left = aspect * bottom;
				var right = aspect * top;
				var width = Math.abs(right - left);
				var height = System.Math.Abs(top - bottom);

				projectionMatrix.makeFrustum(left + x * width / fullWidth,
				                             left + (x + this.width) * width / fullWidth,
				                             top - (y + this.height) * height / fullHeight,
				                             top - y * height / fullHeight,
				                             near,
				                             far);
			}
			else
			{
				projectionMatrix.makePerspective(fov, aspect, near, far);
			}
		}
	}
}
