using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper.Objects;
using ssGame.PhysicsObjects;
using Microsoft.Xna.Framework;
using Helper.Physics.PhysicsObjects;
using Microsoft.Xna.Framework.Graphics;

namespace ssGame
{
    class Assets
    {
        public enum Types
        {
            EnemyFighter,
            EnemyCruiser,
            EnemyCrystal,
            Feather1, 
            Feather2,
        }

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
