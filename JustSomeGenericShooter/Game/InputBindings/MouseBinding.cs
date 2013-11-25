using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JustSomeGenericShooter.InputBindings
{
    public enum MState
    {
        Left,
        Right, 
        Middle,
        XAxis,
        YAxis
    }

    public class MouseBinding : GameComponent, IBinding
    {
        private readonly Dictionary<MState, Action> bindMap;

        public MouseBinding(Game game) : base(game)
        {
            bindMap = new Dictionary<MState, Action>();
        }

        public void Bind(object button, Action action)
        {
            var btn = (MState) button;
            bindMap.Add(btn, action);
        }

        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && bindMap.ContainsKey(MState.Left))
                bindMap[MState.Left]();
            if (Mouse.GetState().RightButton == ButtonState.Pressed && bindMap.ContainsKey(MState.Right))
                bindMap[MState.Right]();
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed && bindMap.ContainsKey(MState.Middle))
                bindMap[MState.Middle]();
            if (bindMap.ContainsKey(MState.XAxis))
                bindMap[MState.XAxis]();
            if (bindMap.ContainsKey(MState.YAxis))
                bindMap[MState.YAxis]();
        }
    }
}
