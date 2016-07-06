namespace THREE
{
	public class SpotLight : Light
	{
		public Object3D target;
		public double intensity;
		public double distance;
		public double angle;
		public double exponent;
		public double shadowCameraNear;
		public double shadowCameraFar;
		public double shadowCameraFov;
		public bool shadowCameraVisible;
		public double shadowBias;
		public double shadowDarkness;
		public int shadowMapWidth;
		public int shadowMapHeight;
		public dynamic shadowMap;
		public dynamic shadowMapSize;
		public dynamic shadowCamera;
		public dynamic shadowMatrix;

		public SpotLight(int hex = 0xFFFFFF, double intensity = 1.0, double distance = 0.0, double angle = System.Math.PI / 2.0, double exponent = 10.0) : base(hex)
		{
			position = new Vector3(0, 1, 0);
			target = new Object3D();

			this.intensity = intensity;
			this.distance = distance;
			this.angle = angle;
			this.exponent = exponent;

			castShadow = false;
			onlyShadow = false;

			shadowCameraNear = 50;
			shadowCameraFar = 5000;
			shadowCameraFov = 50;

			shadowCameraVisible = false;

			shadowBias = 0;
			shadowDarkness = 0.5;

			shadowMapWidth = 512;
			shadowMapHeight = 512;

			shadowMap = null;
			shadowMapSize = null;
			shadowCamera = null;
			shadowMatrix = null;
		}
	}
}
