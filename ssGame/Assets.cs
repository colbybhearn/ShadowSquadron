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
        

        public static Gobject CreateEnemyFighter(Model landerModel)
        {
            Vector3 scale = new Vector3(2, 2, 2);
            EnemyFighter lander = new EnemyFighter(
                Vector3.Zero,
                scale,
                Matrix.Identity,
                landerModel,
                0
                );

            return lander;
        }
    }
}
