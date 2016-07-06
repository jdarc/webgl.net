using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class ArrayUnitTests : BaseTest
    {
        [Test(Description = "Verifies the functionality of the new array-like objects in the TypedArray spec")]
        public void ShouldDoMagic()
        {
            var currentlyRunning = "";
            var allPassed = true;

            Action<string> running = str => { currentlyRunning = str; };

            Action<string> output = wtu.debug;

            Action pass = () => wtu.testPassed(currentlyRunning);

            Action<string> fail =
                str =>
                {
                    allPassed = false;
                    string exc;
                    if (str != null)
                    {
                        exc = currentlyRunning + ": " + str;
                    }
                    else
                    {
                        exc = currentlyRunning;
                    }
                    wtu.testFailed(exc);
                };

            Action<string, dynamic, dynamic> assertEq =
                (prefix, expected, val) =>
                {
                    if (expected != val)
                    {
                        var str = prefix + ": expected " + expected + ", got " + val;
                        throw new Exception(str);
                    }
                };

            Action<dynamic, dynamic> assert =
                (prefix, expected) =>
                {
                    if (expected == null)
                    {
                        var str = prefix + ": expected value / true";
                        throw new Exception(str);
                    }
                };

            Action printSummary =
                () =>
                {
                    if (allPassed)
                    {
                        wtu.debug("Test passed.");
                    }
                    else
                    {
                        wtu.debug("TEST FAILED");
                    }
                };

            //
            // Tests for unsigned array variants
            //
            Action<string, string> testSetAndGet10To1 =
                (type, name) =>
                {
                    running("test " + name + " SetAndGet10To1");
                    try
                    {
                        var array = createtype(type, 10);
                        for (var i = 0; i < 10; i++)
                        {
                            array[i] = 10 - i;
                        }
                        for (var i = 0; i < 10; i++)
                        {
                            assertEq("Element " + i, 10 - i, array[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<string, string> testConstructWithArrayOfUnsignedValues =
                (type, name) =>
                {
                    running("test " + name + " ConstructWithArrayOfUnsignedValues");
                    try
                    {
                        var array = createtype(type, new JSArray(10, 9, 8, 7, 6, 5, 4, 3, 2, 1));
                        assertEq("array.length", 10, array.length);
                        for (var i = 0; i < 10; i++)
                        {
                            assertEq("Element " + i, 10 - i, array[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<string, string> testConstructWithTypedArrayOfUnsignedValues =
                (type, name) =>
                {
                    running("test " + name + " ConstructWithTypedArrayOfUnsignedValues");
                    try
                    {
                        var tmp = createtype(type, new JSArray(10, 9, 8, 7, 6, 5, 4, 3, 2, 1));
                        var array = createtype(type, tmp);
                        assertEq("Array length", 10, array.length);
                        for (var i = 0; i < 10; i++)
                        {
                            assertEq("Element " + i, 10 - i, array[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            //
            // Tests for signed array variants
            //
            Action<string, string> testSetAndGetPos10ToNeg10 =
                (type, name) =>
                {
                    running("test " + name + " SetAndGetPos10ToNeg10");
                    try
                    {
                        var array = createtype(type, 21);
                        for (var i = 0; i < 21; i++)
                        {
                            array[i] = 10 - i;
                        }
                        for (var i = 0; i < 21; i++)
                        {
                            assertEq("Element " + i, 10 - i, array[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> testConstructWithArrayOfSignedValues =
                (type, name) =>
                {
                    running("test " + name + " ConstructWithArrayOfSignedValues");
                    try
                    {
                        var array = createtype(type, new JSArray(10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, -1, -2, -3, -4, -5, -6, -7, -8, -9, -10));
                        assertEq("Array length", 21, array.length);
                        for (var i = 0; i < 21; i++)
                        {
                            assertEq("Element " + i, 10 - i, array[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> testConstructWithTypedArrayOfSignedValues =
                (type, name) =>
                {
                    running("test " + name + " ConstructWithTypedArrayOfSignedValues");
                    try
                    {
                        var tmp = createtype(type, new JSArray(10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, -1, -2, -3, -4, -5, -6, -7, -8, -9, -10));
                        var array = createtype(type, tmp);
                        assertEq("Array length", 21, array.length);
                        for (var i = 0; i < 21; i++)
                        {
                            assertEq("Element " + i, 10 - i, array[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            //
            // Test cases for integral types.
            // Some JavaScript engines need separate copies of this code in order
            // to exercise all of their optimized code paths.
            //
            Action<string, string, bool> testIntegralArrayTruncationBehavior =
                (type, name, unsigned) =>
                {
                    running("test integral array truncation behavior for " + name);

                    JSArray sourceData;
                    JSArray expectedResults;

                    if (unsigned)
                    {
                        sourceData = new JSArray(0.6, 10.6f);
                        expectedResults = new JSArray(0, 10);
                    }
                    else
                    {
                        sourceData = new JSArray(0.6f, 10.6f, -0.6f, -10.6f);
                        expectedResults = new JSArray(0, 10, 0, -10);
                    }

                    var numIterations = 10;
                    var array = createtype(type, numIterations);

                    // The code block in each of the case statements below is identical, but some
                    // JavaScript engines need separate copies in order to exercise all of
                    // their optimized code paths.

                    try
                    {
                        switch (type)
                        {
                            case "Int8Array":
                                for (var ii = 0; ii < sourceData.length; ++ii)
                                {
                                    for (var jj = 0; jj < numIterations; ++jj)
                                    {
                                        array[jj] = sourceData[ii];
                                        assertEq("Storing " + sourceData[ii], expectedResults[ii], array[jj]);
                                    }
                                }
                                break;
                            case "Int16Array":
                                for (var ii = 0; ii < sourceData.length; ++ii)
                                {
                                    for (var jj = 0; jj < numIterations; ++jj)
                                    {
                                        array[jj] = sourceData[ii];
                                        assertEq("Storing " + sourceData[ii], expectedResults[ii], array[jj]);
                                    }
                                }
                                break;
                            case "Int32Array":
                                for (var ii = 0; ii < sourceData.length; ++ii)
                                {
                                    for (var jj = 0; jj < numIterations; ++jj)
                                    {
                                        array[jj] = sourceData[ii];
                                        assertEq("Storing " + sourceData[ii], expectedResults[ii], array[jj]);
                                    }
                                }
                                break;
                            case "Uint8Array":
                                for (var ii = 0; ii < sourceData.length; ++ii)
                                {
                                    for (var jj = 0; jj < numIterations; ++jj)
                                    {
                                        array[jj] = sourceData[ii];
                                        assertEq("Storing " + sourceData[ii], expectedResults[ii], array[jj]);
                                    }
                                }
                                break;
                            case "Uint16Array":
                                for (var ii = 0; ii < sourceData.length; ++ii)
                                {
                                    for (var jj = 0; jj < numIterations; ++jj)
                                    {
                                        array[jj] = sourceData[ii];
                                        assertEq("Storing " + sourceData[ii], expectedResults[ii], array[jj]);
                                    }
                                }
                                break;
                            case "Uint32Array":
                                for (var ii = 0; ii < sourceData.length; ++ii)
                                {
                                    for (var jj = 0; jj < numIterations; ++jj)
                                    {
                                        array[jj] = sourceData[ii];
                                        assertEq("Storing " + sourceData[ii], expectedResults[ii], array[jj]);
                                    }
                                }
                                break;
                            default:
                                fail("Unhandled type");
                                break;
                        }

                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            //
            // Test cases for both signed and unsigned types
            //
            Action<dynamic, dynamic> testGetWithOutOfRangeIndices =
                (type, name) =>
                {
                    wtu.debug("Testing " + name + " GetWithOutOfRangeIndices");
                    // See below for declaration of this global variable
                    var array = createtype(type, new JSArray(2, 3));
                    wtu.shouldBeUndefined(() => array[2]);
                    wtu.shouldBeUndefined(() => array[-1]);
                    wtu.shouldBeUndefined(() => array[0x20000000]);
                };

            Action<dynamic, dynamic, dynamic> testOffsetsAndSizes =
                (type, name, elementSizeInBytes) =>
                {
                    running("test " + name + " OffsetsAndSizes");
                    try
                    {
                        var arr = createtype(type, 0);
                        var len = 10;
                        assertEq("arr.bytesPerElement", elementSizeInBytes, arr.bytesPerElement);
                        var array = createtype(type, len);
                        assert(array.buffer, array.buffer);
                        assertEq("array.byteOffset", 0, array.byteOffset);
                        assertEq("array.length", len, array.length);
                        assertEq("array.byteLength", len * elementSizeInBytes, array.byteLength);
                        array = createtype(type, array.buffer, elementSizeInBytes, len - 1);
                        assert(array.buffer, array.buffer);
                        assertEq("array.byteOffset", elementSizeInBytes, array.byteOffset);
                        assertEq("array.length", len - 1, array.length);
                        assertEq("array.byteLength", (len - 1) * elementSizeInBytes, array.byteLength);
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> testSetFromTypedArray =
                (type, name) =>
                {
                    running("test " + name + " SetFromTypedArray");
                    try
                    {
                        var array = createtype(type, 10);
                        var array2 = createtype(type, 5);
                        for (var i = 0; i < 10; i++)
                        {
                            assertEq("Element " + i, 0, array[i]);
                        }
                        for (var i = 0; i < array2.length; i++)
                        {
                            array2[i] = i;
                        }
                        array.set(array2);
                        for (var i = 0; i < array2.length; i++)
                        {
                            assertEq("Element " + i, i, array[i]);
                        }
                        array.set(array2, 5);
                        for (var i = 0; i < array2.length; i++)
                        {
                            assertEq("Element " + i, i, array[5 + i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> negativeTestSetFromTypedArray =
                (type, name) =>
                {
                    running("negativeTest " + name + " SetFromTypedArray");
                    try
                    {
                        var array = createtype(type, 5);
                        var array2 = createtype(type, 6);
                        for (var i = 0; i < 5; i++)
                        {
                            assertEq("Element " + i, 0, array[i]);
                        }
                        for (var i = 0; i < array2.length; i++)
                        {
                            array2[i] = i;
                        }
                        try
                        {
                            array.set(array2);
                            fail("Expected exception from array.set(array2)");
                            return;
                        }
                        catch
                        {
                        }
                        try
                        {
                            array2.set(array, 2);
                            fail("Expected exception from array2.set(array, 2)");
                            return;
                        }
                        catch
                        {
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> testSetFromArray =
                (type, name) =>
                {
                    running("test " + name + " SetFromArray");
                    try
                    {
                        var array = createtype(type, 10);
                        var array2 = new JSArray(10, 9, 8, 7, 6, 5, 4, 3, 2, 1);
                        for (var i = 0; i < 10; i++)
                        {
                            assertEq("Element " + i, 0, array[i]);
                        }
                        array.set(array2, 0);
                        for (var i = 0; i < array2.length; i++)
                        {
                            assertEq("Element " + i, 10 - i, array[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> negativeTestSetFromArray =
                (type, name) =>
                {
                    running("negativeTest " + name + " SetFromArray");
                    try
                    {
                        var array = createtype(type, new JSArray(2, 3));
                        try
                        {
                            array.set(new JSArray(4, 5), 1);
                            fail(string.Empty);
                            return;
                        }
                        catch (Exception e)
                        {
                        }
                        try
                        {
                            array.set(new JSArray(4, 5, 6));
                            fail(string.Empty);
                            return;
                        }
                        catch
                        {
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> testSubarray =
                (type, name) =>
                {
                    running("test " + name + " Subarray");
                    try
                    {
                        var array = createtype(type, new JSArray(0, 1, 2, 3, 4, 5, 6, 7, 8, 9));
                        var subarray = array.subarray(0, 5);
                        assertEq("subarray.length", 5, subarray.length);
                        for (var i = 0; i < 5; i++)
                        {
                            assertEq("Element " + i, i, subarray[i]);
                        }
                        subarray = array.subarray(4, 10);
                        assertEq("subarray.length", 6, subarray.length);
                        for (var i = 0; i < 6; i++)
                        {
                            assertEq("Element " + i, 4 + i, subarray[i]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> negativeTestSubarray =
                (type, name) =>
                {
                    running("negativeTest " + name + " Subarray");
                    try
                    {
                        var array = createtype(type, new JSArray(0, 1, 2, 3, 4, 5, 6, 7, 8, 9));
                        var subarray = array.subarray(5, 11);
                        if (subarray.length != 5)
                        {
                            fail(string.Empty);
                            return;
                        }
                        subarray = array.subarray(10, 10);
                        if (subarray.length != 0)
                        {
                            fail(string.Empty);
                            return;
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic, dynamic, dynamic> testSetBoundaryConditions =
                (type, name, testValues, expectedValues) =>
                {
                    running("test " + name + " SetBoundaryConditions");
                    try
                    {
                        var array = createtype(type, 1);
                        assertEq("Array length", 1, array.length);
                        for (var ii = 0; ii < testValues.length; ++ii)
                        {
                            for (var jj = 0; jj < 10; ++jj)
                            {
                                array[0] = testValues[ii];
                                assertEq("Element 0", expectedValues[ii], array[0]);
                            }
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic, dynamic, dynamic> testConstructionBoundaryConditions =
                (type, name, testValues, expectedValues) =>
                {
                    running("test " + name + " ConstructionBoundaryConditions");
                    try
                    {
                        var array = createtype(type, testValues);
                        assertEq("Array length", testValues.length, array.length);
                        for (var ii = 0; ii < testValues.length; ++ii)
                        {
                            assertEq("Element " + ii, expectedValues[ii], array[ii]);
                        }
                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            Action<dynamic, dynamic> testConstructionWithNullBuffer =
                (type, name) =>
                {
                    dynamic array;
                    try
                    {
                        array = createtype(type, null);
                        wtu.testFailed("Construction of " + name + " with null buffer should throw exception");
                    }
                    catch
                    {
                        wtu.testPassed("Construction of " + name + " with null buffer threw exception");
                    }
                    try
                    {
                        array = createtype(type, null, 0, 0);
                        wtu.testFailed("Construction of " + name + " with (null buffer, 0) should throw exception");
                    }
                    catch
                    {
                        wtu.testPassed("Construction of " + name + " with (null buffer, 0) threw exception");
                    }
                    try
                    {
                        array = createtype(type, null, 0, 0);
                        wtu.testFailed("Construction of " + name + " with (null buffer, 0, 0) should throw exception");
                    }
                    catch
                    {
                        wtu.testPassed("Construction of " + name + " with (null buffer, 0, 0) threw exception");
                    }
                };

            Action<Action, string> shouldThrowIndexSizeErr =
                (func, text) =>
                {
                    var errorText = text + " should throw an exception";
                    try
                    {
                        func();
                        wtu.testFailed(errorText);
                    }
                    catch (Exception)
                    {
                        wtu.testPassed(text + " threw an exception");
                    }
                };

            Action<string, string> testConstructionWithOutOfRangeValues =
                (type, name) => shouldThrowIndexSizeErr(() =>
                                                        {
                                                            var buffer = new ArrayBuffer(4);
                                                            var array = createtype(type, buffer, 4, 0x3FFFFFFF);
                                                        }, "Construction of " + name + " with out-of-range values");

            Action<string, string> testConstructionWithNegativeOutOfRangeValues =
                (type, name) =>
                {
                    try
                    {
                        var buffer = new ArrayBuffer(-1);
                        wtu.testFailed("Construction of ArrayBuffer with negative size should throw exception");
                    }
                    catch
                    {
                        wtu.testPassed("Construction of ArrayBuffer with negative size threw exception");
                    }
                    try
                    {
                        var array = createtype(type, -1);
                        wtu.testFailed("Construction of " + name + " with negative size should throw exception");
                    }
                    catch
                    {
                        wtu.testPassed("Construction of " + name + " with negative size threw exception");
                    }
                    shouldThrowIndexSizeErr(() =>
                                            {
                                                var buffer = new ArrayBuffer(4);
                                                var array = createtype(type, buffer, 4, -2147483648);
                                            }, "Construction of " + name + " with negative out-of-range values");
                };

            Action<string, string, int> testConstructionWithUnalignedOffset =
                (type, name, elementSizeInBytes) =>
                {
                    if (elementSizeInBytes > 1)
                    {
                        shouldThrowIndexSizeErr(() =>
                                                {
                                                    var buffer = new ArrayBuffer(32);
                                                    var array = createtype(type, buffer, 1, elementSizeInBytes);
                                                }, "Construction of " + name + " with unaligned offset");
                    }
                };

            Action<string, string, int> testConstructionWithUnalignedLength =
                (type, name, elementSizeInBytes) =>
                {
                    if (elementSizeInBytes > 1)
                    {
                        shouldThrowIndexSizeErr(() =>
                                                {
                                                    var buffer = new ArrayBuffer(elementSizeInBytes + 1);
                                                    var array = createtype(type, buffer);
                                                }, "Construction of " + name + " with unaligned length");
                    }
                };

            Action<dynamic, dynamic, dynamic> testConstructionOfHugeArray =
                (type, name, sz) =>
                {
                    if (sz == 1)
                    {
                        return;
                    }
                    try
                    {
                        // Construction of huge arrays must fail because byteLength is
                        // an unsigned long
                        var array = createtype(type, 3000000000);
                        wtu.testFailed("Construction of huge " + name + " should throw exception");
                    }
                    catch
                    {
                        wtu.testPassed("Construction of huge " + name + " threw exception");
                    }
                };

            Action<dynamic, dynamic, dynamic> testConstructionWithBothArrayBufferAndLength =
                (type, name, elementSizeInBytes) =>
                {
                    var bufByteLength = 1000 * elementSizeInBytes;
                    var buf = new ArrayBuffer(bufByteLength);
                    var array1 = createtype(type, buf);
                    var array2 = createtype(type, bufByteLength / elementSizeInBytes);
                    if (array1.length == array2.length)
                    {
                        wtu.testPassed("Array lengths matched with explicit and implicit creation of ArrayBuffer");
                    }
                    else
                    {
                        wtu.testFailed("Array lengths DID NOT MATCH with explicit and implicit creation of ArrayBuffer");
                    }
                };

            Action<string, string, int> testSubarrayWithOutOfRangeValues =
                (type, name, sz) =>
                {
                    wtu.debug("Testing subarray of " + name);
                    try
                    {
                        var buffer = new ArrayBuffer(32);
                        var array = createtype(type, buffer);
                        var typeSize = sz;
                        wtu.shouldBe(() => array.length, 32 / typeSize);
                        try
                        {
                            wtu.shouldBe(() => array.subarray(4, 0x3FFFFFFF).length, (32 / typeSize) - 4);
                            wtu.shouldBe(() => array.subarray(4, -2147483648).length, 0);
                            // Test subarray() against overflows.
                            array = array.subarray(2);
                            if (sz > 1)
                            {
                                // Full byte offset is +1 larger than the maximum unsigned long int.
                                // Make sure subarray() still handles it correctly.  Otherwise overflow would happen and
                                // offset would be 0, and array.length array.length would incorrectly be 1.
                                var start = 4294967296 / sz - 2;
                                array = array.subarray(start, start + 1);
                                wtu.shouldBe(() => array.length, 0);
                            }
                        }
                        catch
                        {
                            wtu.testFailed("Subarray of " + name + " threw exception");
                        }
                    }
                    catch (Exception e)
                    {
                        wtu.testFailed("Exception: " + e);
                    }
                };

            Action<dynamic, dynamic, dynamic> testSubarrayWithDefaultValues =
                (type, name, sz) =>
                {
                    wtu.debug("Testing subarray with default inputs of " + name);
                    try
                    {
                        var buffer = new ArrayBuffer(32);
                        var array = createtype(type, buffer);
                        var typeSize = sz;
                        wtu.shouldBe((Func<dynamic>)(() => array.length), 32 / typeSize);
                        try
                        {
                            wtu.shouldBe((Func<dynamic>)(() => array.subarray(0).length), 32 / typeSize);
                            wtu.shouldBe((Func<dynamic>)(() => array.subarray(2).length), (32 / typeSize) - 2);
                            wtu.shouldBe(() => array.subarray(-2).length, 2);
                            wtu.shouldBe((Func<dynamic>)(() => array.subarray(-2147483648).length), (32 / typeSize));
                        }
                        catch
                        {
                            wtu.testFailed("Subarray of " + name + " threw exception");
                        }
                    }
                    catch (Exception e)
                    {
                        wtu.testFailed("Exception: " + e);
                    }
                };

            Action<string, string> testSettingFromArrayWithOutOfRangeOffset =
                (type, name) =>
                {
                    var webglArray = createtype(type, 32);
                    var array = new JSArray();
                    for (var i = 0; i < 16; i++)
                    {
                        array.push(i);
                    }
                    try
                    {
                        webglArray.set(array, 0x7FFFFFF8);
                        wtu.testFailed("Setting " + name + " from array with out-of-range offset was not caught");
                    }
                    catch
                    {
                        wtu.testPassed("Setting " + name + " from array with out-of-range offset was caught");
                    }
                };

            Action<string, string> testSettingFromFakeArrayWithOutOfRangeLength =
                (type, name) =>
                {
                    var webglArray = createtype(type, 32);
                    var array = new JSArray();
                    array.length = -1;
                    try
                    {
                        webglArray.set(array, 8);
                        wtu.testFailed("Setting " + name + " from fake array with invalid length was not caught");
                    }
                    catch
                    {
                        wtu.testPassed("Setting " + name + " from fake array with invalid length was caught");
                    }
                };

            Action<string, string> testSettingFromTypedArrayWithOutOfRangeOffset =
                (type, name) =>
                {
                    var webglArray = createtype(type, 32);
                    var srcArray = createtype(type, 16);
                    for (var i = 0; i < 16; i++)
                    {
                        srcArray[i] = i;
                    }
                    try
                    {
                        webglArray.set(srcArray, 0x7FFFFFF8);
                        wtu.testFailed("Setting " + name + " from " + name + " with out-of-range offset was not caught");
                    }
                    catch
                    {
                        wtu.testPassed("Setting " + name + " from " + name + " with out-of-range offset was caught");
                    }
                };

            Action<dynamic, dynamic> negativeTestGetAndSetMethods =
                (type, name) =>
                {
                    var webGLArray = createtype(type, new JSArray(2, 3));
                    wtu.shouldBeUndefined(() => webGLArray.get);
                    var exceptionThrown = false;
                    // We deliberately check for an exception here rather than using
                    // shouldThrow here because the precise contents of the syntax
                    // error are not specified.
                    try
                    {
                        webGLArray.set(0, 1);
                    }
                    catch
                    {
                        exceptionThrown = true;
                    }
                    var output2 = "array.set(0, 1) ";
                    if (exceptionThrown)
                    {
                        wtu.testPassed(output2 + "threw exception.");
                    }
                    else
                    {
                        wtu.testFailed(output2 + "did not throw exception.");
                    }
                };

            Action<string, string> testNaNConversion =
                (type, name) =>
                {
                    running("test storing NaN in " + name);

                    var array = createtype(type, new JSArray(1, 1));
                    var results = new JSArray();

                    // The code block in each of the case statements below is identical, but some
                    // JavaScript engines need separate copies in order to exercise all of
                    // their optimized code paths.
                    try
                    {
                        switch (type)
                        {
                            case "Float32Array":
                                for (var i = 0; i < array.length; ++i)
                                {
                                    array[i] = float.NaN;
                                    results[i] = array[i];
                                }
                                break;
                            case "Int8Array":
                                for (var i = 0; i < array.length; ++i)
                                {
                                    array[i] = float.NaN;
                                    results[i] = array[i];
                                }
                                break;
                            case "Int16Array":
                                for (var i = 0; i < array.length; ++i)
                                {
                                    array[i] = float.NaN;
                                    results[i] = array[i];
                                }
                                break;
                            case "Int32Array":
                                for (var i = 0; i < array.length; ++i)
                                {
                                    array[i] = float.NaN;
                                    results[i] = array[i];
                                }
                                break;
                            case "Uint8Array":
                                for (var i = 0; i < array.length; ++i)
                                {
                                    array[i] = float.NaN;
                                    results[i] = array[i];
                                }
                                break;
                            case "Uint16Array":
                                for (var i = 0; i < array.length; ++i)
                                {
                                    array[i] = float.NaN;
                                    results[i] = array[i];
                                }
                                break;
                            case "Uint32Array":
                                for (var i = 0; i < array.length; ++i)
                                {
                                    array[i] = float.NaN;
                                    results[i] = array[i];
                                }
                                break;
                            default:
                                fail("Unhandled type");
                                break;
                        }

                        // Some types preserve NaN values; all other types convert NaN to zero.
                        if (type == "Float32Array")
                        {
                            assert("initial NaN preserved", float.IsNaN(createtype(type, new JSArray(double.NaN))[0]));
                            for (var i = 0; i < array.length; ++i)
                            {
                                assert("NaN preserved via setter", float.IsNaN((float)results[i]));
                            }
                        }
                        else
                        {
                            assertEq("initial NaN converted to zero", 0, createtype(type, new JSArray(double.NaN))[0]);
                            for (var i = 0; i < array.length; ++i)
                            {
                                assertEq("NaN converted to zero by setter", 0, results[i]);
                            }
                        }

                        pass();
                    }
                    catch (Exception e)
                    {
                        fail(e.ToString());
                    }
                };

            //
            // Test driver
            //
            Action runTests =
                () =>
                {
                    allPassed = true;

                    // The "name" attribute is a concession to browsers which don"t
                    // implement the "name" property on Action objects
                    var testCases = new[]
                                    {
                                        new
                                        {
                                            name = "Float32Array",
                                            unsigned = false,
                                            integral = false,
                                            elementSizeInBytes = 4,
                                            testValues = new JSArray(-500.5f, 500.5f),
                                            expectedValues = new JSArray(-500.5f, 500.5f)
                                        },
                                        new
                                        {
                                            name = "Int8Array",
                                            unsigned = false,
                                            integral = true,
                                            elementSizeInBytes = 1,
                                            testValues = new JSArray(-128, 127, -129, 128),
                                            expectedValues = new JSArray(-128, 127, 127, -128)
                                        },
                                        new
                                        {
                                            name = "Int16Array",
                                            unsigned = false,
                                            integral = true,
                                            elementSizeInBytes = 2,
                                            testValues = new JSArray(-32768, 32767, -32769, 32768),
                                            expectedValues = new JSArray(-32768, 32767, 32767, -32768)
                                        },
                                        new
                                        {
                                            name = "Int32Array",
                                            unsigned = false,
                                            integral = true,
                                            elementSizeInBytes = 4,
                                            testValues = new JSArray(-2147483648, 2147483647, -2147483649, 2147483648),
                                            expectedValues = new JSArray(-2147483648, 2147483647, 2147483647, -2147483648)
                                        },
                                        new
                                        {
                                            name = "Uint8Array",
                                            unsigned = true,
                                            integral = true,
                                            elementSizeInBytes = 1,
                                            testValues = new JSArray(0, 255, -1, 256),
                                            expectedValues = new JSArray(0, 255, 255, 0)
                                        },
                                        new
                                        {
                                            name = "Uint16Array",
                                            unsigned = true,
                                            integral = true,
                                            elementSizeInBytes = 2,
                                            testValues = new JSArray(0, 65535, -1, 65536),
                                            expectedValues = new JSArray(0, 65535, 65535, 0)
                                        },
                                        new
                                        {
                                            name = "Uint32Array",
                                            unsigned = true,
                                            integral = true,
                                            elementSizeInBytes = 4,
                                            testValues = new JSArray(0, 4294967295, -1, 4294967296),
                                            expectedValues = new JSArray(0, 4294967295, 4294967295, 0)
                                        }
                                    };

                    for (var i = 0; i < testCases.Length; i++)
                    {
                        var testCase = testCases[i];
                        running(testCase.name);
//                    if (!(testCase.name in window)) {
//                        fail("does not exist");
//                        continue;
//                    }
                        var type = testCase.name;
                        var name = testCase.name;
                        if (testCase.unsigned)
                        {
                            testSetAndGet10To1(type, name);
                            testConstructWithArrayOfUnsignedValues(type, name);
                            testConstructWithTypedArrayOfUnsignedValues(type, name);
                        }
                        else
                        {
                            testSetAndGetPos10ToNeg10(type, name);
                            testConstructWithArrayOfSignedValues(type, name);
                            testConstructWithTypedArrayOfSignedValues(type, name);
                        }
                        if (testCase.integral)
                        {
                            testIntegralArrayTruncationBehavior(type, name, testCase.unsigned);
                        }
                        testGetWithOutOfRangeIndices(type, name);
                        testOffsetsAndSizes(type, name, testCase.elementSizeInBytes);
                        testSetFromTypedArray(type, name);
                        negativeTestSetFromTypedArray(type, name);
                        testSetFromArray(type, name);
                        negativeTestSetFromArray(type, name);
                        testSubarray(type, name);
                        negativeTestSubarray(type, name);
                        testSetBoundaryConditions(type,
                                                  name,
                                                  testCase.testValues,
                                                  testCase.expectedValues);
                        testConstructionBoundaryConditions(type,
                                                           name,
                                                           testCase.testValues,
                                                           testCase.expectedValues);
                        testConstructionWithNullBuffer(type, name);
                        testConstructionWithOutOfRangeValues(type, name);
                        testConstructionWithNegativeOutOfRangeValues(type, name);
                        testConstructionWithUnalignedOffset(type, name, testCase.elementSizeInBytes);
                        testConstructionWithUnalignedLength(type, name, testCase.elementSizeInBytes);
                        testConstructionOfHugeArray(type, name, testCase.elementSizeInBytes);
                        testConstructionWithBothArrayBufferAndLength(type, name, testCase.elementSizeInBytes);
                        testSubarrayWithOutOfRangeValues(type, name, testCase.elementSizeInBytes);
                        testSubarrayWithDefaultValues(type, name, testCase.elementSizeInBytes);
                        testSettingFromArrayWithOutOfRangeOffset(type, name);
                        testSettingFromFakeArrayWithOutOfRangeLength(type, name);
                        testSettingFromTypedArrayWithOutOfRangeOffset(type, name);
                        negativeTestGetAndSetMethods(type, name);
                        testNaNConversion(type, name);
                    }

                    printSummary();
                };

            runTests();
        }

        private dynamic createtype(string type, ArrayBuffer buffer, int byteOffset, int length)
        {
            switch (type)
            {
                case "Float32Array":
                    return new Float32Array(buffer, byteOffset, length);
                case "Int8Array":
                    return new Int8Array(buffer, byteOffset, length);
                case "Int16Array":
                    return new Int16Array(buffer, byteOffset, length);
                case "Int32Array":
                    return new Int32Array(buffer, byteOffset, length);
                case "Uint8Array":
                    return new Uint8Array(buffer, byteOffset, length);
                case "Uint16Array":
                    return new Uint16Array(buffer, byteOffset, length);
                case "Uint32Array":
                    return new Uint32Array(buffer, byteOffset, length);
                default:
                    throw new NotImplementedException();
            }
        }

        private dynamic createtype(string type, dynamic src)
        {
            switch (type)
            {
                case "Float32Array":
                    return new Float32Array(src);
                case "Int8Array":
                    return new Int8Array(src);
                case "Int16Array":
                    return new Int16Array(src);
                case "Int32Array":
                    return new Int32Array(src);
                case "Uint8Array":
                    return new Uint8Array(src);
                case "Uint16Array":
                    return new Uint16Array(src);
                case "Uint32Array":
                    return new Uint32Array(src);
                default:
                    throw new NotImplementedException();
            }
        }

        private dynamic createtype(string type, int length)
        {
            switch (type)
            {
                case "Float32Array":
                    return new Float32Array(length);
                case "Int8Array":
                    return new Int8Array(length);
                case "Int16Array":
                    return new Int16Array(length);
                case "Int32Array":
                    return new Int32Array(length);
                case "Uint8Array":
                    return new Uint8Array(length);
                case "Uint16Array":
                    return new Uint16Array(length);
                case "Uint32Array":
                    return new Uint32Array(length);
                default:
                    throw new NotImplementedException();
            }
        }

        private dynamic createtype(string type, JSArray vals)
        {
            dynamic arr = null;
            switch (type)
            {
                case "Float32Array":
                    arr = new Float32Array(vals.length);
                    break;
                case "Int8Array":
                    arr = new Int8Array(vals.length);
                    break;
                case "Int16Array":
                    arr = new Int16Array(vals.length);
                    break;
                case "Int32Array":
                    arr = new Int32Array(vals.length);
                    break;
                case "Uint8Array":
                    arr = new Uint8Array(vals.length);
                    break;
                case "Uint16Array":
                    arr = new Uint16Array(vals.length);
                    break;
                case "Uint32Array":
                    arr = new Uint32Array(vals.length);
                    break;
                default:
                    throw new NotImplementedException();
            }
            for (var i = 0; i < vals.length; i++)
            {
                arr[i] = vals[i];
            }
            return arr;
        }
    }
}