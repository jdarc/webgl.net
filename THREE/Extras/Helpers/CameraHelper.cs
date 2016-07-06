using System;
using WebGL;

namespace THREE
{
	public class CameraHelper : Line
	{
		private static readonly Projector __projector = new Projector();
		private static readonly Vector3 __v = new Vector3();
		private static readonly Camera __c = new Camera();

		public JSObject pointMap;
		public Camera camera;

		public CameraHelper(Camera camera) :
			base(new Geometry(), new LineBasicMaterial(JSObject.create(new {color = 0xffffff, vertexColors = THREE.FaceColors})), THREE.LinePieces)
		{
			var scope = this;

			Action<string, int> addPoint = (id, hex) =>
			{
				scope.geometry.vertices.push(new Vector3());
				scope.geometry.colors.push(new Color(hex));

				if (scope.pointMap[id] == null)
				{
					scope.pointMap[id] = new JSArray();
				}

				scope.pointMap[id].push(scope.geometry.vertices.length - 1);
			};

			Action<string, string, int> addLine = (a, b, hex) =>
			{
				addPoint(a, hex);
				addPoint(b, hex);
			};

			matrixWorld = camera.matrixWorld;
			matrixAutoUpdate = false;

			pointMap = new JSObject();

			const int hexFrustum = 0xffaa00;
			const int hexCone = 0xff0000;
			const int hexUp = 0x00aaff;
			const int hexTarget = 0xffffff;
			const int hexCross = 0x333333;

			addLine("n1", "n2", hexFrustum);
			addLine("n2", "n4", hexFrustum);
			addLine("n4", "n3", hexFrustum);
			addLine("n3", "n1", hexFrustum);

			addLine("f1", "f2", hexFrustum);
			addLine("f2", "f4", hexFrustum);
			addLine("f4", "f3", hexFrustum);
			addLine("f3", "f1", hexFrustum);

			addLine("n1", "f1", hexFrustum);
			addLine("n2", "f2", hexFrustum);
			addLine("n3", "f3", hexFrustum);
			addLine("n4", "f4", hexFrustum);

			addLine("p", "n1", hexCone);
			addLine("p", "n2", hexCone);
			addLine("p", "n3", hexCone);
			addLine("p", "n4", hexCone);

			addLine("u1", "u2", hexUp);
			addLine("u2", "u3", hexUp);
			addLine("u3", "u1", hexUp);

			addLine("c", "t", hexTarget);
			addLine("p", "c", hexCross);

			addLine("cn1", "cn2", hexCross);
			addLine("cn3", "cn4", hexCross);

			addLine("cf1", "cf2", hexCross);
			addLine("cf3", "cf4", hexCross);

			this.camera = camera;

			update();
		}

		public void update()
		{
			var scope = this;

			Action<string, double, double, double> setPoint = (point, x, y, z) =>
			{
				__v.set(x, y, z);
				__projector.unprojectVector(__v, __c);

				var points = scope.pointMap[point];

				if (points != null)
				{
					for (var i = 0; i < (int)points.length; i++)
					{
						scope.geometry.vertices[points[i]].copy(__v);
					}
				}
			};

			const int w = 1;
			const int h = 1;

			__c.projectionMatrix.copy(camera.projectionMatrix);

			setPoint("c", 0, 0, -1);
			setPoint("t", 0, 0, 1);

			setPoint("n1", -w, -h, -1);
			setPoint("n2", w, -h, -1);
			setPoint("n3", -w, h, -1);
			setPoint("n4", w, h, -1);

			setPoint("f1", -w, -h, 1);
			setPoint("f2", w, -h, 1);
			setPoint("f3", -w, h, 1);
			setPoint("f4", w, h, 1);

			setPoint("u1", w * 0.7, h * 1.1, -1);
			setPoint("u2", -w * 0.7, h * 1.1, -1);
			setPoint("u3", 0, h * 2, -1);

			setPoint("cf1", -w, 0, 1);
			setPoint("cf2", w, 0, 1);
			setPoint("cf3", 0, -h, 1);
			setPoint("cf4", 0, h, 1);

			setPoint("cn1", -w, 0, -1);
			setPoint("cn2", w, 0, -1);
			setPoint("cn3", 0, -h, -1);
			setPoint("cn4", 0, h, -1);

			geometry.verticesNeedUpdate = true;
		}
	}
}
