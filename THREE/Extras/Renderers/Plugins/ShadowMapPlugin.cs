using System;
using WebGL;

namespace THREE
{
	public class ShadowMapPlugin : Plugin
	{
		public static readonly Projector __projector = new Projector();

		private dynamic _gl;
		private WebGLRenderer _renderer;
		private dynamic _depthMaterial;
		private dynamic _depthMaterialMorph;
		private dynamic _depthMaterialSkin;
		private dynamic _depthMaterialMorphSkin;

		private readonly Frustum _frustum = new Frustum();
		private readonly Matrix4 _projScreenMatrix = new Matrix4();
		private readonly Vector3 _min = new Vector3();
		private readonly Vector3 _max = new Vector3();

		public override void init(dynamic renderer)
		{
			_gl = renderer.context;
			_renderer = renderer;

			var depthShader = THREE.ShaderLib["depthRGBA"];
			var depthUniforms = WebGLShaders.UniformsUtils.clone(depthShader.uniforms);

			_depthMaterial = new ShaderMaterial(JSObject.create(new {depthShader.fragmentShader, depthShader.vertexShader, uniforms = depthUniforms}));
			_depthMaterialMorph = new ShaderMaterial(JSObject.create(new {depthShader.fragmentShader, depthShader.vertexShader, uniforms = depthUniforms, morphTargets = true}));
			_depthMaterialSkin = new ShaderMaterial(JSObject.create(new {depthShader.fragmentShader, depthShader.vertexShader, uniforms = depthUniforms, skinning = true}));
			_depthMaterialMorphSkin = new ShaderMaterial(JSObject.create(new {depthShader.fragmentShader, depthShader.vertexShader, uniforms = depthUniforms, morphTargets = true, skinning = true}));

			_depthMaterial._shadowPass = true;
			_depthMaterialMorph._shadowPass = true;
			_depthMaterialSkin._shadowPass = true;
			_depthMaterialMorphSkin._shadowPass = true;
		}

		public override void render(dynamic scene, dynamic camera, int viewportWidth = 0, int viewportHeight = 0)
		{
			if (_renderer.shadowMapEnabled && _renderer.shadowMapAutoUpdate)
			{
				this.update(scene, camera);
			}
		}

