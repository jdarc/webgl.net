using System;
using System.Collections;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class NullUniformLocation : BaseTest
    {
        [Test(Description = "Tests calling the various uniform[Matrix]* APIs with a null uniform location")]
        public void ShouldDoMagic()
        {
            var gl = wtu.create3DContext(Canvas);
            var program = wtu.loadStandardProgram(gl);

            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeUndefined(() =>
                                  {
                                      gl.useProgram(program);
                                      return null;
                                  }, null);

            Func<string, dynamic> callUniformFunction =
                name =>
                {
                    var isArrayVariant = (name[name.Length - 1] == 'v');
                    var isMatrix = (name.IndexOf("Matrix") != -1);
                    var sizeIndex = (isArrayVariant ? name.Length - 3 : name.Length - 2);
                    var size = int.Parse(name.Substring(sizeIndex, 1));
                    // Initialize argument list with null uniform location
                    var args = new ArrayList {null};
                    var method = wtu.getMethod(gl, name);
                    if (isArrayVariant)
                    {
                        // Call variant which takes values as array
                        if (isMatrix)
                        {
                            size = size * size;
                            args.Add(false);
                        }
                        var parameterInfos = method.GetParameters();
                        var parameterType = parameterInfos[parameterInfos.Length - 1].ParameterType;
                        dynamic array = null;
                        if (parameterType.Name.Equals("Int32Array"))
                        {
                            array = new Int32Array(size);
                        }
                        else if (parameterType.Name.Equals("Float32Array"))
                        {
                            array = new Float32Array(size);
                        }
                        else if (parameterType.Name.Equals("Int32[]"))
                        {
                            array = new int[size];
                        }
                        else if (parameterType.Name.Equals("Float32[]"))
                        {
                            array = new float[size];
                        }
                        for (var i = 0; i < size; i++)
                        {
                            array[i] = i;
                        }
                        args.Add((object)array);
                    }
                    else
                    {
                        // Call variant which takes values as parameters
                        for (var i = 0; i < size; i++)
                        {
                            args.Add(i);
                        }
                    }
                    return method.Invoke(gl, args.ToArray());
                };

            var funcs = new[]
                        {
                            "uniform1f", "uniform1fv", "uniform1i", "uniform1iv",
                            "uniform2f", "uniform2fv", "uniform2i", "uniform2iv",
                            "uniform3f", "uniform3fv", "uniform3i", "uniform3iv",
                            "uniform4f", "uniform4fv", "uniform4i", "uniform4iv",
                            "uniformMatrix2fv", "uniformMatrix3fv", "uniformMatrix4fv"
                        };
            foreach (var t in funcs)
            {
                var localT = t;
                wtu.shouldBeUndefined(() => callUniformFunction(localT));
                wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            }
        }
    }
}