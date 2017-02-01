﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using ubeat.Beatmap;
using ubeat.Utils;

namespace ubeat.GameScreen
{
    public class ScreenBase : IScreen, IDisposable
    {

        public delegate void KeyEventHandler(object sender, InputEvents.KeyPressEventArgs args);
        public event KeyEventHandler onKeyPress;

        private KeyboardState keyboardOldState;

        public ScreenBase(string name = "BaseScreen")
        {
            keyboardOldState = Keyboard.GetState();
            ActualScreenMode = Screen.ScreenModeManager.GetActualMode();
            Name = name;
            OnLoad += _OnLoad;
            Controls = new List<UIObjectBase>();
        }

        internal void RenderBg()
        {
            if (Background != null)
            {
                int screenWidth = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = UbeatGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                var screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);

                UbeatGame.Instance.SpriteBatch.Draw(Background, screenRectangle, null, Microsoft.Xna.Framework.Color.White, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
            } 

            RenderPeak();
        }

        internal void RenderObjects()
        {
            try
            {
                foreach (UIObjectBase obj in Controls)
                    obj.Render();
            }
            catch
            {

            }
        }

        public virtual void Render()
        {
            if (!Visible || isDisposing) return;

            RenderBg();

            RenderObjects();

        }



        public virtual void Update(GameTime tm)
        {
            if (!Visible || isDisposing) return;

            KeyboardState actualState = Keyboard.GetState();

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                EscapeAlredyPressed = true;

            if (keyboardState.IsKeyUp(Keys.Escape))
            {
                if (EscapeAlredyPressed)
                {
                    EscapeAlredyPressed = false;
                    OnBackSpacePress?.Invoke(this, new EventArgs());
                }
            }

            checkKeyboardEvents(keyboardOldState, actualState);

            UpdateControls();

            UpdatePeak();

            actualState = keyboardOldState;
        }

        private void checkKeyboardEvents(KeyboardState kbOldState, KeyboardState kbActualState)
        {
            Keys[] currentPressedKeys = kbActualState.GetPressedKeys();
            Keys[] oldPressedKeys = kbOldState.GetPressedKeys();


            if (currentPressedKeys.Length < 1 && oldPressedKeys.Length < 1)
                return;

            foreach (Keys aKey in currentPressedKeys)
            {
                if (kbOldState.IsKeyUp(aKey))
                {
                    onKeyPress?.Invoke(this, new InputEvents.KeyPressEventArgs{
                        Key = aKey
                    });
                }
            }
        }

        internal void RenderPeak()
        {
            UbeatGame.Instance.SpriteBatch.Draw(SpritesContent.Instance.TopEffect, new Rectangle(0, 0, ActualScreenMode.Width, ActualScreenMode.Height), Color.White * (peak * .7f / 10f));

        }

        internal void UpdatePeak()
        {
            //Experimental
            float pScale = UbeatGame.Instance.Player.PeakVol;
            if (pScale > 1) pScale = 1;
            if (pScale > 0.7f)
                pScale = 1;
            else
                pScale = 0f;

            if (pScale < peak) pScale = peak;

            peak = pScale;
            if (peak > 0f)
            {
                peak -= (float)(UbeatGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f);
            }
        }

        internal virtual void UpdateControls()
        {
            foreach (UIObjectBase obj in Controls)
                obj.Update();
        }

        public void BackPressed(IScreen screen)
        {
            ScreenManager.ChangeTo(screen);
        }        

        public void OnLoadScreen()
        {
            OnLoad?.Invoke(this, new EventArgs());
        }

        private void _OnLoad(object sender, EventArgs e)
        {

#if DEBUG
            Logger.Instance.Debug($"{this.Name} loaded");
#endif
        }

        private void _OnUnload(object sender, EventArgs e)
        {
#if DEBUG
            Logger.Instance.Debug($"{this.Name} unloaded");
#endif
        }

        public void Dispose()
        {
            isDisposing = true;
        }

        public void ChangeBackground(string backgroundPath)
        {
            //if (!backgroundPath.EndsWith(".png") && !backgroundPath.EndsWith(".jpg"))
            //    return;

            try
            {
                using (var fs = new FileStream(backgroundPath, FileMode.Open, FileAccess.Read))
                {
                    Background = Texture2D.FromStream(UbeatGame.Instance.GraphicsDevice, fs);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Logger.Instance.Warn("There was a problem loading the background: {0}", ex.Message);
                Logger.Instance.Warn("StackTrace: {0}", ex.StackTrace);
#else
                Logger.Instance.Warn("There was a problem loading the background");
#endif
                // Use a default bgs n stuff class
                Background = SpritesContent.Instance.DefaultBackground;
            }
        }

        public virtual void ChangeBeatmapDisplay(ubeatBeatMap bm)
        {
            if (UbeatGame.Instance.SelectedBeatmap.SongPath != bm.SongPath)
            {
                UbeatGame.Instance.Player.Play(bm.SongPath);
                UbeatGame.Instance.Player.soundOut.Volume = UbeatGame.Instance.GeneralVolume;
            }

            UbeatGame.Instance.SelectedBeatmap = bm;

            ChangeBackground(bm.Background);
        }

        internal float peak = 0;
        internal Screen.ScreenMode ActualScreenMode;

        private bool EscapeAlredyPressed;

        public bool isDisposing;
        public Texture2D Background { get; set; }
        public List<UIObjectBase> Controls { get; set; }
        public float Opacity { get; set; }
        public IScreen ScreenInstance { get; set; }
        public bool Visible { get; set; }
        public string Name { get; set; }
        public event EventHandler OnLoad;
        public event EventHandler OnBackSpacePress;
    }
}