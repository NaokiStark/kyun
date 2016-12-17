using System;
using ubeat.GameScreen;

namespace ubeat.Dialog
{
    public class DialogLayer : ScreenBase
    {
        static DialogLayer instance = null;

        public static DialogLayer Instance {
            get
            {
                if (instance == null)
                    instance = new DialogLayer();

                return instance;
            }
        }

        public DialogLayer()
        {
            
        }
        
        void ShowDialog()
        {

        }

        public void Confirm()
        {

        }

    }

    public class DialogMessage
    {
        string Message { get; set; }
        MessageButtons Buttons { get; set; }
    }

    /// <summary>
    /// Message Buttons
    /// </summary>
    public enum MessageButtons
    {
        /// <summary>
        /// Ok button
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Accept Button
        /// </summary>
        Accept = 1,

        /// <summary>
        /// Yes - No buttons
        /// </summary>
        YesNo = 2,

        /// <summary>
        /// Accept - Cancel buttons
        /// </summary>
        AcceptCancel = 3,

        /// <summary>
        /// Custom buttos (Not Implemented yet)
        /// </summary>
        Custom = 4
    }

}
