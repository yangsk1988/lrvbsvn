using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace VirtualBicycle.UI
{
    public class GameUI : UnmanagedResource, IDisposable
    {
        #region Fields

        public Device Device
        {
            get;
            private set;
        }

        Sprite UISprite
        {
            get;
            set;
        }

        Stack<UIComponent> uiStack;

        private UIComponent currentComponent;
        public UIComponent CurrentComponent
        {
            get { return currentComponent; }
            set
            {
                if (currentComponent != value)
                {
                    if (currentComponent != null)
                    {
                        if (currentComponent.IsStacked)
                        {
                            uiStack.Push(currentComponent);
                        }
                    }
                    else 
                    {
                        uiStack.Push(null);
                    } 
                    
                    SetCurrentComponent(value);

                    currentComponent = value;
                }
            }
        }

        void SetCurrentComponent(UIComponent value)
        {
            if (value != null)
            {
                value.NotifyActivated();
            }
            if (currentComponent != null)
            {
                currentComponent.NotifyDeactivated();
            }
            currentComponent = value;
        }

        private Game game;


        public bool Pop()
        {
            if (uiStack.Count > 0)
            {
                SetCurrentComponent(uiStack.Pop());
                return true;
            }
            return false;
        }

        #endregion

        #region Construtor
        public GameUI(Device device, Game game)
        {
            this.Device = device;
            this.game = game;
            this.uiStack = new Stack<UIComponent>();

            LoadUnmanagedResources();
        }
        #endregion

        #region Render
        public void Render()
        {
            UISprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.DoNotSaveState);

            if (currentComponent != null)
            {
                currentComponent.Render(UISprite);
            }

            //font.DrawString(GetSprite, fpsc.ToString(), 5, 5, -1);

            UISprite.End();

            if (currentComponent != null)
            {
                currentComponent.Render();
            }
        }
        #endregion

        #region Update
        public void Update(float dt)
        {
            if (currentComponent != null)
            {
                currentComponent.Update(dt);
            }

            //fpsc.Update();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                UnloadUnmanagedResources();
            }
        }

        protected override void loadUnmanagedResources()
        {
            this.UISprite = new Sprite(Device);
        }

        protected override void unloadUnmanagedResources()
        {
            this.UISprite.Dispose();
        }

    }
}
