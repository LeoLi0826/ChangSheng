using UnityEngine;
using UnityEditor;

namespace ET
{
    public class PlayModeSettings
    {
        private static GUIContent playModeContent;
        private const string MaximizeOnPlayKey = "MaximizeOnPlay";
        private const string MuteOnPlayKey = "MuteOnPlay";
        private const string PauseOnPlayKey = "PauseOnPlay";

        [InitializeOnLoadMethod]
        static void Init()
        {
            playModeContent = EditorGUIUtility.TrTextContentWithIcon("Play Mode",
                "播放模式设置",
                "UnityEditor.GameView");

            UnityEditorToolbar.AddRightToolbarElement(OnToolbarGUI);

            // 注册播放模式变更事件
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnToolbarGUI()
        {
            if (EditorGUILayout.DropdownButton(playModeContent, FocusType.Passive, EditorStyles.toolbarPopup))
            {
                ShowPlayModeMenu();
            }
        }

        static void ShowPlayModeMenu()
        {
            var menu = new GenericMenu();

            bool maximizeOnPlay = EditorPrefs.GetBool(MaximizeOnPlayKey, false);
            bool muteOnPlay = EditorPrefs.GetBool(MuteOnPlayKey, false);
            bool pauseOnPlay = EditorPrefs.GetBool(PauseOnPlayKey, false);

            menu.AddItem(new GUIContent("运行时最大化游戏视图"), maximizeOnPlay, () => { EditorPrefs.SetBool(MaximizeOnPlayKey, !maximizeOnPlay); });

            menu.AddItem(new GUIContent("运行时静音"), muteOnPlay, () => { EditorPrefs.SetBool(MuteOnPlayKey, !muteOnPlay); });

            menu.AddItem(new GUIContent("运行时暂停"), pauseOnPlay, () => { EditorPrefs.SetBool(PauseOnPlayKey, !pauseOnPlay); });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("清除所有PlayerPrefs"), false, () =>
            {
                if (EditorUtility.DisplayDialog("清除PlayerPrefs",
                        "确定要清除所有PlayerPrefs数据吗？",
                        "确定", "取消"))
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                }
            });

            menu.ShowAsContext();
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                // 最大化游戏视图
                if (EditorPrefs.GetBool(MaximizeOnPlayKey, false))
                {
                    var gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
                    gameView.maximized = true;
                }

                // 静音
                if (EditorPrefs.GetBool(MuteOnPlayKey, false))
                {
                    AudioListener.volume = 0;
                }

                // 暂停
                if (EditorPrefs.GetBool(PauseOnPlayKey, false))
                {
                    EditorApplication.isPaused = true;
                }
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // 恢复音量
                if (EditorPrefs.GetBool(MuteOnPlayKey, false))
                {
                    AudioListener.volume = 1;
                }

                // 取消最大化
                if (EditorPrefs.GetBool(MaximizeOnPlayKey, false))
                {
                    var gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
                    gameView.maximized = false;
                }
            }
        }
    }
}