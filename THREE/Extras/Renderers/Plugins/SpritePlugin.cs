using WebGL;

namespace THREE
{
	public class SpritePlugin : Plugin
	{
		private WebGLRenderingContext _gl;
		private WebGLRenderer _renderer;
		private string _precision;
		private dynamic _sprite = new JSObject();

		public override void init(dynamic renderer)
		{
			_gl = renderer.context;
			_renderer = renderer;

			_precision = renderer.getPrecision();

			_sprite.vertices = new Float32Array(8 + 8);
			_sprite.faces = new Uint16Array(6);

			var i = 0;

			_sprite.vertices[i++] = -1;
			_sprite.vertices[i++] = -1; // vertex 0
			_sprite.vertices[i++] = 0;
			_sprite.vertices[i++] = 0; // uv 0

			_sprite.vertices[i++] = 1;
			_sprite.vertices[i++] = -1; // vertex 1
			_sprite.vertices[i++] = 1;
			_sprite.vertices[i++] = 0; // uv 1

			_sprite.vertices[i++] = 1;
			_sprite.vertices[i++] = 1; // vertex 2
			_sprite.vertices[i++] = 1;
			_sprite.vertices[i++] = 1; // uv 2

			_sprite.vertices[i++] = -1;
			_sprite.vertices[i++] = 1; // vertex 3
			_sprite.vertices[i++] = 0;
			_sprite.vertices[i] = 1; // uv 3

			i = 0;

			_sprite.faces[i++] = 0;
			_sprite.faces[i++] = 1;
			_sprite.faces[i++] = 2;
			_sprite.faces[i++] = 0;
			_sprite.faces[i++] = 2;
			_sprite.faces[i] = 3;

			_sprite.vertexBuffer = _gl.createBuffer();
			_sprite.elementBuffer = _gl.createBuffer();

			_gl.bindBuffer(_gl.ARRAY_BUFFER, _sprite.vertexBuffer);
			_gl.bufferData(_gl.ARRAY_BUFFER, _sprite.vertices, _gl.STATIC_DRAW);

			_gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, _sprite.elementBuffer);
			_gl.bufferData(_gl.ELEMENT_ARRAY_BUFFER, _sprite.faces, _gl.STATIC_DRAW);

			_sprite.program = createProgram(THREE.ShaderSprite["sprite"], _precision);

			_sprite.attributes = new JSObject();
			_sprite.uniforms = new JSObject();

			_sprite.attributes.position = _gl.getAttribLocation(_sprite.program, "position");
			_sprite.attributes.uv = _gl.getAttribLocation(_sprite.program, "uv");

			_sprite.uniforms.uvOffset = _gl.getUniformLocation(_sprite.program, "uvOffset");
			_sprite.uniforms.uvScale = _gl.getUniformLocation(_sprite.program, "uvScale");

			_sprite.uniforms.rotation = _gl.getUniformLocation(_sprite.program, "rotation");
			_sprite.uniforms.scale = _gl.getUniformLocation(_sprite.program, "scale");
			_sprite.uniforms.alignment = _gl.getUniformLocation(_sprite.program, "alignment");

			_sprite.uniforms.color = _gl.getUniformLocation(_sprite.program, "color");
			_sprite.uniforms.map = _gl.getUniformLocation(_sprite.program, "map");
			_sprite.uniforms.opacity = _gl.getUniformLocation(_sprite.program, "opacity");

			_sprite.uniforms.useScreenCoordinates = _gl.getUniformLocation(_sprite.program, "useScreenCoordinates");
			_sprite.uniforms.sizeAttenuation = _gl.getUniformLocation(_sprite.program, "sizeAttenuation");
			_sprite.uniforms.screenPosition = _gl.getUniformLocation(_sprite.program, "screenPosition");
			_sprite.uniforms.modelViewMatrix = _gl.getUniformLocation(_sprite.program, "modelViewMatrix");
			_sprite.uniforms.projectionMatrix = _gl.getUniformLocation(_sprite.program, "projectionMatrix");

