using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;
using ssGame.PhysicsObjects;
using Microsoft.Xna.Framework;
using GameHelper.Physics.PhysicsObjects;
using Microsoft.Xna.Framework.Graphics;

namespace ssGame
{
    class Assets
    {
        

        public static Gobject CreateFighter()
        {
            Vector3 scale = new Vector3(2, 2, 2);
            EnemyFighter f = new EnemyFighter(
                Vector3.Zero,
                scale,
                Matrix.Identity,
                null,
                0
                );

            return f;
        }

        public static Gobject CreateCruiser()
        {
            Vector3 scale = new Vector3(2, 2, 2);
            EnemyCruiser c = new EnemyCruiser(
                Vector3.Zero,
                scale,
                Matrix.Identity,
                null,
                0
                );

            return c;
        }

        public static Gobject CreateFeather()
        {
            Vector3 scale = new Vector3(2, 2, 2);
            Feather f = new Feather(
                Vector3.Zero,
                scale,
                Matrix.Identity,
                null,
                0
                );

            return f;
        }

        internal static Beam CreateBeam()
        {
            Vector3 scale = new Vector3(2, 2, 2);
            Beam f = new Beam(
                Vector3.Zero,
                scale,
                Matrix.Identity,
                null,
                0
                );

            return f;
        }
    }
}
