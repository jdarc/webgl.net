using System;
using System.Windows.Forms;
using Demo.THREE;

namespace Demo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Nehe.Lesson4Form());
            Application.Run(new Nehe.Lesson5Form());
            Application.Run(new Nehe.Lesson16Form());
            Application.Run(new BenchmarkForm());

            Application.Run(new CustomAttributesForm());
            Application.Run(new GeometryShapesForm());
            Application.Run(new TrailsForm());
            Application.Run(new GeometryCubeForm());
            Application.Run(new GeometriesForm());
            Application.Run(new BufferGeometryForm());
            Application.Run(new BufferGeometryLinesForm());
            Application.Run(new BufferGeometryParticlesForm());
            Application.Run(new CameraForm());
            Application.Run(new PerformanceDoubleSidedForm());
            Application.Run(new PerformanceStaticForm());
            Application.Run(new LoaderCtmMaterialsForm());
            Application.Run(new ParticlesBillboardsForm());
            Application.Run(new AnimationSkinningForm());
            Application.Run(new MaterialsTextureCompressedForm());
            Application.Run(new MaterialsForm());
            Application.Run(new LodForm());
            Application.Run(new LinesSphereForm());
            Application.Run(new LinesSplinesForm());
            Application.Run(new MaterialsWireframeForm());
            Application.Run(new MaterialsTextureAnisotropyForm());
            Application.Run(new MaterialsTextureFiltersForm());
            Application.Run(new MaterialsTextureManualMipmapForm());
            Application.Run(new AnimationSkinningMorphForm());
            Application.Run(new MaterialsBlendingForm());
            Application.Run(new LinesDashedForm());
            Application.Run(new LightsHemisphereForm());
            Application.Run(new MaterialsLightmapForm());
            Application.Run(new TestMemoryForm());
            Application.Run(new SpritesForm());
            Application.Run(new PerformanceForm());
            Application.Run(new ParticlesSpritesForm());
            Application.Run(new MorphNormalsForm());
            Application.Run(new MorphTargetsHorseForm());
            Application.Run(new CustomAttributesParticlesForm());
            Application.Run(new CustomAttributesParticles2Form());
            Application.Run(new CustomAttributesParticles3Form());
            Application.Run(new CustomAttributesRibbonsForm());
            Application.Run(new GeometryColorsForm());
        }
    }
}