			_sprite.uniforms.fogType = _gl.getUniformLocation(_sprite.program, "fogType");
			_sprite.uniforms.fogDensity = _gl.getUniformLocation(_sprite.program, "fogDensity");
			_sprite.uniforms.fogNear = _gl.getUniformLocation(_sprite.program, "fogNear");
			_sprite.uniforms.fogFar = _gl.getUniformLocation(_sprite.program, "fogFar");
			_sprite.uniforms.fogColor = _gl.getUniformLocation(_sprite.program, "fogColor");

			_sprite.uniforms.alphaTest = _gl.getUniformLocation(_sprite.program, "alphaTest");
		}

		public override void update(dynamic scene, dynamic camera)
		{
		}

		public override void render(dynamic scene, dynamic camera, int viewportWidth = 0, int viewportHeight = 0)
		{
			JSArray sprites = scene.__webglSprites;
			var nSprites = sprites.length;

			if (!JSObject.eval(nSprites))
			{
				return;
			}

			var attributes = _sprite.attributes;
			var uniforms = _sprite.uniforms;

			var invAspect = viewportHeight / (double)viewportWidth;

			var halfViewportWidth = viewportWidth * 0.5;
			var halfViewportHeight = viewportHeight * 0.5;

			// setup gl

			_gl.useProgram(_sprite.program);

			_gl.enableVertexAttribArray((uint)attributes.position);
			_gl.enableVertexAttribArray((uint)attributes.uv);

			_gl.disable(_gl.CULL_FACE);
			_gl.enable(_gl.BLEND);

			_gl.bindBuffer(_gl.ARRAY_BUFFER, _sprite.vertexBuffer);
			_gl.vertexAttribPointer((uint)attributes.position, 2, _gl.FLOAT, false, 2 * 8, 0);
			_gl.vertexAttribPointer((uint)attributes.uv, 2, _gl.FLOAT, false, 2 * 8, 8);

			_gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, _sprite.elementBuffer);

			_gl.uniformMatrix4fv(uniforms.projectionMatrix, false, camera.projectionMatrix.elements);

			_gl.activeTexture(_gl.TEXTURE0);
			_gl.uniform1i(uniforms.map, 0);

			var oldFogType = 0;
			var sceneFogType = 0;
			var fog = scene.fog;

			if (fog != null)
			{
				_gl.uniform3f(uniforms.fogColor, (float)fog.color.r, (float)fog.color.g, (float)fog.color.b);

				if (fog is Fog)
				{
					_gl.uniform1f(uniforms.fogNear, (float)fog.near);
					_gl.uniform1f(uniforms.fogFar, (float)fog.far);

					_gl.uniform1i(uniforms.fogType, 1);
					oldFogType = 1;
					sceneFogType = 1;
				}
				else if (fog is FogExp2)
				{
					_gl.uniform1f(uniforms.fogDensity, (float)fog.density);

					_gl.uniform1i(uniforms.fogType, 2);
					oldFogType = 2;
					sceneFogType = 2;
				}
			}
			else
			{
				_gl.uniform1i(uniforms.fogType, 0);
				oldFogType = 0;
				sceneFogType = 0;
			}

			// update positions and sort
			dynamic sprite;
			SpriteMaterial material;
			var scale = new Float32Array(2);

			for (var i = 0; i < nSprites; i ++)
			{
				sprite = sprites[i];
				material = sprite.material;

				if (! sprite.visible || material.opacity == 0)
				{
					continue;
				}

				if (! material.useScreenCoordinates)
				{
					sprite._modelViewMatrix.multiplyMatrices(camera.matrixWorldInverse, sprite.matrixWorld);
					sprite.z = - sprite._modelViewMatrix.elements[14];
				}
				else
				{
					sprite.z = - sprite.position.z;
				}
			}

			sprites.sort(painterSortStable);

			// render all sprites

