using System;
using Microsoft.Xna.Framework;

namespace JustSomeGenericShooter.InputBindings
{
    interface IBinding
    {
        void Bind(object input, Action action);
    }
}