		public override void update(dynamic scene, dynamic camera)
		{
			dynamic i;
			dynamic il;
			dynamic shadowMatrix;
			dynamic shadowCamera;
			dynamic webglObject;
			dynamic obj;
			dynamic light;
			dynamic renderList;
			dynamic lights = new JSArray();
			dynamic k = 0;
			dynamic fog = null;

			_gl.clearColor(1, 1, 1, 1);
			_gl.disable(_gl.BLEND);

			_gl.enable(_gl.CULL_FACE);
			_gl.frontFace(_gl.CCW);

			_gl.cullFace(_renderer.shadowMapCullFace == THREE.CullFaceFront ? _gl.FRONT : _gl.BACK);

			_renderer.setDepthTest(true);

			dynamic virtualLight = null;
			for (i = 0, il = scene.__lights.Count; i < il; i++)
			{
				light = scene.__lights[i];

				if (! light.castShadow)
				{
					continue;
				}

				if ((light is DirectionalLight) && light.shadowCascade)
				{
					dynamic n;
					for (n = 0; n < light.shadowCascadeCount; n++)
					{
						if (! light.shadowCascadeArray[n])
						{
							virtualLight = createVirtualLight(light, n);
							virtualLight.originalCamera = camera;

							var gyro = new Gyroscope {position = light.shadowCascadeOffset};

							gyro.add(virtualLight);
							gyro.add(virtualLight.target);

							camera.add(gyro);

							light.shadowCascadeArray[n] = virtualLight;

							JSConsole.log(String.Format("Created virtualLight", virtualLight));
						}
						else
						{
							virtualLight = light.shadowCascadeArray[n];
						}

						updateVirtualLight(light, n);

						lights[k] = virtualLight;
						k++;
					}
				}
				else
				{
					lights[k] = light;
					k++;
				}
			}

			for (i = 0, il = lights.length; i < il; i++)
			{
				light = lights[i];

				if (light.shadowMap == null)
				{
					var shadowFilter = THREE.LinearFilter;

					if (_renderer.shadowMapType == THREE.PCFSoftShadowMap)
					{
						shadowFilter = THREE.NearestFilter;
					}

					var pars = JSObject.create(new {minFilter = shadowFilter, magFilter = shadowFilter, format = THREE.RGBAFormat});

					light.shadowMap = new WebGLRenderTarget(light.shadowMapWidth, light.shadowMapHeight, pars);
					light.shadowMapSize = new Vector2(light.shadowMapWidth, light.shadowMapHeight);

					light.shadowMatrix = new Matrix4();
				}

				if (light.shadowCamera == null)
				{
					if (light is SpotLight)
					{
						light.shadowCamera = new PerspectiveCamera(light.shadowCameraFov, light.shadowMapWidth / light.shadowMapHeight, light.shadowCameraNear, light.shadowCameraFar);
					}
					else if (light is DirectionalLight)
					{
						light.shadowCamera = new OrthographicCamera(light.shadowCameraLeft, light.shadowCameraRight, light.shadowCameraTop, light.shadowCameraBottom, light.shadowCameraNear, light.shadowCameraFar);
					}
					else
					{
						JSConsole.error("Unsupported light type for shadow");
						continue;
					}

					scene.add(light.shadowCamera);

					if (_renderer.autoUpdateScene)
					{
						scene.updateMatrixWorld();
					}
				}

				if (light.shadowCameraVisible && light.cameraHelper == null)
				{
					light.cameraHelper = new CameraHelper(light.shadowCamera);
					light.shadowCamera.add(light.cameraHelper);
				}

				if ((light.isVirtual ?? false) && virtualLight != null && virtualLight.originalCamera == camera)
				{
					updateShadowCamera(camera, light);
				}

				var shadowMap = light.shadowMap;
				shadowMatrix = light.shadowMatrix;
				shadowCamera = light.shadowCamera;

				shadowCamera.position.copy(light.matrixWorld.getPosition());
				shadowCamera.lookAt(light.target.matrixWorld.getPosition());
				shadowCamera.updateMatrixWorld();

				shadowCamera.matrixWorldInverse.getInverse(shadowCamera.matrixWorld);

				if (light.cameraHelper != null)
				{
					light.cameraHelper.visible = light.shadowCameraVisible;
				}
				if (light.shadowCameraVisible)
				{
					light.cameraHelper.update();
				}

				shadowMatrix.set(0.5, 0.0, 0.0, 0.5,
				                 0.0, 0.5, 0.0, 0.5,
				                 0.0, 0.0, 0.5, 0.5,
				                 0.0, 0.0, 0.0, 1.0);

				shadowMatrix.multiply(shadowCamera.projectionMatrix);
				shadowMatrix.multiply(shadowCamera.matrixWorldInverse);

				_projScreenMatrix.multiplyMatrices(shadowCamera.projectionMatrix, shadowCamera.matrixWorldInverse);
				_frustum.setFromMatrix(_projScreenMatrix);

				_renderer.setRenderTarget(shadowMap);
				_renderer.clear();

				renderList = scene.__webglObjects;

				dynamic j;
				dynamic jl;
				for (j = 0, jl = renderList.length; j < jl; j++)
				{
					webglObject = renderList[j];
					obj = webglObject.obj;

					webglObject.render = false;

					if (obj.visible && obj.castShadow)
					{
						if (! (obj is Mesh || obj is ParticleSystem) || ! (obj.frustumCulled) || _frustum.intersectsObject(obj))
						{
							obj._modelViewMatrix.multiplyMatrices(shadowCamera.matrixWorldInverse, obj.matrixWorld);

							webglObject.render = true;
						}
					}
				}

				dynamic objectMaterial;

				for (j = 0, jl = renderList.length; j < jl; j++)
				{
					webglObject = renderList[j];

					if (webglObject.render)
					{
						obj = webglObject.obj;
						var buffer = webglObject.buffer;

						objectMaterial = getObjectMaterial(obj);

						var useMorphing = obj.geometry.morphTargets.length > 0 && objectMaterial.morphTargets;
						var useSkinning = obj is SkinnedMesh && objectMaterial.skinning;

						dynamic material;
						if (obj.customDepthMaterial != null)
						{
							material = obj.customDepthMaterial;
						}
						else if (useSkinning)
						{
							material = useMorphing ? _depthMaterialMorphSkin : _depthMaterialSkin;
						}
						else if (useMorphing)
						{
							material = _depthMaterialMorph;
						}
						else
						{
							material = _depthMaterial;
						}

						if (buffer is BufferGeometry)
						{
							_renderer.renderBufferDirect(shadowCamera, scene.__lights, fog, material, buffer, obj);
						}
						else
						{
							_renderer.renderBuffer(shadowCamera, scene.__lights, fog, material, buffer, obj);
						}
					}
				}

				renderList = scene.__webglObjectsImmediate;

				for (j = 0, jl = renderList.length; j < jl; j++)
				{
					webglObject = renderList[j];
					obj = webglObject.obj;

					if (obj.visible && obj.castShadow)
					{
						obj._modelViewMatrix.multiplyMatrices(shadowCamera.matrixWorldInverse, obj.matrixWorld);

						_renderer.renderImmediateObject(shadowCamera, scene.__lights, fog, _depthMaterial, obj);
					}
				}
			}

			var clearColor = _renderer.getClearColor();
			var clearAlpha = _renderer.getClearAlpha();

			_gl.clearColor((float)clearColor.r, (float)clearColor.g, (float)clearColor.b, (float)clearAlpha);
			_gl.enable(_gl.BLEND);

			if (_renderer.shadowMapCullFace == THREE.CullFaceFront)
			{
				_gl.cullFace(_gl.BACK);
			}
		}

