using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Content;
using kyun;
using kyun.Audio;
using kyun.Utils;
using kyun.GameScreen;

namespace Redux.Utilities.Managers
{
    public class KeyboardManager : GameComponent
    {
        //public variables
        public string Text { get; set; }
        /// <summary>
        /// This represents the amount of milliseconds between key presses that must pass
        /// </summary>
        public int MustPass = 125;

        //private and internal variables
        KeyboardState mainState;
        KeyboardState prevState;
        public bool Enabled { get; set; }
        public delegate void KeyEvents(bool clear = false);
        public event KeyEvents OnKeyPress;
        public event KeyEvents OnKeyDown;
        public event KeyEvents OnKeyUp;

        System.Windows.Forms.Form gameForm;

        public KeyboardManager(Game game) :
            base(game) {
            game.Window.TextInput += Window_TextInput;/*
            gameForm = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(game.Window.Handle);
            if(gameForm != null)
            {
                gameForm.KeyPreview = true;
                gameForm.KeyUp += GameForm_KeyUp;
            }*/

        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (ScreenManager.ActualScreen is not BeatmapScreen)
            {
                return;
            }

            
            if (e.Key == Keys.Back)
            {
                if(Text == null)
                {
                    EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                    return;
                }

                if (Text.Length < 1)
                {
                    EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                    return;
                }

                Text = Text.Remove(Text.Length - 1, 1);
                EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                OnKeyPress?.Invoke();
            }
            else
            {
                int charval = (int)e.Character;

                if (charval != 13)
                {
                    if (char.IsLetterOrDigit(e.Character)
                        || char.IsWhiteSpace(e.Character)
                        || char.IsSymbol(e.Character)
                        || char.IsPunctuation(e.Character)
                        || e.Character == 32
                       ) //??
                    {
                        Text += e.Character;
                    }
                    EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                    OnKeyPress?.Invoke();
                }
            }

           
        }

        private void GameForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!Enabled)
                return;

           if(e.KeyCode == System.Windows.Forms.Keys.Back || e.KeyCode == System.Windows.Forms.Keys.Enter || e.KeyCode == System.Windows.Forms.Keys.Space)
            {

                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.Back:

                        if (Text.Length < 1)
                        {
                            break;
                        }                            

                        Text = Text.Remove(Text.Length - 1, 1);
                        break;
                    case System.Windows.Forms.Keys.Enter:
                        Text += (char)e.KeyCode;
                        break;
                    case System.Windows.Forms.Keys.Space:
                        Text += " ";
                        break;
                }

                //kyun.Audio.AudioPlaybackEngine.Instance.PlaySound(kyun.Utils.SpritesContent.Instance.ButtonOver);
                EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                OnKeyPress?.Invoke();

                return;
            }

            
            if((char.IsLetterOrDigit((char)e.KeyCode) || char.IsPunctuation((char)e.KeyCode) || char.IsWhiteSpace((char)e.KeyCode)))
            {
                if (!(e.KeyCode >= System.Windows.Forms.Keys.D0 && e.KeyCode <= System.Windows.Forms.Keys.Z))
                {
                    return;
                }

                string input = ((char)e.KeyValue).ToString();

                if (e.Shift)
                {
                    input = input.ToUpper();
                }
                else
                {
                    input = input.ToLower();
                }


                Text += input;

                //kyun.Audio.AudioPlaybackEngine.Instance.PlaySound(kyun.Utils.SpritesContent.Instance.ButtonOver);
                EffectsPlayer.PlayEffect(SpritesContent.Instance.ButtonOver);
                OnKeyPress?.Invoke();
            }

            
        }

        private void GameForm_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {


        }

        DateTime prevUpdate = DateTime.Now;
        public override void Update(GameTime gameTime)
        {

           
        }

        public string Convert(Keys[] keys)
        {
            
            string output = "";
            bool usesShift = (keys.Contains(Keys.LeftShift) || keys.Contains(Keys.RightShift));
            if(keys.Length > 0){
                if (OnKeyPress != null)
                {
                    OnKeyPress();
                }
            }
            foreach (Keys key in keys)
            {
               

                //thanks SixOfEleven @ DIC
                if (prevState.IsKeyUp(key))
                    continue;

                if (key >= Keys.A && key <= Keys.Z)
                    output += key.ToString();
                else if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                    output += ((int)(key - Keys.NumPad0)).ToString();
                else if (key >= Keys.D0 && key <= Keys.D9)
                {
                    string num = ((int)(key - Keys.D0)).ToString();
                    #region special num chars
                    if (usesShift)
                    {
                        switch (num)
                        {
                            case "1":
                                {
                                    num = "!";
                                }
                                break;
                            case "2":
                                {
                                    num = "@";
                                }
                                break;
                            case "3":
                                {
                                    num = "#";
                                }
                                break;
                            case "4":
                                {
                                    num = "$";
                                }
                                break;
                            case "5":
                                {
                                    num = "%";
                                }
                                break;
                            case "6":
                                {
                                    num = "^";
                                }
                                break;
                            case "7":
                                {
                                    num = "&";
                                }
                                break;
                            case "8":
                                {
                                    num = "*";
                                }
                                break;
                            case "9":
                                {
                                    num = "(";
                                }
                                break;
                            case "0":
                                {
                                    num = ")";
                                }
                                break;
                            default:
                                //wtf?
                                break;
                        }
                    }
                    #endregion
                    output += num;
                }
                else if (key == Keys.OemPeriod)
                    output += ".";
                else if (key == Keys.OemTilde)
                    output += "'";
                else if (key == Keys.Space)
                    output += " ";
                else if (key == Keys.OemMinus)
                    output += "-";
                else if (key == Keys.OemPlus)
                    output += "+";
                else if (key == Keys.OemQuestion && usesShift)
                    output += "?";
                else if (key == Keys.Back) //backspace
                    output += "\b";

                if (!usesShift) //shouldn't need to upper because it's automagically in upper case
                    output = output.ToLower();
            }
            return output;
        }
    }
}


