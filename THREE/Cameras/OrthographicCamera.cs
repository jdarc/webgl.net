namespace THREE
{
	public class OrthographicCamera : Camera
	{
		public double left;
		public double right;
		public double top;
		public double bottom;
		public double near;
		public double far;

		public OrthographicCamera(double left, double right, double top, double bottom, double near = 0.1, double far = 2000.0)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
			this.near = near;
			this.far = far;

			updateProjectionMatrix();
		}

		public void updateProjectionMatrix()
		{
			projectionMatrix.makeOrthographic(left, right, top, bottom, near, far);
		}
	}
}
