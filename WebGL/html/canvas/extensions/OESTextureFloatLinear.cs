namespace WebGL
{
    // ReSharper disable InconsistentNaming

    internal class OESTextureFloatLinear : WebGLExtension
    {
        internal OESTextureFloatLinear(WebGLRenderingContext context) : base(context)
        {
        }

        internal override ExtensionName getName()
        {
            return ExtensionName.OESTextureFloatLinearName;
        }
    }

    // ReSharper restore InconsistentNaming
}
