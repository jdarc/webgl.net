using WebGL;

namespace THREE
{
	public class ShapeGeometry : Geometry
	{
		public dynamic shapebb;

		public ShapeGeometry(dynamic shapes, dynamic options = null)
		{
			if (shapes is JSArray == false)
			{
				shapes = new JSArray(shapes);
			}

			shapebb = shapes[shapes.length - 1].getBoundingBox();

			this.addShapeList(shapes, options);

			computeCentroids();
			computeFaceNormals();
		}

		/**
		 * Add an array of shapes to THREE.ShapeGeometry.
 		 */

		public ShapeGeometry addShapeList(dynamic shapes, dynamic options)
		{
			int l = shapes.length;
			for (var i = 0; i < l; i++)
			{
				this.addShape(shapes[i], options);
			}

			return this;
		}

		/**
		 * Adds a shape to THREE.ShapeGeometry, based on THREE.ExtrudeGeometry.
		 */

		public void addShape(dynamic shape, dynamic options)
		{
			if (options == null)
			{
				options = new JSObject();
			}
			var curveSegments = options.curveSegments ?? 12;

			var material = options.material ?? 0;
			var uvgen = options.UVGenerator ?? new ExtrudeGeometry.WorldUVGenerator();

			int i;
			int l;
			dynamic hole;

			var shapesOffset = this.vertices.length;
			var shapePoints = shape.extractPoints(curveSegments);

			var vertices = shapePoints.shape;
			var holes = shapePoints.holes;

			var reverse = !Shape.Utils.isClockWise(vertices);

			if (reverse)
			{
				vertices = vertices.reverse();

				for (i = 0, l = holes.length; i < l; i++)
				{
					hole = holes[i];

					if (Shape.Utils.isClockWise(hole))
					{
						holes[i] = hole.reverse();
					}
				}
			}

			var faces = Shape.Utils.triangulateShape(vertices, holes);

			for (i = 0, l = holes.length; i < l; i++)
			{
				hole = holes[i];
				vertices = vertices.concat(hole);
			}

			dynamic vert, vlen = vertices.length;
			var flen = faces.length;

			for (i = 0; i < vlen; i++)
			{
				vert = vertices[i];

				this.vertices.push(new Vector3(vert.x, vert.y, 0));
			}

			for (i = 0; i < flen; i++)
			{
				var face = faces[i];

				var a = face[0] + shapesOffset;
				var b = face[1] + shapesOffset;
				var c = face[2] + shapesOffset;

				this.faces.push(new Face3(a, b, c, null, null, material));
				faceVertexUvs[0].push(uvgen.generateBottomUV(this, shape, options, a, b, c));
			}
		}
	}
}
