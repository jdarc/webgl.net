namespace WebGL
{
    // ReSharper disable InconsistentNaming

    internal class EXTTextureFilterAnisotropic : WebGLExtension
    {
        internal EXTTextureFilterAnisotropic(WebGLRenderingContext context) : base(context)
        {
        }

        internal override ExtensionName getName()
        {
            return ExtensionName.EXTTextureFilterAnisotropicName;
        }
    }

    // ReSharper restore InconsistentNaming
}
