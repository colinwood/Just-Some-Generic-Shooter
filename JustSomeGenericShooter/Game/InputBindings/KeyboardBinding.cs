using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JustSomeGenericShooter.InputBindings
{
    class KeyboardBinding : GameComponent, IBinding
    {
        private readonly Dictionary<Keys, Action> bindMap; 
        public KeyboardBinding(Game game) : base(game)
        {
            bindMap = new Dictionary<Keys, Action>();
        }

        public void Bind(object input, Action action)
        {
            var @in = (Keys)input;
            bindMap.Add(@in, action);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var bind in bindMap.Where(bind => Keyboard.GetState().GetPressedKeys().Contains(bind.Key)))
            {
                bind.Value();
            }
            base.Update(gameTime);
        }
    }
}
