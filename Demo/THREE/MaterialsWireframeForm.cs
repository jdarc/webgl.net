using System;
using System.Windows.Forms;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class MaterialsWireframeForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private int mouseX;
        private int mouseY;
        private readonly Mesh meshLines;
        private readonly Mesh meshQuads;
        private readonly Mesh meshTris;
        private readonly Mesh meshMixed;

        public MaterialsWireframeForm()
        {
            camera = new PerspectiveCamera(40, aspectRatio, 1, 2000);
            camera.position.z = 800;

            scene = new Scene();

            var size = 150.0;

            var geometryLines = new CubeGeometry(size, size, size);
            var geometryQuads = new CubeGeometry(size, size, size);
            var geometryTris = new CubeGeometry(size, size, size);

            GeometryUtils.triangulateQuads(geometryTris);

            // wireframe using gl.LINES

            var materialLines = new MeshBasicMaterial(JSObject.create(new {wireframe = true}));

            meshLines = new Mesh(geometryLines, materialLines);
            meshLines.position.x = 0;
            scene.add(meshLines);

            //

            const string vertexShader = @"
attribute vec4 center;
varying vec4 vCenter;

void main() {

	vCenter = center;
	gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

}";

            const string fragmentShader =
                @"
#extension GL_OES_standard_derivatives : enable

varying vec4 vCenter;

float edgeFactorTri() {

	vec3 d = fwidth( vCenter.xyz );
	vec3 a3 = smoothstep( vec3( 0.0 ), d * 1.5, vCenter.xyz );
	return min( min( a3.x, a3.y ), a3.z );

}

float edgeFactorQuad1() {

	vec2 d = fwidth( vCenter.xy );
	vec2 a2 = smoothstep( vec2( 0.0 ), d * 1.5, vCenter.xy );

	return min( a2.x, a2.y );

}

float edgeFactorQuad2() {

	vec2 d = fwidth( 1.0 - vCenter.xy );
	vec2 a2 = smoothstep( vec2( 0.0 ), d * 1.5, 1.0 - vCenter.xy );

	return min( a2.x, a2.y );
}

void main() {

	if ( vCenter.w == 0.0 ) {

		gl_FragColor.rgb = mix( vec3( 1.0 ), vec3( 0.2 ), edgeFactorTri() );

	} else {

		gl_FragColor.rgb = mix( vec3( 1.0 ), vec3( 0.2 ), min( edgeFactorQuad1(), edgeFactorQuad2() ) );

	}

	gl_FragColor.a = 1.0;

}";

            // wireframe using gl.TRIANGLES (interpreted as quads)

            var attributesQuads = JSObject.create(new {center = new {type = "v4", boundTo = "faceVertices", value = new JSArray()}});
            var valuesQuads = attributesQuads.center.value;

            setupAttributes(geometryQuads, valuesQuads);

            var materialQuads = new ShaderMaterial(JSObject.create((dynamic)new
                                                                          {
                                                                              uniforms = new JSObject(),
                                                                              attributes = attributesQuads,
                                                                              vertexShader,
                                                                              fragmentShader
                                                                          }));

            meshQuads = new Mesh(geometryQuads, materialQuads);
            meshQuads.position.x = 300;
            scene.add(meshQuads);

            // wireframe using gl.TRIANGLES (interpreted as triangles)

            var attributesTris = JSObject.create(new {center = new {type = "v4", boundTo = "faceVertices", value = new JSArray()}});
            var valuesTris = attributesTris.center.value;

            setupAttributes(geometryTris, valuesTris);

            var materialTris = new ShaderMaterial(JSObject.create((dynamic)new
                                                                         {
                                                                             uniforms = new JSObject(),
                                                                             attributes = attributesTris,
                                                                             vertexShader,
                                                                             fragmentShader
                                                                         })
                );

            meshTris = new Mesh(geometryTris, materialTris);
            meshTris.position.x = -300;
            scene.add(meshTris);

            // wireframe using gl.TRIANGLES (mixed triangles and quads)

            var mixedGeometry = new SphereGeometry(size / 2.0, 32, 16);

            var attributesMixed = JSObject.create(new {center = new {type = "v4", boundTo = "faceVertices", value = new JSArray()}});
            var valuesMixed = attributesMixed.center.value;

            setupAttributes(mixedGeometry, valuesMixed);

            var materialMixed = new ShaderMaterial(JSObject.create((dynamic)new
                                                                          {
                                                                              uniforms = new JSObject(),
                                                                              attributes = attributesMixed,
                                                                              vertexShader,
                                                                              fragmentShader
                                                                          }));

            meshMixed = new Mesh(mixedGeometry, materialMixed);
            meshMixed.position.x = 0;
            scene.add(meshMixed);

            // renderer

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
        }

        protected override void onWindowResize(EventArgs e)
        {
            if (camera != null)
            {
                camera.aspect = aspectRatio;
                camera.updateProjectionMatrix();

                renderer.setSize(ClientSize.Width, ClientSize.Height);
            }
        }

        protected override void onMouseClick(MouseEventArgs e)
        {
        }

        protected override void onMouseMove(MouseEventArgs e)
        {
            mouseX = (int)(e.X - windowHalf.X);
            mouseY = (int)(e.Y - windowHalf.Y);
        }

        private void setupAttributes(dynamic geometry, dynamic values)
        {
            for (var f = 0; f < geometry.faces.length; f ++)
            {
                var face = geometry.faces[f];

                if (face is Face3)
                {
                    values[f] = new JSArray(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0));
                }
                else
                {
                    values[f] = new JSArray(new Vector4(1, 0, 0, 1), new Vector4(1, 1, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 0, 1));
                }
            }
        }

        protected override void render()
        {
            meshLines.rotation.x += 0.005;
            meshLines.rotation.y += 0.01;

            meshQuads.rotation.x += 0.005;
            meshQuads.rotation.y += 0.01;

            meshTris.rotation.x += 0.005;
            meshTris.rotation.y += 0.01;

            if (meshMixed != null)
            {
                meshMixed.rotation.x += 0.005;
                meshMixed.rotation.y += 0.01;
            }

            renderer.render(scene, camera);
        }
    }
}