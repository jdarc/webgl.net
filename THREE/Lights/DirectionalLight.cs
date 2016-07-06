using WebGL;

namespace THREE
{
	public class DirectionalLight : Light
	{
		public Object3D target;
		public double intensity;
		public double shadowCameraNear;
		public double shadowCameraFar;
		public double shadowCameraLeft;
		public double shadowCameraRight;
		public double shadowCameraTop;
		public double shadowCameraBottom;
		public bool shadowCameraVisible;
		public double shadowBias;
		public double shadowDarkness;
		public int shadowMapWidth;
		public int shadowMapHeight;
		public bool shadowCascade;
		public Vector3 shadowCascadeOffset;
		public int shadowCascadeCount;
		public JSArray shadowCascadeBias;
		public JSArray shadowCascadeWidth;
		public JSArray shadowCascadeHeight;
		public JSArray shadowCascadeNearZ;
		public JSArray shadowCascadeFarZ;
		public JSArray shadowCascadeArray;
		public dynamic shadowMap;
		public dynamic shadowMapSize;
		public dynamic shadowCamera;
		public dynamic shadowMatrix;

		public DirectionalLight(int hex = 0xFFFFFF, double intensity = 1.0) : base(hex)
		{
			position = new Vector3(0, 1, 0);
			target = new Object3D();

			this.intensity = intensity;

			castShadow = false;
			onlyShadow = false;

			shadowCameraNear = 50;
			shadowCameraFar = 5000;

			shadowCameraLeft = -500;
			shadowCameraRight = 500;
			shadowCameraTop = 500;
			shadowCameraBottom = -500;

			shadowCameraVisible = false;

			shadowBias = 0;
			shadowDarkness = 0.5;

			shadowMapWidth = 512;
			shadowMapHeight = 512;

			shadowCascade = false;

			shadowCascadeOffset = new Vector3(0, 0, -1000);
			shadowCascadeCount = 2;

			shadowCascadeBias = new JSArray(0.0, 0.0, 0.0);
			shadowCascadeWidth = new JSArray(512, 512, 512);
			shadowCascadeHeight = new JSArray(512, 512, 512);

			shadowCascadeNearZ = new JSArray(-1.000, 0.990, 0.998);
			shadowCascadeFarZ = new JSArray(0.990, 0.998, 1.000);

			shadowCascadeArray = new JSArray();

			shadowMap = null;
			shadowMapSize = null;
			shadowCamera = null;
			shadowMatrix = null;
		}
	}
}
