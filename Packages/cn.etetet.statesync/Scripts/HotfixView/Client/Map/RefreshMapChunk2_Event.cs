using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Main)]
    public class RefreshMapChunk2_Event : AEvent<Scene, RefreshMapChunk2>
    {
        protected override async ETTask Run(Scene scene, RefreshMapChunk2 a)
        {
            await ETTask.CompletedTask;
        }
    }
}
