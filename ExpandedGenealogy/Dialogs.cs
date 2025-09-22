using System;
using System.Collections.Generic;
using Sims3.SimIFace;
using Sims3.UI;

namespace Destrospean.ExpandedGenealogy
{
    public class Dialogs
    {
        public class ComboSelectionDialog : ModalDialog
        {
            public enum ControlID : uint
            {
                kComboBoxId = 2u,
                kOKButtonID,
                kCancelButtonID,
                kTitleTextID
            }

            public const string kLayoutName = "ComboSelectionDialog";

            public const int kWinExportID = 4096;

            public Button mOkButton;

            public Button mCancelButton;

            public ComboBox mCombo;

            public object mResult;

            public object Result
            {
                get
                {
                    return mResult;
                }
            }

            public static object Show(string titleText, IDictionary<string, object> entries, object defaultEntry)
            {
                return Show(titleText, entries, defaultEntry, new Vector2(-1f, -1f), PauseMode.PauseSimulator);
            }

            public static object Show(string titleText, IDictionary<string, object> entries, object defaultEntry, Vector2 position, PauseMode pauseMode)
            {
                if (EnableModalDialogs)
                {
                    using (ComboSelectionDialog comboSelectionDialog = new ComboSelectionDialog(titleText, entries, defaultEntry, position, pauseMode))
                    {
                        comboSelectionDialog.StartModal();
                        return comboSelectionDialog.Result;
                    }
                }
                return defaultEntry;
            }

            public ComboSelectionDialog(string titleText, IDictionary<string, object> entries, object defaultEntry, Vector2 position, PauseMode pauseMode)
                : base("ComboSelectionDialog", 4096, true, pauseMode, null)
            {
                if (mModalDialogWindow == null)
                {
                    return;
                }
                Text text = mModalDialogWindow.GetChildByID(5u, true) as Text;
                if (text != null)
                {
                    text.Caption = titleText;
                }
                mCombo = mModalDialogWindow.GetChildByID(2u, true) as ComboBox;
                foreach (KeyValuePair<string, object> entry in entries)
                {
                    mCombo.ValueList.Add(entry.Key, entry.Value);
                    if (entry.Value as string == defaultEntry as string)
                    {
                        mCombo.CurrentSelection = (uint)(mCombo.ValueList.Count - 1);
                    }
                }
                Rect area = mModalDialogWindow.Area;
                float width = area.BottomRight.x - area.TopLeft.x, height = area.BottomRight.y - area.TopLeft.y, x = position.x, y = position.y;
                if (x < 0f && y < 0f)
                {
                    Rect parentArea = mModalDialogWindow.Parent.Area;
                    x = (float)Math.Round((parentArea.BottomRight.x - parentArea.TopLeft.x - width) / 2f);
                    y = (float)Math.Round((parentArea.BottomRight.y - parentArea.TopLeft.y - height) / 2f);
                }
                area.Set(x, y, x + width, y + height);
                mModalDialogWindow.Area = area;
                mOkButton = mModalDialogWindow.GetChildByID(3u, false) as Button;
                if (mOkButton != null)
                {
                    mOkButton.Click += OnButtonClick;
                }
                mCancelButton = mModalDialogWindow.GetChildByID(4u, false) as Button;
                if (mCancelButton != null)
                {
                    mCancelButton.Click += OnButtonClick;
                }
                OkayID = 3u;
                SelectedID = OkayID;
                CancelID = 4u;
            }

            public void OnButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
            {
                eventArgs.Handled = true;
                EndDialog(sender.ID);
            }

            public override bool OnEnd(uint buttonID)
            {
                if (buttonID == 3)
                {
                    mResult = mCombo.EntryTags[(int)mCombo.CurrentSelection];
                }
                else
                {
                    mResult = null;
                }
                return true;
            }
        }
    }
}