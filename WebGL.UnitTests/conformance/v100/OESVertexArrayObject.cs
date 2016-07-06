using System;
using System.Collections;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class OESVertexArrayObject : BaseTest
    {
        [Test(Description = "This test verifies the functionality of the OES_vertex_array_object extension, if it is available.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");

            var gl = wtu.create3DContext(Canvas);
            dynamic ext = null;

            Action<bool> runSupportedTest =
                extensionEnabled =>
                {
                    var supported = gl.getSupportedExtensions();
                    if (Array.IndexOf(supported, "OES_vertex_array_object") >= 0)
                    {
                        if (extensionEnabled)
                        {
                            wtu.testPassed("OES_vertex_array_object listed as supported and getExtension succeeded");
                        }
                        else
                        {
                            wtu.testFailed("OES_vertex_array_object listed as supported but getExtension failed");
                        }
                    }
                    else
                    {
                        if (extensionEnabled)
                        {
                            wtu.testFailed("OES_vertex_array_object not listed as supported but getExtension succeeded");
                        }
                        else
                        {
                            wtu.testPassed("OES_vertex_array_object not listed as supported and getExtension failed -- this is legal");
                        }
                    }
                };

            Action runBindingTestDisabled =
                () =>
                {
                    wtu.debug("Testing binding enum with extension disabled");

                    // Use the constant directly as we don't have the extension
                    const int VERTEX_ARRAY_BINDING_OES = 0x85B5;

                    gl.getParameter(VERTEX_ARRAY_BINDING_OES);
                    wtu.glErrorShouldBe(gl, gl.INVALID_ENUM, "VERTEX_ARRAY_BINDING_OES should not be queryable if extension is disabled");
                };

            Action runBindingTestEnabled =
                () =>
                {
                    wtu.debug("Testing binding enum with extension enabled");

                    wtu.shouldBe(() => ext.VERTEX_ARRAY_BINDING_OES, 0x85B5);

                    gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES);
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "VERTEX_ARRAY_BINDING_OES query should succeed if extension is enable");

                    // Default value is null
                    if (gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES) == null)
                    {
                        wtu.testPassed("Default value of VERTEX_ARRAY_BINDING_OES is null");
                    }
                    else
                    {
                        wtu.testFailed("Default value of VERTEX_ARRAY_BINDING_OES is not null");
                    }

                    wtu.debug("Testing binding a VAO");
                    var vao0 = ext.createVertexArrayOES();
                    var vao1 = ext.createVertexArrayOES();
                    wtu.shouldBeNull(() => gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES));
                    ext.bindVertexArrayOES(vao0);
                    if (gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES) == vao0)
                    {
                        wtu.testPassed("gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES) is expected VAO");
                    }
                    else
                    {
                        wtu.testFailed("gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES) is not expected VAO");
                    }
                    ext.bindVertexArrayOES(vao1);
                    if (gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES) == vao1)
                    {
                        wtu.testPassed("gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES) is expected VAO");
                    }
                    else
                    {
                        wtu.testFailed("gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES) is not expected VAO");
                    }
                    ext.bindVertexArrayOES(null);
                    wtu.shouldBeNull(() => gl.getParameter(ext.VERTEX_ARRAY_BINDING_OES));
                    ext.deleteVertexArrayOES(vao0);
                    ext.deleteVertexArrayOES(vao1);
                };

            Action runObjectTest =
                () =>
                {
                    wtu.debug("Testing object creation");

                    var vao = ext.createVertexArrayOES();
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "createVertexArrayOES should not set an error");
                    wtu.shouldBeNonNull(() => vao);

                    // Expect false if never bound
                    wtu.shouldBeFalse(() => ext.isVertexArrayOES(vao));
                    ext.bindVertexArrayOES(vao);
                    wtu.shouldBeTrue(() => ext.isVertexArrayOES(vao));
                    ext.bindVertexArrayOES(null);
                    wtu.shouldBeTrue(() => ext.isVertexArrayOES(vao));

                    wtu.shouldBeFalse(() => ext.isVertexArrayOES());
                    wtu.shouldBeFalse(() => ext.isVertexArrayOES(null));

                    ext.deleteVertexArrayOES(vao);
                    vao = null;
                };

            Action runAttributeTests =
                () =>
                {
                    wtu.debug("Testing attributes work across bindings");

                    var states = new ArrayList();

                    var attrCount = gl.getParameter(gl.MAX_VERTEX_ATTRIBS);
                    for (var n = 0; n < attrCount; n++)
                    {
                        gl.bindBuffer(gl.ARRAY_BUFFER, null);
                        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null);

                        dynamic state = new {};
                        states.Add(state);

                        var vao = state.vao = ext.createVertexArrayOES();
                        ext.bindVertexArrayOES(vao);

                        if (n % 2 == 0)
                        {
                            gl.enableVertexAttribArray((uint)n);
                        }
                        else
                        {
                            gl.disableVertexAttribArray((uint)n);
                        }

                        if (n % 2 == 0)
                        {
                            var buffer = state.buffer = gl.createBuffer();
                            gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
                            gl.bufferData(gl.ARRAY_BUFFER, 1024, gl.STATIC_DRAW);

                            gl.vertexAttribPointer((uint)n, 1 + n % 4, gl.FLOAT, true, n * 4, n * 4);
                        }

                        if (n % 2 == 0)
                        {
                            var elbuffer = state.elbuffer = gl.createBuffer();
                            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, elbuffer);
                            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, 1024, gl.STATIC_DRAW);
                        }

                        ext.bindVertexArrayOES(null);
                    }

                    var anyMismatch = false;
                    for (var n = 0; n < attrCount; n++)
                    {
                        dynamic state = states[n];

                        ext.bindVertexArrayOES(state.vao);

                        var isEnabled = gl.getVertexAttrib((uint)n, gl.VERTEX_ATTRIB_ARRAY_ENABLED);
                        if ((n % 2 == 1) || isEnabled)
                        {
                            // Valid
                        }
                        else
                        {
                            wtu.testFailed("VERTEX_ATTRIB_ARRAY_ENABLED not preserved");
                            anyMismatch = true;
                        }

                        var buffer = gl.getVertexAttrib((uint)n, gl.VERTEX_ATTRIB_ARRAY_BUFFER_BINDING);
                        if (n % 2 == 0)
                        {
                            if (buffer == state.buffer)
                            {
                                // Matched
                                if ((gl.getVertexAttrib((uint)n, gl.VERTEX_ATTRIB_ARRAY_SIZE) == 1 + n % 4) &&
                                    (gl.getVertexAttrib((uint)n, gl.VERTEX_ATTRIB_ARRAY_TYPE) == gl.FLOAT) &&
                                    (gl.getVertexAttrib((uint)n, gl.VERTEX_ATTRIB_ARRAY_NORMALIZED) == true) &&
                                    (gl.getVertexAttrib((uint)n, gl.VERTEX_ATTRIB_ARRAY_STRIDE) == n * 4) &&
                                    (gl.getVertexAttribOffset((uint)n, gl.VERTEX_ATTRIB_ARRAY_POINTER) == n * 4))
                                {
                                    // Matched
                                }
                                else
                                {
                                    wtu.testFailed("VERTEX_ATTRIB_ARRAY_* not preserved");
                                    anyMismatch = true;
                                }
                            }
                            else
                            {
                                wtu.testFailed("VERTEX_ATTRIB_ARRAY_BUFFER_BINDING not preserved");
                                anyMismatch = true;
                            }
                        }
                        else
                        {
                            // GL_CURRENT_VERTEX_ATTRIB is not preserved
                            if (buffer)
                            {
                                wtu.testFailed("VERTEX_ATTRIB_ARRAY_BUFFER_BINDING not preserved");
                                anyMismatch = true;
                            }
                        }

                        var elbuffer = gl.getParameter(gl.ELEMENT_ARRAY_BUFFER_BINDING);
                        if (n % 2 == 0)
                        {
                            if (elbuffer == state.elbuffer)
                            {
                                // Matched
                            }
                            else
                            {
                                wtu.testFailed("ELEMENT_ARRAY_BUFFER_BINDING not preserved");
                                anyMismatch = true;
                            }
                        }
                        else
                        {
                            if (elbuffer == null)
                            {
                                // Matched
                            }
                            else
                            {
                                wtu.testFailed("ELEMENT_ARRAY_BUFFER_BINDING not preserved");
                                anyMismatch = true;
                            }
                        }
                    }
                    ext.bindVertexArrayOES(null);
                    if (!anyMismatch)
                    {
                        wtu.testPassed("All attributes preserved across bindings");
                    }

                    for (var n = 0; n < attrCount; n++)
                    {
                        dynamic state = states[n];
                        ext.deleteVertexArrayOES(state.vao);
                    }
                };

            Action runAttributeValueTests =
                () =>
                {
                    wtu.debug("Testing that attribute values are not attached to bindings");

                    dynamic v = null;
                    var vao0 = ext.createVertexArrayOES();
                    var anyFailed = false;

                    ext.bindVertexArrayOES(null);
                    gl.vertexAttrib4f(0, 0, 1, 2, 3);

                    v = gl.getVertexAttrib(0, gl.CURRENT_VERTEX_ATTRIB);
                    if (!(v[0] == 0 && v[1] == 1 && v[2] == 2 && v[3] == 3))
                    {
                        wtu.testFailed("Vertex attrib value not round-tripped?");
                        anyFailed = true;
                    }

                    ext.bindVertexArrayOES(vao0);

                    v = gl.getVertexAttrib(0, gl.CURRENT_VERTEX_ATTRIB);
                    if (!(v[0] == 0 && v[1] == 1 && v[2] == 2 && v[3] == 3))
                    {
                        wtu.testFailed("Vertex attrib value reset across bindings");
                        anyFailed = true;
                    }

                    gl.vertexAttrib4f(0, 4, 5, 6, 7);
                    ext.bindVertexArrayOES(null);

                    v = gl.getVertexAttrib(0, gl.CURRENT_VERTEX_ATTRIB);
                    if (!(v[0] == 4 && v[1] == 5 && v[2] == 6 && v[3] == 7))
                    {
                        wtu.testFailed("Vertex attrib value bound to buffer");
                        anyFailed = true;
                    }

                    if (!anyFailed)
                    {
                        wtu.testPassed("Vertex attribute values are not attached to bindings");
                    }

                    ext.bindVertexArrayOES(null);
                    ext.deleteVertexArrayOES(vao0);
                };

            Action runDrawTests =
                () =>
                {
                    wtu.debug("Testing draws with various VAO bindings");

                    Canvas.setWidth(50);
                    Canvas.setHeight(50);
                    gl.viewport(0, 0, Canvas.width, Canvas.height);

                    var vao0 = ext.createVertexArrayOES();
                    var vao1 = ext.createVertexArrayOES();

                    var program = wtu.setupSimpleTextureProgram(gl, 0, 1);

                    Action<float> setupQuad =
                        s =>
                        {
                            var opt_positionLocation = 0;
                            var opt_texcoordLocation = 1;
                            var vertexObject = gl.createBuffer();
                            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
                            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[]
                                                                            {
                                                                                1.0f * s, 1.0f * s, 0.0f,
                                                                                -1.0f * s, 1.0f * s, 0.0f,
                                                                                -1.0f * s, -1.0f * s, 0.0f,
                                                                                1.0f * s, 1.0f * s, 0.0f,
                                                                                -1.0f * s, -1.0f * s, 0.0f,
                                                                                1.0f * s, -1.0f * s, 0.0f
                                                                            }), gl.STATIC_DRAW);
                            gl.enableVertexAttribArray((uint)opt_positionLocation);
                            gl.vertexAttribPointer((uint)opt_positionLocation, 3, gl.FLOAT, false, 0, 0);

                            vertexObject = gl.createBuffer();
                            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
                            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[]
                                                                            {
                                                                                1.0f * s, 1.0f * s,
                                                                                0.0f * s, 1.0f * s,
                                                                                0.0f * s, 0.0f * s,
                                                                                1.0f * s, 1.0f * s,
                                                                                0.0f * s, 0.0f * s,
                                                                                1.0f * s, 0.0f * s
                                                                            }), gl.STATIC_DRAW);
                            gl.enableVertexAttribArray((uint)opt_texcoordLocation);
                            gl.vertexAttribPointer((uint)opt_texcoordLocation, 2, gl.FLOAT, false, 0, 0);
                        };

                    Func<dynamic, dynamic, dynamic> readLocation =
                        (x, y) =>
                        {
                            var pixels = new Uint8Array(1 * 1 * 4);
                            gl.readPixels(x, y, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, pixels);
                            return pixels;
                        };

                    Func<dynamic, dynamic, dynamic> testPixel =
                        (blackList, whiteList) =>
                        {
                            Func<dynamic, dynamic, bool> testList =
                                (list, expected) =>
                                {
                                    for (var n = 0; n < list.length; n++)
                                    {
                                        var l = list[n];
                                        var x = -Math.Floor(l * Canvas.width / 2f) + Canvas.width / 2f;
                                        var y = -Math.Floor(l * Canvas.height / 2f) + Canvas.height / 2f;
                                        var source = readLocation(x, y);
                                        if (Math.Abs(source[0] - expected) > 2)
                                        {
                                            return false;
                                        }
                                    }
                                    return true;
                                };
                            return testList(blackList, 0) && testList(whiteList, 255);
                        };

                    Action<dynamic, dynamic> verifyDraw =
                        (drawNumber, s) =>
                        {
                            wtu.drawQuad(gl);
                            var blackList = new JSArray();
                            var whiteList = new JSArray();
                            var points = new JSArray(0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f);
                            for (var n = 0; n < points.length; n++)
                            {
                                if (points[n] <= s)
                                {
                                    blackList.push(points[n]);
                                }
                                else
                                {
                                    whiteList.push(points[n]);
                                }
                            }
                            if (testPixel(blackList, whiteList))
                            {
                                wtu.testPassed("Draw " + drawNumber + " passed pixel test");
                            }
                            else
                            {
                                wtu.testFailed("Draw " + drawNumber + " failed pixel test");
                            }
                        };

                    // Setup all bindings
                    setupQuad(1);
                    ext.bindVertexArrayOES(vao0);
                    setupQuad(0.5f);
                    ext.bindVertexArrayOES(vao1);
                    setupQuad(0.25f);

                    // Verify drawing
                    ext.bindVertexArrayOES(null);
                    verifyDraw(0, 1);
                    ext.bindVertexArrayOES(vao0);
                    verifyDraw(1, 0.5);
                    ext.bindVertexArrayOES(vao1);
                    verifyDraw(2, 0.25);

                    ext.bindVertexArrayOES(null);
                    ext.deleteVertexArrayOES(vao0);
                    ext.deleteVertexArrayOES(vao1);
                };

            if (gl == null)
            {
                wtu.testFailed("WebGL context does not exist");
            }
            else
            {
                wtu.testPassed("WebGL context exists");

                // Run tests with extension disabled
                runBindingTestDisabled();

                // Query the extension and store globally so shouldBe can access it
                ext = gl.getExtension("OES_vertex_array_object");
                if (ext == null)
                {
                    wtu.testPassed("No OES_vertex_array_object support -- this is legal");

                    runSupportedTest(false);
                }
                else
                {
                    wtu.testPassed("Successfully enabled OES_vertex_array_object extension");

                    runSupportedTest(true);
                    runBindingTestEnabled();
                    runObjectTest();
                    runAttributeTests();
                    runAttributeValueTests();
                    runDrawTests();
                }
            }

            wtu.debug("");
        }
    }
}