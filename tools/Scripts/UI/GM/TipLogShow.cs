using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
[Hotfix]
    public class TipLogShow : PanelBase
    {
        public Text LogContent;
        public Toggle AutoRefresh;

        public void OnCloseClick()
        {
            MainPanelMgr.Instance.Close("TipLogShow");
        }

        public void OnRefreshClick()
        {
            Refresh();
        }

        public void OnAutoRefreshClick(bool c)
        {
            if (c)
            {
                Refresh();
            }
        }

        private void OnRecordLog(GameEvent ge, EventParameter parameter)
        {
            if (AutoRefresh.isOn)
            {
                Refresh();
            }
        }

        public override void Show()
        {
            base.Show();
            LogMgr.StartSaveLog();
            Refresh();

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_RECORD_LOG, OnRecordLog);
        }

        public override void Hide()
        {
            base.Hide();
            LogMgr.StopSaveLog();

            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_RECORD_LOG, OnRecordLog);
        }
        
        public void Refresh()
        {
            LogContent.text = LogMgr.GetLosShow();
        }
    }
}

