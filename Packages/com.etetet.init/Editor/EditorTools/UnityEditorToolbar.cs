using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public static class ToolbarCallback
    {
        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject currentToolbar;

        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            // 如果工具栏实例不存在，则查找并设置
            if (currentToolbar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
                if (toolbars.Length == 0) return;

                currentToolbar = toolbars[0] as ScriptableObject;
                if (currentToolbar == null) return;

                // 获取根可视化元素
                var rootField = ToolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                if (rootField == null) return;

                var root = rootField.GetValue(currentToolbar) as VisualElement;
                if (root == null) return;

                // 注册左侧和右侧回调
                RegisterToolbarCallback(root, "ToolbarZoneLeftAlign", OnToolbarGUILeft);
                RegisterToolbarCallback(root, "ToolbarZoneRightAlign", OnToolbarGUIRight);
            }
        }

        private static void RegisterToolbarCallback(VisualElement root, string zoneName, Action callback)
        {
            var zone = root.Q(zoneName);
            if (zone == null) return;

            var container = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                }
            };

            var imguiContainer = new IMGUIContainer();
            imguiContainer.style.flexGrow = 1;
            imguiContainer.onGUIHandler = () => callback?.Invoke();
            
            container.Add(imguiContainer);
            zone.Add(container);
        }
    }

    [InitializeOnLoad]
    public static class UnityEditorToolbar
    {
        private static GUIStyle commandStyle;
        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();

        static UnityEditorToolbar()
        {
            ToolbarCallback.OnToolbarGUILeft = OnGUILeft;
            ToolbarCallback.OnToolbarGUIRight = OnGUIRight;
        }

        private static void OnGUILeft()
        {
            InitStyles();
            
            GUILayout.BeginHorizontal(commandStyle);
            foreach (var handler in LeftToolbarGUI)
            {
                try
                {
                    handler?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            GUILayout.EndHorizontal();
        }

        private static void OnGUIRight()
        {
            InitStyles();
            
            GUILayout.BeginHorizontal(commandStyle);
            foreach (var handler in RightToolbarGUI)
            {
                try
                {
                    handler?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            GUILayout.EndHorizontal();
        }

        private static void InitStyles()
        {
            if (commandStyle == null)
            {
                commandStyle = new GUIStyle("CommandLeft")
                {
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 22,
                    stretchHeight = true
                };
            }
        }

        public static void AddLeftToolbarElement(Action guiHandler)
        {
            if (guiHandler != null && !LeftToolbarGUI.Contains(guiHandler))
            {
                LeftToolbarGUI.Add(guiHandler);
            }
        }

        public static void AddRightToolbarElement(Action guiHandler)
        {
            if (guiHandler != null && !RightToolbarGUI.Contains(guiHandler))
            {
                RightToolbarGUI.Add(guiHandler);
            }
        }

        public static void RemoveLeftToolbarElement(Action guiHandler)
        {
            LeftToolbarGUI.Remove(guiHandler);
        }

        public static void RemoveRightToolbarElement(Action guiHandler)
        {
            RightToolbarGUI.Remove(guiHandler);
        }
    }
}