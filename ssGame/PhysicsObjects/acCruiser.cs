using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;

namespace ssGame.PhysicsObjects
{
    public class acCruiser : AssetConfig
    {
        public override bool LoadFromFile(string file)
        {
            base.LoadFromFile(file);
            this.Scale = new Microsoft.Xna.Framework.Vector3(5, 5, 5);

            return true;
        }
    }
}
