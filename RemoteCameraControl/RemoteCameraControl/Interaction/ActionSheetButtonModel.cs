using System;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public class ActionSheetButtonModel
    {
        public string Text { get; private set; }
        public Action Action { get; private set; }
        public string IconName { get; private set; }

        public ActionSheetButtonModel(string text, Action action, string iconName)
        {
            Text = text;
            Action = action;
            IconName = iconName;
        }
    }
}