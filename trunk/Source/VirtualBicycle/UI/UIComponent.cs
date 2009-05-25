using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace VirtualBicycle.UI
{
    public abstract class UIComponent
    {
        public Game Game
        {
            get;
            private set;
        }

        protected UIComponent(Game game)
        {
            this.Game = game;
        }
        /// <summary>
        /// 获取一个值，表示是否已经加载完毕
        /// </summary>
        public bool IsLoaded
        {
            get;
            private set;
        }

        public virtual bool IsStacked
        {
            get { return true; }
        }

        protected abstract void update(float dt);

        public void Update(float dt)
        {
            if (!IsLoaded)
            {
                Load();
            }
            update(dt);
        }

        public void Render()
        {
            if (!IsLoaded)
            {
                Load();
            }
            render();
        }
        protected abstract void render();
        
        public void Render(Sprite sprite)
        {
            if (!IsLoaded)
            {
                Load();
            }
            render(sprite);
        }
        protected abstract void render(Sprite sprite);


        public virtual void NotifyActivated() { }
        public virtual void NotifyDeactivated() { }

        protected abstract void load();
        protected abstract void unload();

        public void Load()
        {
            if (!IsLoaded)
            {
                load();
                IsLoaded = true;
            }
        }
        public void Unload()
        {
            if (IsLoaded)
            {
                unload();
                IsLoaded = false;
            }
        }

        public float GetPositionWidth(float x)
        {
            const float defaultWidth = 1024f;
            return x / defaultWidth * Game.Window.ClientSize.Width;
        }

        public float GetPositionHeight(float x)
        {
            const float defaultHeight = 768f;
            return x / defaultHeight * Game.Window.ClientSize.Height;
        }

        public Vector2 GetPosition(Vector2 pos)
        {
            const float defaultWidth = 1024f;
            const float defaultHeight = 768f;
            float x = pos.X / defaultWidth * Game.Window.ClientSize.Width;
            float y = pos.Y / defaultHeight * Game.Window.ClientSize.Height;
            return new Vector2(x, y);
        }
    }
}