			for (var i = 0; i < nSprites; i ++)
			{
				sprite = sprites[i];
				material = sprite.material;

				if (!sprite.visible || material.opacity == 0)
				{
					continue;
				}

				if (material.map != null && material.map.image != null && JSObject.eval(material.map.image.width))
				{
					_gl.uniform1f(uniforms.alphaTest, (float)material.alphaTest);

					if (material.useScreenCoordinates)
					{
						_gl.uniform1i(uniforms.useScreenCoordinates, 1);
						_gl.uniform3f(uniforms.screenPosition,
						              (float)(((sprite.position.x * _renderer.devicePixelRatio) - halfViewportWidth) / halfViewportWidth),
						              (float)((halfViewportHeight - (sprite.position.y * _renderer.devicePixelRatio)) / halfViewportHeight),
						              (float)(System.Math.Max(0, System.Math.Min(1, sprite.position.z))));

						scale[0] = _renderer.devicePixelRatio;
						scale[1] = _renderer.devicePixelRatio;
					}
					else
					{
						_gl.uniform1i(uniforms.useScreenCoordinates, 0);
						_gl.uniform1i(uniforms.sizeAttenuation, material.sizeAttenuation ? 1 : 0);
						_gl.uniformMatrix4fv(uniforms.modelViewMatrix, false, sprite._modelViewMatrix.elements);

						scale[0] = 1;
						scale[1] = 1;
					}

					int fogType = scene.fog != null && material.fog ? sceneFogType : 0;

					if (oldFogType != fogType)
					{
						_gl.uniform1i(uniforms.fogType, fogType);
						oldFogType = fogType;
					}

					var size = 1.0 / (material.scaleByViewport ? viewportHeight : 1);

					scale[0] *= size * invAspect * sprite.scale.x;
					scale[1] *= size * sprite.scale.y;

					_gl.uniform2f(uniforms.uvScale, (float)material.uvScale.x, (float)material.uvScale.y);
					_gl.uniform2f(uniforms.uvOffset, (float)material.uvOffset.x, (float)material.uvOffset.y);
					_gl.uniform2f(uniforms.alignment, (float)material.alignment.x, (float)material.alignment.y);

					_gl.uniform1f(uniforms.opacity, (float)material.opacity);
					_gl.uniform3f(uniforms.color, (float)material.color.r, (float)material.color.g, (float)material.color.b);

					_gl.uniform1f(uniforms.rotation, (float)sprite.rotation);
					_gl.uniform2fv(uniforms.scale, scale);

					_renderer.setBlending(material.blending, material.blendEquation, material.blendSrc, material.blendDst);
					_renderer.setDepthTest(material.depthTest);
					_renderer.setDepthWrite(material.depthWrite);
					_renderer.setTexture(material.map, 0);

					_gl.drawElements(_gl.TRIANGLES, 6, _gl.UNSIGNED_SHORT, 0);
				}
			}

			// restore gl

			_gl.enable(_gl.CULL_FACE);
		}

		public dynamic createProgram(dynamic shader, dynamic precision)
		{
			var program = _gl.createProgram();

			var fragmentShader = _gl.createShader(_gl.FRAGMENT_SHADER);
			var vertexShader = _gl.createShader(_gl.VERTEX_SHADER);

			var prefix = "precision " + precision + " float;\n";

			_gl.shaderSource(fragmentShader, prefix + shader.fragmentShader);
			_gl.shaderSource(vertexShader, prefix + shader.vertexShader);

			_gl.compileShader(fragmentShader);
			_gl.compileShader(vertexShader);

			_gl.attachShader(program, fragmentShader);
			_gl.attachShader(program, vertexShader);

			_gl.linkProgram(program);

			return program;
		}

		private static int painterSortStable(dynamic a, dynamic b)
		{
			var az = JSObject.safe<double>(a.z);
			var bz = JSObject.safe<double>(b.z);
			if (!(double.IsNaN(az) || double.IsNaN(bz)) && az != bz)
			{
				var result = (bz - az);
				return result < 0 ? -1 : (result > 0 ? 1 : 0);
			}
			return JSObject.safe<int>(b.id) - JSObject.safe<int>(a.id);
		}
	}
}
