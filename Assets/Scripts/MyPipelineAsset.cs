using UnityEngine;
using UnityEngine.Rendering;
namespace SRPStudy
{
    [CreateAssetMenu(menuName = "Rendering/MyPipeline")]
    public class MyPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            //返回上一个脚本自定义的管线
            return new MyPipeline();
        }
    }

}