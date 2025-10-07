using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace ET
{
    public class SceneQuickSwitch
    {
        private static GUIContent switchSceneContent;
        private static List<string> sceneAssetList;

        [InitializeOnLoadMethod]
        static void Init()
        {
            sceneAssetList = new List<string>();
            var curOpenSceneName = EditorSceneManager.GetActiveScene().name;
            switchSceneContent = EditorGUIUtility.TrTextContentWithIcon(string.IsNullOrEmpty(curOpenSceneName) ? "Switch Scene" : curOpenSceneName,
                "切换场景",
                "UnityLogo");

            EditorSceneManager.sceneOpened += OnSceneOpened;
            UnityEditorToolbar.AddLeftToolbarElement(OnToolbarGUI);
        }

        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            switchSceneContent.text = scene.name;
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            if (EditorGUILayout.DropdownButton(switchSceneContent, FocusType.Passive, EditorStyles.toolbarPopup, GUILayout.MaxWidth(150)))
            {
                DrawSwitchSceneDropdownMenus();
            }
        }

        static void DrawSwitchSceneDropdownMenus()
        {
            GenericMenu popMenu = new GenericMenu();
            popMenu.allowDuplicateNames = true;
            var sceneGuids = AssetDatabase.FindAssets("t:Scene");
            sceneAssetList.Clear();

            for (int i = 0; i < sceneGuids.Length; i++)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                sceneAssetList.Add(scenePath);

                string fileDir = Path.GetDirectoryName(scenePath);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                string displayName = sceneName;

                // 如果场景在文件夹中，显示相对路径
                if (fileDir != "Assets")
                {
                    string relativePath = fileDir.Replace("Assets/", "");
                    displayName = $"{relativePath}/{sceneName}";
                }

                popMenu.AddItem(new GUIContent(displayName), false, menuIdx => { SwitchScene((int)menuIdx); }, i);
            }

            popMenu.ShowAsContext();
        }

        private static void SwitchScene(int menuIdx)
        {
            if (menuIdx >= 0 && menuIdx < sceneAssetList.Count)
            {
                var scenePath = sceneAssetList[menuIdx];
                var curScene = EditorSceneManager.GetActiveScene();

                if (curScene != null && curScene.isDirty)
                {
                    int opIndex = EditorUtility.DisplayDialogComplex("警告",
                        $"当前场景{curScene.name}未保存,是否保存?",
                        "保存",
                        "取消",
                        "不保存");

                    switch (opIndex)
                    {
                        case 0: // 保存
                            if (!EditorSceneManager.SaveOpenScenes())
                            {
                                return;
                            }

                            break;
                        case 1: // 取消
                            return;
                        case 2: // 不保存
                            break;
                    }
                }

                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
        }
    }
}