		private dynamic createVirtualLight(dynamic light, dynamic cascade)
		{
			dynamic virtualLight = new DirectionalLight();

			virtualLight.isVirtual = true;

			virtualLight.onlyShadow = true;
			virtualLight.castShadow = true;

			virtualLight.shadowCameraNear = light.shadowCameraNear;
			virtualLight.shadowCameraFar = light.shadowCameraFar;

			virtualLight.shadowCameraLeft = light.shadowCameraLeft;
			virtualLight.shadowCameraRight = light.shadowCameraRight;
			virtualLight.shadowCameraBottom = light.shadowCameraBottom;
			virtualLight.shadowCameraTop = light.shadowCameraTop;

			virtualLight.shadowCameraVisible = light.shadowCameraVisible;

			virtualLight.shadowDarkness = light.shadowDarkness;

			virtualLight.shadowBias = light.shadowCascadeBias[cascade];
			virtualLight.shadowMapWidth = light.shadowCascadeWidth[cascade];
			virtualLight.shadowMapHeight = light.shadowCascadeHeight[cascade];

			virtualLight.pointsWorld = new JSArray();
			virtualLight.pointsFrustum = new JSArray();

			var pointsWorld = virtualLight.pointsWorld;
			var pointsFrustum = virtualLight.pointsFrustum;

			for (var i = 0; i < 8; i++)
			{
				pointsWorld[i] = new Vector3();
				pointsFrustum[i] = new Vector3();
			}

			var nearZ = light.shadowCascadeNearZ[cascade];
			var farZ = light.shadowCascadeFarZ[cascade];

			pointsFrustum[0].set(-1, -1, nearZ);
			pointsFrustum[1].set(1, -1, nearZ);
			pointsFrustum[2].set(-1, 1, nearZ);
			pointsFrustum[3].set(1, 1, nearZ);

			pointsFrustum[4].set(-1, -1, farZ);
			pointsFrustum[5].set(1, -1, farZ);
			pointsFrustum[6].set(-1, 1, farZ);
			pointsFrustum[7].set(1, 1, farZ);

			return virtualLight;
		}

		private void updateVirtualLight(dynamic light, dynamic cascade)
		{
			var virtualLight = light.shadowCascadeArray[cascade];

			virtualLight.position.copy(light.position);
			virtualLight.target.position.copy(light.target.position);
			virtualLight.lookAt(virtualLight.target);

			virtualLight.shadowCameraVisible = light.shadowCameraVisible;
			virtualLight.shadowDarkness = light.shadowDarkness;

			virtualLight.shadowBias = light.shadowCascadeBias[cascade];

			var nearZ = light.shadowCascadeNearZ[cascade];
			var farZ = light.shadowCascadeFarZ[cascade];

			var pointsFrustum = virtualLight.pointsFrustum;

			pointsFrustum[0].z = nearZ;
			pointsFrustum[1].z = nearZ;
			pointsFrustum[2].z = nearZ;
			pointsFrustum[3].z = nearZ;

			pointsFrustum[4].z = farZ;
			pointsFrustum[5].z = farZ;
			pointsFrustum[6].z = farZ;
			pointsFrustum[7].z = farZ;
		}

		private void updateShadowCamera(dynamic camera, dynamic light)
		{
			var shadowCamera = light.shadowCamera;
			var pointsFrustum = light.pointsFrustum;
			var pointsWorld = light.pointsWorld;

			_min.set(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
			_max.set(-double.PositiveInfinity, -double.PositiveInfinity, -double.PositiveInfinity);

			for (var i = 0; i < 8; i++)
			{
				var p = pointsWorld[i];

				p.copy(pointsFrustum[i]);
				__projector.unprojectVector(p, camera);

				p.applyMatrix4(shadowCamera.matrixWorldInverse);

				if (p.x < _min.x)
				{
					_min.x = p.x;
				}
				if (p.x > _max.x)
				{
					_max.x = p.x;
				}

				if (p.y < _min.y)
				{
					_min.y = p.y;
				}
				if (p.y > _max.y)
				{
					_max.y = p.y;
				}

				if (p.z < _min.z)
				{
					_min.z = p.z;
				}
				if (p.z > _max.z)
				{
					_max.z = p.z;
				}
			}

			shadowCamera.left = _min.x;
			shadowCamera.right = _max.x;
			shadowCamera.top = _max.y;
			shadowCamera.bottom = _min.y;

			shadowCamera.updateProjectionMatrix();
		}

		private dynamic getObjectMaterial(dynamic obj)
		{
			return obj.material is MeshFaceMaterial ? obj.material.materials[0] : obj.material;
		}
	}
}
