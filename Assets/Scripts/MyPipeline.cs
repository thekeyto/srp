using UnityEngine;
using UnityEngine.Rendering;
namespace SRPStudy
{
    public class MyPipeline : RenderPipeline
    {
        private CommandBuffer myCommandBuffer;
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            if (myCommandBuffer == null) myCommandBuffer = new CommandBuffer() { name = "SRP Study" };
            var _LightDir0 = Shader.PropertyToID("_DLightDir");
            var _LightColor0 = Shader.PropertyToID("_DLightColor");
            var _CameraPos = Shader.PropertyToID("_CameraPos");
            foreach (var camera in cameras)
            {
                context.SetupCameraProperties(camera);

                myCommandBuffer.ClearRenderTarget(true, true, Color.gray);

                ScriptableCullingParameters cullParam = new ScriptableCullingParameters();
                camera.TryGetCullingParameters(out cullParam);
                cullParam.isOrthographic = false;
                CullingResults cullResults = context.Cull(ref cullParam);

                var lights = cullResults.visibleLights;
                myCommandBuffer.name = "Render Lights";
                foreach (var light in lights)
                {
                    if (light.lightType != LightType.Directional) continue;
                    Vector4 lightpos = light.localToWorldMatrix.GetColumn(2);
                    Vector4 lightDir = -lightpos;
                    lightDir.w = 0;
                    Color lightColor = light.finalColor;
                    Vector4 cameraPos = camera.transform.position;
                    myCommandBuffer.SetGlobalVector(_CameraPos, cameraPos);
                    myCommandBuffer.SetGlobalVector(_LightDir0, lightDir);
                    myCommandBuffer.SetGlobalColor(_LightColor0, lightColor);
                    break;
                }

                context.ExecuteCommandBuffer(myCommandBuffer);
                myCommandBuffer.Clear();

                SortingSettings sortSet = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
                DrawingSettings drawSet = new DrawingSettings(new ShaderTagId("BaseLit"), sortSet);

                FilteringSettings filtSet = new FilteringSettings(RenderQueueRange.opaque, -1);

                context.DrawRenderers(cullResults, ref drawSet, ref filtSet);
                context.DrawSkybox(camera);
                context.Submit();
            }
        }
    }

}