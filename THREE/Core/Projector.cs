using System;
using WebGL;

namespace THREE
{
	public class Projector
	{
		private dynamic _object;
		private dynamic _objectCount;
		private dynamic _objectPool;
		private dynamic _objectPoolLength;
		private dynamic _vertex;
		private dynamic _vertexCount;
		private dynamic _vertexPool;
		private dynamic _vertexPoolLength;
		private dynamic _face;
		private dynamic _face3Count;
		private dynamic _face3Pool;
		private dynamic _face3PoolLength;
		private dynamic _face4Count;
		private dynamic _face4Pool;
		private dynamic _face4PoolLength;
		private dynamic _line;
		private dynamic _lineCount;
		private dynamic _linePool;
		private dynamic _linePoolLength;
		private dynamic _particle;
		private dynamic _particleCount;
		private dynamic _particlePool;
		private dynamic _particlePoolLength;
		private dynamic _renderData;
		private dynamic _vector3;
		private dynamic _vector4;
		private dynamic _clipBox;
		private dynamic _boundingBox;
		private dynamic _points3;
		private dynamic _points4;
		private dynamic _viewMatrix;
		private dynamic _viewProjectionMatrix;
		private dynamic _modelMatrix;
		private dynamic _modelViewProjectionMatrix;
		private dynamic _normalMatrix;
		private dynamic _normalViewMatrix;
		private dynamic _centroid;
		private dynamic _frustum;
		private dynamic _clippedVertex1PositionScreen;
		private dynamic _clippedVertex2PositionScreen;
		private dynamic _face3VertexNormals;

