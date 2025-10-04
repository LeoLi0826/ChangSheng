using System;
using UnityEngine.SceneManagement;

namespace ET.Client
{
    [Event(SceneType.StateSync)]
    public class SceneChangeStart_AddComponent: AEvent<Scene, SceneChangeStart>
    {
        protected override async ETTask Run(Scene root, SceneChangeStart args)
        {
            try
            {
                Scene currentScene = root.CurrentScene();
                EntityRef<CurrentScenesComponent> currentScenesComponentRef = root.GetComponent<CurrentScenesComponent>();
                ResourcesLoaderComponent resourcesLoaderComponent = currentScene.GetComponent<ResourcesLoaderComponent>();
                // 加载场景资源
                await resourcesLoaderComponent.LoadSceneAsync(currentScene.Name, LoadSceneMode.Single,
                    (progress) =>
                    {
                        CurrentScenesComponent currentScenes = currentScenesComponentRef;
                        currentScenes.Progress = (int)progress * 99f;
                    });

                currentScene.AddComponent<MapManageComponent>();
                currentScene.AddComponent<MapGeneratorComponent>();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

        }
    }
}