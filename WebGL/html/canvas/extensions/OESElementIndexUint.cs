﻿namespace WebGL
{
    // ReSharper disable InconsistentNaming

    internal class OESElementIndexUint : WebGLExtension
    {
        internal OESElementIndexUint(WebGLRenderingContext context) : base(context)
        {
        }

        internal override ExtensionName getName()
        {
            return ExtensionName.OESElementIndexUintName;
        }
    }

    // ReSharper restore InconsistentNaming
}