		public Projector()
		{
			_object = null;
			_objectCount = 0;
			_objectPool = new JSArray();
			_objectPoolLength = 0;
			_vertex = null;
			_vertexCount = 0;
			_vertexPool = new JSArray();
			_vertexPoolLength = 0;
			_face = null;
			_face3Count = 0;
			_face3Pool = new JSArray();
			_face3PoolLength = 0;
			_face4Count = 0;
			_face4Pool = new JSArray();
			_face4PoolLength = 0;
			_line = null;
			_lineCount = 0;
			_linePool = new JSArray();
			_linePoolLength = 0;
			_particle = null;
			_particleCount = 0;
			_particlePool = new JSArray();
			_particlePoolLength = 0;
			_renderData = JSObject.create(new {objects = new JSArray(), sprites = new JSArray(), lights = new JSArray(), elements = new JSArray()});
			_vector3 = new Vector3();
			_vector4 = new Vector4();
			_clipBox = new Box3(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
			_boundingBox = new Box3();
			_points3 = new JSArray(3);
			_points4 = new JSArray(4);
			_viewMatrix = new Matrix4();
			_viewProjectionMatrix = new Matrix4();
			_modelMatrix = null;
			_modelViewProjectionMatrix = new Matrix4();
			_normalMatrix = new Matrix3();
			_normalViewMatrix = new Matrix3();
			_centroid = new Vector3();
			_frustum = new Frustum();
			_clippedVertex1PositionScreen = new Vector4();
			_clippedVertex2PositionScreen = new Vector4();
			_face3VertexNormals = null;
		}

		public virtual Vector3 projectVector(Vector3 vector, Camera camera)
		{
			camera.matrixWorldInverse.getInverse(camera.matrixWorld);

			_viewProjectionMatrix.multiplyMatrices(camera.projectionMatrix, camera.matrixWorldInverse);

			return vector.applyProjection(_viewProjectionMatrix);
		}

		public virtual Vector3 unprojectVector(Vector3 vector, Camera camera)
		{
			camera.projectionMatrixInverse.getInverse(camera.projectionMatrix);

			_viewProjectionMatrix.multiplyMatrices(camera.matrixWorld, camera.projectionMatrixInverse);

			return vector.applyProjection(_viewProjectionMatrix);
		}

		public Raycaster pickingRay(Vector3 vector, Camera camera)
		{
			// set two vectors with opposing z values
			vector.z = -1.0;
			var end = new Vector3(vector.x, vector.y, 1.0);

			unprojectVector(vector, camera);
			unprojectVector(end, camera);

			// find direction from vector to end
			end.sub(vector).normalize();

			return new Raycaster(vector, end);
		}

		public dynamic projectScene(Scene scene, Camera camera, dynamic sortObjects, dynamic sortElements)
		{
			dynamic visible = false,
			        o, ol, v, vl, f, fl, n, nl, c, cl, u, ul, obj,
			        geometry, vertices, vertex, vertexPositionScreen,
			        faces, face, faceVertexNormals, faceVertexUvs, uvs,
			        v1, v2, v3, v4, isFaceMaterial, objectMaterials;

			_face3Count = 0;
			_face4Count = 0;
			_lineCount = 0;
			_particleCount = 0;

			_renderData.elements.length = 0;

			scene.updateMatrixWorld();

			if (camera.parent == null)
			{
				camera.updateMatrixWorld();
			}

			_viewMatrix.copy(camera.matrixWorldInverse.getInverse(camera.matrixWorld));
			_viewProjectionMatrix.multiplyMatrices(camera.projectionMatrix, _viewMatrix);

			_normalViewMatrix.getInverse(_viewMatrix);
			_normalViewMatrix.transpose();

			_frustum.setFromMatrix(_viewProjectionMatrix);

			_renderData = projectGraph(scene, sortObjects);

			for (o = 0, ol = _renderData.objects.length; o < ol; o ++)
			{
				obj = _renderData.objects[o].obj;

				_modelMatrix = obj.matrixWorld;

				_vertexCount = 0;

				if (obj is Mesh)
				{
					geometry = obj.geometry;

					vertices = geometry.vertices;
					faces = geometry.faces;
					faceVertexUvs = geometry.faceVertexUvs;

					_normalMatrix.getInverse(_modelMatrix);
					_normalMatrix.transpose();

					isFaceMaterial = obj.material is MeshFaceMaterial;
					objectMaterials = isFaceMaterial == true ? obj.material : null;

					for (v = 0, vl = vertices.length; v < vl; v ++)
					{
						_vertex = getNextVertexInPool();

						_vertex.positionWorld.copy(vertices[v]).applyMatrix4(_modelMatrix);
						_vertex.positionScreen.copy(_vertex.positionWorld).applyMatrix4(_viewProjectionMatrix);

						_vertex.positionScreen.x /= _vertex.positionScreen.w;
						_vertex.positionScreen.y /= _vertex.positionScreen.w;
						_vertex.positionScreen.z /= _vertex.positionScreen.w;

						_vertex.visible = ! (_vertex.positionScreen.x < -1 || _vertex.positionScreen.x > 1 ||
						                     _vertex.positionScreen.y < -1 || _vertex.positionScreen.y > 1 ||
						                     _vertex.positionScreen.z < -1 || _vertex.positionScreen.z > 1);
					}

					for (f = 0, fl = faces.length; f < fl; f ++)
					{
						face = faces[f];

						var material = isFaceMaterial == true
						               	? objectMaterials.materials[face.materialIndex]
						               	: obj.material;

						if (material == null)
						{
							continue;
						}

						var side = material.side;

						if (face is Face3)
						{
							v1 = _vertexPool[face.a];
							v2 = _vertexPool[face.b];
							v3 = _vertexPool[face.c];

							_points3[0] = v1.positionScreen;
							_points3[1] = v2.positionScreen;
							_points3[2] = v3.positionScreen;

							if (v1.visible == true || v2.visible == true || v3.visible == true ||
							    _clipBox.isIntersectionBox(_boundingBox.setFromPoints(_points3)))
							{
								visible = ((v3.positionScreen.x - v1.positionScreen.x) * (v2.positionScreen.y - v1.positionScreen.y) -
								           (v3.positionScreen.y - v1.positionScreen.y) * (v2.positionScreen.x - v1.positionScreen.x)) < 0;

								if (side == THREE.DoubleSide || visible == (side == THREE.FrontSide))
								{
									_face = getNextFace3InPool();

									_face.v1.copy(v1);
									_face.v2.copy(v2);
									_face.v3.copy(v3);
								}
								else
								{
									continue;
								}
							}
							else
							{
								continue;
							}
						}
						else if (face is Face4)
						{
							v1 = _vertexPool[face.a];
							v2 = _vertexPool[face.b];
							v3 = _vertexPool[face.c];
							v4 = _vertexPool[face.d];

							_points4[0] = v1.positionScreen;
							_points4[1] = v2.positionScreen;
							_points4[2] = v3.positionScreen;
							_points4[3] = v4.positionScreen;

							if (v1.visible == true || v2.visible == true || v3.visible == true || v4.visible == true ||
							    _clipBox.isIntersectionBox(_boundingBox.setFromPoints(_points4)))
							{
								visible = (v4.positionScreen.x - v1.positionScreen.x) * (v2.positionScreen.y - v1.positionScreen.y) -
								          (v4.positionScreen.y - v1.positionScreen.y) * (v2.positionScreen.x - v1.positionScreen.x) < 0 ||
								          (v2.positionScreen.x - v3.positionScreen.x) * (v4.positionScreen.y - v3.positionScreen.y) -
								          (v2.positionScreen.y - v3.positionScreen.y) * (v4.positionScreen.x - v3.positionScreen.x) < 0;

								if (side == THREE.DoubleSide || visible == (side == THREE.FrontSide))
								{
									_face = getNextFace4InPool();

									_face.v1.copy(v1);
									_face.v2.copy(v2);
									_face.v3.copy(v3);
									_face.v4.copy(v4);
								}
								else
								{
									continue;
								}
							}
							else
							{
								continue;
							}
						}

						_face.normalModel.copy(face.normal);

						if (visible == false && (side == THREE.BackSide || side == THREE.DoubleSide))
						{
							_face.normalModel.negate();
						}

						_face.normalModel.applyMatrix3(_normalMatrix).normalize();

						_face.normalModelView.copy(_face.normalModel).applyMatrix3(_normalViewMatrix);

						_face.centroidModel.copy(face.centroid).applyMatrix4(_modelMatrix);

						faceVertexNormals = face.vertexNormals;

						for (n = 0, nl = faceVertexNormals.length; n < nl; n ++)
						{
							var normalModel = _face.vertexNormalsModel[n];
							normalModel.copy(faceVertexNormals[n]);

							if (visible == false && (side == THREE.BackSide || side == THREE.DoubleSide))
							{
								normalModel.negate();
							}

							normalModel.applyMatrix3(_normalMatrix).normalize();

							var normalModelView = _face.vertexNormalsModelView[n];
							normalModelView.copy(normalModel).applyMatrix3(_normalViewMatrix);
						}

						_face.vertexNormalsLength = faceVertexNormals.length;

						for (c = 0, cl = faceVertexUvs.length; c < cl; c ++)
						{
							uvs = faceVertexUvs[c][f];

							if (uvs == null)
							{
								continue;
							}

							for (u = 0, ul = uvs.length; u < ul; u ++)
							{
								_face.uvs[c][u] = uvs[u];
							}
						}

						_face.color = face.color;
						_face.material = material;

						_centroid.copy(_face.centroidModel).applyProjection(_viewProjectionMatrix);

						_face.z = _centroid.z;

						_renderData.elements.push(_face);
					}
				}
				else if (obj is Line)
				{
					_modelViewProjectionMatrix.multiplyMatrices(_viewProjectionMatrix, _modelMatrix);

					vertices = obj.geometry.vertices;

					v1 = getNextVertexInPool();
					v1.positionScreen.copy(vertices[0]).applyMatrix4(_modelViewProjectionMatrix);

					// Handle LineStrip and LinePieces
					var step = obj.type == THREE.LinePieces ? 2 : 1;

					for (v = 1, vl = vertices.length; v < vl; v ++)
					{
						v1 = getNextVertexInPool();
						v1.positionScreen.copy(vertices[v]).applyMatrix4(_modelViewProjectionMatrix);

						if ((v + 1) % step > 0)
						{
							continue;
						}

						v2 = _vertexPool[_vertexCount - 2];

						_clippedVertex1PositionScreen.copy(v1.positionScreen);
						_clippedVertex2PositionScreen.copy(v2.positionScreen);

						if (clipLine(_clippedVertex1PositionScreen, _clippedVertex2PositionScreen) == true)
						{
							// Perform the perspective divide
							_clippedVertex1PositionScreen.multiplyScalar(1 / _clippedVertex1PositionScreen.w);
							_clippedVertex2PositionScreen.multiplyScalar(1 / _clippedVertex2PositionScreen.w);

							_line = getNextLineInPool();
							_line.v1.positionScreen.copy(_clippedVertex1PositionScreen);
							_line.v2.positionScreen.copy(_clippedVertex2PositionScreen);

							_line.z = Math.max(_clippedVertex1PositionScreen.z, _clippedVertex2PositionScreen.z);

							_line.material = obj.material;

							_renderData.elements.push(_line);
						}
					}
				}
			}

			for (o = 0, ol = _renderData.sprites.length; o < ol; o++)
			{
				obj = _renderData.sprites[o].obj;

				_modelMatrix = obj.matrixWorld;

				if (obj is Particle)
				{
					_vector4.set(_modelMatrix.elements[12], _modelMatrix.elements[13], _modelMatrix.elements[14], 1);
					_vector4.applyMatrix4(_viewProjectionMatrix);

					_vector4.z /= _vector4.w;

					if (_vector4.z > 0 && _vector4.z < 1)
					{
						_particle = getNextParticleInPool();
						_particle.obj = obj;
						_particle.x = _vector4.x / _vector4.w;
						_particle.y = _vector4.y / _vector4.w;
						_particle.z = _vector4.z;

						_particle.rotation = obj.rotation.z;

						_particle.scale.x = obj.scale.x * Math.abs(_particle.x - (_vector4.x + camera.projectionMatrix.elements[0]) / (_vector4.w + camera.projectionMatrix.elements[12]));
						_particle.scale.y = obj.scale.y * Math.abs(_particle.y - (_vector4.y + camera.projectionMatrix.elements[5]) / (_vector4.w + camera.projectionMatrix.elements[13]));

						_particle.material = obj.material;

						_renderData.elements.push(_particle);
					}
				}
			}

			if (sortElements == true)
			{
				((JSArray)_renderData.elements).sort(painterSort);
			}

			return _renderData;
		}

		private dynamic projectGraph(dynamic root, dynamic sortObjects)
		{
			_objectCount = 0;

			_renderData.objects.length = 0;
			_renderData.sprites.length = 0;
			_renderData.lights.length = 0;

			projectObject(root);

			if (sortObjects == true)
			{
				((JSArray)_renderData.objects).sort(painterSort);
			}

			return _renderData;
		}

		private void projectObject(dynamic parent)
		{
			for (int c = 0, cl = parent.children.length; c < cl; c++)
			{
				var obj = parent.children[c];

				if (obj.visible == false)
				{
					continue;
				}

				if (obj is Light)
				{
					_renderData.lights.push(obj);
				}
				else if (obj is Mesh || obj is Line)
				{
					if (obj.frustumCulled == false || _frustum.intersectsObject(obj) == true)
					{
						_object = getNextObjectInPool();
						_object.obj = obj;

						if (obj.renderDepth != null)
						{
							_object.z = obj.renderDepth;
						}
						else
						{
							_vector3.copy(obj.matrixWorld.getPosition());
							_vector3.applyProjection(_viewProjectionMatrix);
							_object.z = _vector3.z;
						}

						_renderData.objects.push(_object);
					}
				}
				else if (obj is Sprite || obj is Particle)
				{
					_object = getNextObjectInPool();
					_object.obj = obj;

					// TODO: Find an elegant and performant solution and remove this dupe code.

					if (obj.renderDepth != null)
					{
						_object.z = obj.renderDepth;
					}
					else
					{
						_vector3.copy(obj.matrixWorld.getPosition());
						_vector3.applyProjection(_viewProjectionMatrix);
						_object.z = _vector3.z;
					}

					_renderData.sprites.push(_object);
				}
				else
				{
					_object = getNextObjectInPool();
					_object.obj = obj;

					if (obj.renderDepth != null)
					{
						_object.z = obj.renderDepth;
					}
					else
					{
						_vector3.copy(obj.matrixWorld.getPosition());
						_vector3.applyProjection(_viewProjectionMatrix);
						_object.z = _vector3.z;
					}

					_renderData.objects.push(_object);
				}

				projectObject(obj);
			}
		}

		private RenderableObject getNextObjectInPool()
		{
			if (_objectCount == _objectPoolLength)
			{
				var obj = new RenderableObject();
				_objectPool.push(obj);
				_objectPoolLength ++;
				_objectCount ++;
				return obj;
			}

			return _objectPool[_objectCount ++];
		}

		private RenderableVertex getNextVertexInPool()
		{
			if (_vertexCount == _vertexPoolLength)
			{
				var vertex = new RenderableVertex();
				_vertexPool.push(vertex);
				_vertexPoolLength ++;
				_vertexCount ++;
				return vertex;
			}

			return _vertexPool[_vertexCount ++];
		}

		private RenderableFace3 getNextFace3InPool()
		{
			if (_face3Count == _face3PoolLength)
			{
				var face = new RenderableFace3();
				_face3Pool.push(face);
				_face3PoolLength ++;
				_face3Count ++;
				return face;
			}

			return _face3Pool[_face3Count ++];
		}

		private RenderableFace4 getNextFace4InPool()
		{
			if (_face4Count == _face4PoolLength)
			{
				var face = new RenderableFace4();
				_face4Pool.push(face);
				_face4PoolLength ++;
				_face4Count ++;
				return face;
			}

			return _face4Pool[_face4Count ++];
		}

		private RenderableLine getNextLineInPool()
		{
			if (_lineCount == _linePoolLength)
			{
				var line = new RenderableLine();
				_linePool.push(line);
				_linePoolLength ++;
				_lineCount++;
				return line;
			}

			return _linePool[_lineCount ++];
		}

		private RenderableParticle getNextParticleInPool()
		{
			if (_particleCount == _particlePoolLength)
			{
				var particle = new RenderableParticle();
				_particlePool.push(particle);
				_particlePoolLength ++;
				_particleCount++;
				return particle;
			}

			return _particlePool[_particleCount ++];
		}

		private int painterSort(dynamic a, dynamic b)
		{
			return (int)(b.z - a.z);
		}

		private bool clipLine(dynamic s1, dynamic s2)
		{
			dynamic alpha1 = 0, alpha2 = 1,
			        bc1near = s1.z + s1.w,
			        bc2near = s2.z + s2.w,
			        bc1far = - s1.z + s1.w,
			        bc2far = - s2.z + s2.w;

			if (bc1near >= 0 && bc2near >= 0 && bc1far >= 0 && bc2far >= 0)
			{
				// Both vertices lie entirely within all clip planes.
				return true;
			}
			else if ((bc1near < 0 && bc2near < 0) || (bc1far < 0 && bc2far < 0))
			{
				// Both vertices lie entirely outside one of the clip planes.
				return false;
			}
			else
			{
				// The line segment spans at least one clip plane.

				if (bc1near < 0)
				{
					// v1 lies outside the near plane, v2 inside
					alpha1 = Math.max(alpha1, bc1near / (bc1near - bc2near));
				}
				else if (bc2near < 0)
				{
					// v2 lies outside the near plane, v1 inside
					alpha2 = Math.min(alpha2, bc1near / (bc1near - bc2near));
				}

				if (bc1far < 0)
				{
					// v1 lies outside the far plane, v2 inside
					alpha1 = Math.max(alpha1, bc1far / (bc1far - bc2far));
				}
				else if (bc2far < 0)
				{
					// v2 lies outside the far plane, v2 inside
					alpha2 = Math.min(alpha2, bc1far / (bc1far - bc2far));
				}

				if (alpha2 < alpha1)
				{
					// The line segment spans two boundaries, but is outside both of them.
					// (This can't happen when we're only clipping against just near/far but good
					//  to leave the check here for future usage if other clip planes are added.)
					return false;
				}
				else
				{
					// Update the s1 and s2 vertices to match the clipped line segment.
					s1.lerp(s2, alpha1);
					s2.lerp(s1, 1 - alpha2);

					return true;
				}
			}
		}
	}
}
