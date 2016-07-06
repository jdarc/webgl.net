using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class InvalidUTF16 : BaseTest
    {
        [Test(Description = "This test verifies that the internal conversion from UTF16 to UTF8 is robust to invalid inputs. Any DOM entry point which converts an incoming string to UTF8 could be used for this test.")]
        public void ShouldDoMagic()
        {
            var array = new JSArray();
            array.push(((char)0x48).ToString()); // H
            array.push(((char)0x69).ToString()); // i
            array.push(((char)0xd87e).ToString()); // Bogus
            var @string = array.join("");

            // In order to make this test not depend on WebGL, the following were
            // attempted:
            //  - Send a string to console.log
            //  - Submit a mailto: form containing a text input with the bogus
            //    string
            // The first code path does not perform a utf8 conversion of the
            // incoming string unless Console::shouldPrintExceptions() returns
            // true. The second seems to sanitize the form's input before
            // converting it to a UTF8 string.

            var gl = wtu.create3DContext(Canvas);
            var program = gl.createProgram();
            gl.bindAttribLocation(program, 0, @string);
            wtu.testPassed("bindAttribLocation with invalid UTF-16 did not crash");
        }
    }
}