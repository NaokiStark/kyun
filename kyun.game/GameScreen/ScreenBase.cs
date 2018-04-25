using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using kyun.Beatmap;
using kyun.Utils;
using kyun.Video;
using kyun.GameScreen.UI;

namespace kyun.GameScreen
{
    public class ScreenBase : IScreen, IDisposable
    {

        bool rightBgEffect;
        int rightEffCount;
        bool bgEffect;

        public delegate void KeyEventHandler(object sender, InputEvents.KeyPressEventArgs args);
        public event KeyEventHandler onKeyPress;

        public event TouchHandler.TouchEventHandler onTouch;

        private KeyboardState keyboardOldState;

        private event EventHandler onFadeInEnd;
        private event EventHandler onFadeOutEnd;
        public event ScrollEventHandler OnScroll;

        public event EventHandler OnMouseMove;

        Vector2 lastMousePosition = Vector2.Zero;

        public delegate void ScrollEventHandler(object sender, bool Up, bool touch = false);

        public static AudioVideoPlayer AVPlayer
        {
            get
            {
                if (avp == null)
                    avp = new AudioVideoPlayer();

                return avp;
            }
        }

        internal static AudioVideoPlayer avp;

        
        public bool AllowVideo { get; set; }

        private FilledRectangle backgroundTransitionLayer;


        public ScreenBase(string name = "BaseScreen")
        {

            avp = new AudioVideoPlayer();
            rPeak = true;
            keyboardOldState = Keyboard.GetState();
            ActualScreenMode = Screen.ScreenModeManager.GetActualMode();
            Name = name;
            OnLoad += _OnLoad;
            Controls = new List<UIObjectBase>();
            KyunGame.Instance.touchHandler.onTouchScreen += ScreenBase_onTouch;
            BackgroundBeat = new UI.Image(SpritesContent.Instance.TopEffect) {
                Position = Vector2.Zero,
                                
            };
            BackgroundFlash = new FilledRectangle(new Vector2(ActualScreenMode.Width, ActualScreenMode.Height), Color.White * .05f);
            BackgroundDim = 1;

            backgroundTransitionLayer = new FilledRectangle(new Vector2(ActualScreenMode.Width, ActualScreenMode.Height), Color.Black)
            {
                Opacity = 0
            };


            OnLoad += ScreenBase_OnLoad;

            onKeyPress += ScreenBase_onKeyPress;


        }

        private void ScreenBase_onKeyPress(object sender, InputEvents.KeyPressEventArgs args)
        {
            if(args.Key == Keys.F11)
            {
                KyunGame.Instance.ToggleFullscreen(!KyunGame.Instance.Graphics.IsFullScreen);
            }
        }

        private void ScreenBase_OnLoad(object sender, EventArgs e)
        {

           // Controls.Add(BackgroundBeat);
        }

        private void ScreenBase_onTouch(object sender, ubeatTouchEventArgs e)
        {
            if(Visible)
                onTouch?.Invoke(this, e);
        }

        internal void RenderBg()
        {
            if (Background != null)
            {
                int screenWidth = KyunGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
                int screenHeight = KyunGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

                var screenRectangle = new Rectangle(screenWidth / 2, screenHeight / 2, (int)(((float)Background.Width / (float)Background.Height) * (float)screenHeight), screenHeight);
                try
                {
                    if(Background != null && !Background.IsDisposed)
                        KyunGame.Instance.SpriteBatch.Draw(Background, screenRectangle, null, Microsoft.Xna.Framework.Color.White * BackgroundDim, 0, new Vector2(Background.Width / 2, Background.Height / 2), SpriteEffects.None, 0);
                }
                catch
                {
                    //
                }
                
            }

            if (AllowVideo)
            {
                AVPlayer.Render();
            }
            
            if (rPeak)
            {
                RenderPeak();
            }

            if(backgroundTransitionLayer.Opacity > 0.01)
            {
                backgroundTransitionLayer.Render();
            }
            
        }

