using System;
using WebGL;

namespace THREE
{
	public class Raycaster
	{
		public const double precision = 0.0001;

		public readonly Ray ray;
		public double near;
		public double far;

		private readonly Sphere sphere = new Sphere();
		private readonly Ray localRay = new Ray();
		private readonly Plane facePlane = new Plane();
		private Vector3 intersectPoint = new Vector3();
		private readonly Matrix4 inverseMatrix = new Matrix4();

		public Raycaster(Vector3 origin, Vector3 direction, double near = 0.0, double far = double.PositiveInfinity)
		{
			ray = new Ray(origin, direction);

			// normalized ray.direction required for accurate distance calculations
			if (ray.direction.length() > 0)
			{
				ray.direction.normalize();
			}

			this.near = near;
			this.far = far;
		}

		private int descSort(dynamic a, dynamic b)
		{
			return a.distance - b.distance;
		}

		private dynamic intersectObject(dynamic obj, dynamic raycaster, dynamic intersects)
		{
			if (obj is Particle)
			{
				var distance = raycaster.ray.distanceToPoint(obj.matrixWorld.getPosition());

				if (distance > obj.scale.x)
				{
					return intersects;
				}

				intersects.push(JSObject.create(new
				{
					distance,
					point = obj.position,
					face = (object)null, obj
				}));
			}
			else if (obj is Mesh)
			{
				// Checking boundingSphere distance to ray
				sphere.set(
				           obj.matrixWorld.getPosition(),
				           obj.geometry.boundingSphere.radius * obj.matrixWorld.getMaxScaleOnAxis());

				if (! raycaster.ray.isIntersectionSphere(sphere))
				{
					return intersects;
				}

				// Checking faces

				var geometry = obj.geometry;
				var vertices = geometry.vertices;

				var isFaceMaterial = obj.material is MeshFaceMaterial;
				var objectMaterials = isFaceMaterial ? obj.material.materials : null;

				var side = obj.material.side;

				var precision = raycaster.precision;

				obj.matrixRotationWorld.extractRotation(obj.matrixWorld);

				inverseMatrix.getInverse(obj.matrixWorld);

				localRay.copy(raycaster.ray).transform(inverseMatrix);

				for (int f = 0, fl = geometry.faces.length; f < fl; f ++)
				{
					var face = geometry.faces[f];

					var material = isFaceMaterial ? objectMaterials[face.materialIndex] : obj.material;

					if (material == null)
					{
						continue;
					}

					facePlane.setFromNormalAndCoplanarPoint(face.normal, vertices[face.a]);

					var planeDistance = localRay.distanceToPlane(facePlane);

					// bail if raycaster and plane are parallel
					if (Math.abs(planeDistance) < precision)
					{
						continue;
					}

					// if negative distance, then plane is behind raycaster
					if (planeDistance < 0)
					{
						continue;
					}

					// check if we hit the wrong side of a single sided face
					side = material.side;
					if (side != THREE.DoubleSide)
					{
						var planeSign = localRay.direction.dot(facePlane.normal);

						if (! (side == THREE.FrontSide ? planeSign < 0 : planeSign > 0))
						{
							continue;
						}
					}

					// this can be done using the planeDistance from localRay because localRay wasn't normalized, but ray was
					if (planeDistance < raycaster.near || planeDistance > raycaster.far)
					{
						continue;
					}

					intersectPoint = localRay.at(planeDistance, intersectPoint); // passing in intersectPoint avoids a copy

					if (face is Face3)
					{
						var a = vertices[face.a];
						var b = vertices[face.b];
						var c = vertices[face.c];

						if (! Triangle.containsPoint(intersectPoint, a, b, c))
						{
							continue;
						}
					}
					else if (face is Face4)
					{
						var a = vertices[face.a];
						var b = vertices[face.b];
						var c = vertices[face.c];
						var d = vertices[face.d];

						if ((! Triangle.containsPoint(intersectPoint, a, b, d)) &&
						    (! Triangle.containsPoint(intersectPoint, b, c, d)))
						{
							continue;
						}
					}
					else
					{
						// This is added because if we call out of this if/else group when none of the cases
						//    match it will add a point to the intersection list erroneously.
                        throw new ApplicationException("face type not supported");
					}

					intersects.push(JSObject.create(new
					{
						distance = planeDistance, // this works because the original ray was normalized, and the transformed localRay wasn't
						point = raycaster.ray.at(planeDistance), face,
						faceIndex = f, obj
					}));
				}
			}

			return null;
		}

		private void intersectDescendants(dynamic obj, dynamic raycaster, dynamic intersects)
		{
			var descendants = obj.getDescendants();

			for (int i = 0, l = descendants.length; i < l; i ++)
			{
				intersectObject(descendants[i], raycaster, intersects);
			}
		}

		public virtual void set(dynamic origin, dynamic direction)
		{
			ray.set(origin, direction);

			// normalized ray.direction required for accurate distance calculations
			if (ray.direction.length() > 0)
			{
				ray.direction.normalize();
			}
		}

		public virtual JSArray intersectObject(dynamic obj, dynamic recursive)
		{
			var intersects = new JSArray();

			if (recursive == true)
			{
				intersectDescendants(obj, this, intersects);
			}

			intersectObject(obj, this, intersects);

			intersects.sort(descSort);

			return intersects;
		}

		public virtual JSArray intersectObjects(dynamic objects, dynamic recursive)
		{
			var intersects = new JSArray();

			for (int i = 0, l = objects.length; i < l; i ++)
			{
				intersectObject(objects[i], this, intersects);

				if (recursive == true)
				{
					intersectDescendants(objects[i], this, intersects);
				}
			}

			intersects.sort(descSort);

			return intersects;
		}
	}
}
