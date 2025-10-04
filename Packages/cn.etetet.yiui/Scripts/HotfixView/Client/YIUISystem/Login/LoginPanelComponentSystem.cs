using System;
using UnityEngine;
using YIUIFramework;
using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// Author  YIUI
    /// Date    2025.10.3
    /// Desc
    /// </summary>
    [FriendOf(typeof(LoginPanelComponent))]
    public static partial class LoginPanelComponentSystem
    {
        [EntitySystem]
        private static void YIUIInitialize(this LoginPanelComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this LoginPanelComponent self)
        {
        }

        [EntitySystem]
        private static async ETTask<bool> YIUIOpen(this LoginPanelComponent self)
        {
            await ETTask.CompletedTask;
            return true;
        }

        #region YIUIEvent开始
        
        [YIUIInvoke(LoginPanelComponent.OnEventPasswordInvoke)]
        private static void OnEventPasswordInvoke(this LoginPanelComponent self, string p1)
        {

        }
        
        [YIUIInvoke(LoginPanelComponent.OnEventAccountInvoke)]
        private static void OnEventAccountInvoke(this LoginPanelComponent self, string p1)
        {

        }
        
        [YIUIInvoke(LoginPanelComponent.OnEventLoginInvoke)]
        private static async ETTask OnEventLoginInvoke(this LoginPanelComponent self)
        {
            await EventSystem.Instance.PublishAsync(self.Scene(),new StartGame());
        }
        #endregion YIUIEvent结束
    }
}