        internal void updateBgTransition()
        {
            if (!inTransition)
                return;

            float transitionDelay = 0.003f;

            if (fadeInBg)
            {
                float sum = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * transitionDelay;
                if (sum + backgroundTransitionLayer.Opacity >= 1)
                {
                    backgroundTransitionLayer.Opacity = 1;
                    inTransition = false;
                    onFadeInEnd?.Invoke(this, new EventArgs());
                    
                    return; //Wait for next update
                }                    

                backgroundTransitionLayer.Opacity += sum;
            }
            else
            {
                float diff = KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * (transitionDelay - 0.002154f);

                if(backgroundTransitionLayer.Opacity - diff < 0.001)
                {
                    backgroundTransitionLayer.Opacity = 0;
                    fadeInBg = true;
                    inTransition = false;
                    onFadeOutEnd?.Invoke(this, new EventArgs());
                    return;
                }

                backgroundTransitionLayer.Opacity -= diff;
            }
        }

        internal virtual void RenderObjects()
        {
            try
            {
                foreach (UIObjectBase obj in Controls) {
                    if (obj.Texture != null)
                    {
                        if (obj.Texture == SpritesContent.Instance.TopEffect)
                        {
                            continue;
                        }
                        else
                        {
                            obj.Render();
                        }
                    }
                    else
                    {
                        obj.Render();
                    }
                }
                    
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

            if (actualState.IsKeyDown(Keys.Escape))
                EscapeAlredyPressed = true;

            if (actualState.IsKeyUp(Keys.Escape))
            {
                if (EscapeAlredyPressed)
                {
                    EscapeAlredyPressed = false;
                    OnBackSpacePress?.Invoke(this, new EventArgs());
                }
            }

            checkKeyboardEvents(keyboardOldState, actualState);
            if (AllowVideo)
            {
                AVPlayer.Update();
            }
            
            UpdateScroll();

            UpdateControls();

            UpdatePeak();

            updateBgTransition();

            Vector2 actualPos = MouseHandler.GetState().Position;

            if (!actualPos.Equals(lastMousePosition))
            {
                OnMouseMove?.Invoke(this, new EventArgs());
            }

            lastMousePosition = actualPos;
        }

        internal virtual void UpdateScroll()
        {
            //Only invoke this if defined
            if (OnScroll != null)
            {
                MouseEvent mouseState = MouseHandler.GetState();
                Rectangle cursor = new Rectangle((int)mouseState.X, (int)mouseState.Y, 1, 1);
                Rectangle rg = new Rectangle(0, 0, ActualScreenMode.Width, ActualScreenMode.Height);


                int actualScrollVal = mouseState.ScrollWheelValue;
                TouchHandler touch = KyunGame.Instance.touchHandler;
                List<TouchPoint> touchPoints = touch.GetAllPointsIntersecs(rg);

                if (actualScrollVal > lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {
                        OnScroll?.Invoke(this, true);
                    }

                }
                else if (actualScrollVal < lastScrollVal)
                {
                    if (cursor.Intersects(rg))
                    {

                        OnScroll?.Invoke(this, false);

                    }
                }
                else if (touchPoints.Count > 0)
                {


                    TouchPoint first = touchPoints[0];
                    if (!alredyTouched)
                    {
                        lastScrollTouchValue = (int)first.Location.Y;
                        alredyTouched = true;
                        scrollInvoked = false;
                    }
                    else
                    {
                        if ((int)first.Location.Y > lastScrollTouchValue + 50)
                        {
                            lastScrollTouchValue = (int)first.Location.Y;
                            mouseEventsCancelled = true;
                            OnScroll?.Invoke(this, true, true);
                            scrollInvoked = true;

                        }
                        else if ((int)first.Location.Y < lastScrollTouchValue - 50)
                        {
                            lastScrollTouchValue = (int)first.Location.Y;
                            mouseEventsCancelled = true;
                            OnScroll?.Invoke(this, false, true);
                            scrollInvoked = true;
                        }

                    }

                }
                else if (touchPoints.Count < 1)
                {

                    alredyTouched = false;
                }

                if (mouseEventsCancelled && scrollInvoked)
                {
                    return;
                }

                lastScrollVal = actualScrollVal;
            }
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

            keyboardOldState = kbActualState;
        }

        internal void RenderPeak()
        {

            Screen.ScreenMode actmode = Screen.ScreenModeManager.GetActualMode();
            KyunGame.Instance.SpriteBatch.Draw(BackgroundBeat.Texture, new Rectangle(0, 0, actmode.Width, actmode.Height), null, Color.White * BackgroundBeat.Opacity * .8f, BackgroundBeat.AngleRotation, Vector2.Zero, (!rightBgEffect)?SpriteEffects.None:SpriteEffects.FlipHorizontally, 0);
            BackgroundFlash.Render();
        }

        internal void UpdatePeak()
        {
            //Experimental
            float pScale = KyunGame.Instance.Player.PeakVol;
            if (pScale > 1) pScale = 1;
            if (pScale > (InstanceManager.MaxPeak  * 99.999995f /100f))
                pScale = 1;
            else
                pScale = 0f;

            if (pScale < peak) pScale = peak;

            peak = pScale;

            if(KyunGame.Instance.Player.PeakVol >= InstanceManager.MaxPeak - 0.001)
            {
                peaked = true;
            }

            if (peak > 0f)
            {
                peak -= (float)(KyunGame.Instance.GameTimeP.ElapsedGameTime.TotalMilliseconds * 0.001f);
            }

            BackgroundFlash.Opacity = BackgroundBeat.Opacity = Math.Max(BackgroundBeat.Opacity - ((float)KyunGame.Instance.GameTimeP.ElapsedGameTime.Milliseconds * 0.003f),0f);

            if (peaked && BackgroundBeat.Opacity == 0)
            {
                rightBgEffect = !rightBgEffect;
                BackgroundBeat.Opacity = .8f;

                peaked = false;
            }
        }

        internal virtual void UpdateControls()
        {
            //List<ComboBox> cmbs = new List<ComboBox>();

            foreach(UIObjectBase obj in Controls)
            {
                if (!(obj is ComboBox))
                    continue;

                ComboBox cobj = (ComboBox)obj;

                if (cobj.IsListVisible)
                {
                    cobj.Update();
                    return;
                }
                    
            }

            
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

        public void ChangeBackground(Texture2D tx)
        {
            if(tx!=null)
            {
                Background = tx;
            }
            else
            {
                Background = SpritesContent.Instance.DefaultBackground;
            }
            
        }

        public void ChangeBackground(string backgroundPath)
        {
            //if (!backgroundPath.EndsWith(".png") && !backgroundPath.EndsWith(".jpg"))
            //    return;

            

            if (!File.Exists(backgroundPath))
            {
                Background = SpritesContent.Instance.DefaultBackground;
                return;
            }

            FileAttributes attr = File.GetAttributes(backgroundPath);

            if (!attr.HasFlag(FileAttributes.Directory))
            {
                try
                {
                    using (var fs = new FileStream(backgroundPath, FileMode.Open, FileAccess.Read))
                    {
                        Background = ContentLoader.FromStream(KyunGame.Instance.GraphicsDevice, fs);
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
            else
            {
                Background = SpritesContent.Instance.DefaultBackground;
            }


        }

        public virtual void ChangeBeatmapDisplay(ubeatBeatMap bm, bool overrideBg = true)
        {
            onFadeInEnd = null;
            inTransition = true;
            fadeInBg = true;


            onFadeInEnd = (e, ar) => {

                if (overrideBg)
                {
                    ChangeBackground(bm.Background);
                }

                /*
                if (KyunGame.Instance.SelectedBeatmap == null)
                {
                    AVPlayer.Play(bm.SongPath, bm.Video, true);
                }
                else if (KyunGame.Instance.SelectedBeatmap.SongPath != bm.SongPath)
                {
                   
                }*/
                AVPlayer.Play(bm.SongPath, bm.Video, true);

               

                fadeInBg = false;
                inTransition = true;
            };
            inTransition = true;
        }

        internal float peak = 0;
        internal Screen.ScreenMode ActualScreenMode;

        private bool EscapeAlredyPressed;
        private UI.Image BackgroundBeat;
        private UI.FilledRectangle BackgroundFlash;

        public bool isDisposing;
        private bool peaked;
        public bool inTransition;
        private bool fadeInBg;
        private int lastScrollVal;
        private bool alredyTouched;
        private int lastScrollTouchValue;
        private bool mouseEventsCancelled;
        private bool scrollInvoked;

        public float BackgroundDim { get; set; }
        public Texture2D Background { get; set; }
        public List<UIObjectBase> Controls { get; set; }
        public float Opacity { get; set; }
        public IScreen ScreenInstance { get; set; }
        public bool Visible { get; set; }
        public string Name { get; set; }
        public bool rPeak { get; set; }

        public event EventHandler OnLoad;
        public event EventHandler OnBackSpacePress;
    }
}