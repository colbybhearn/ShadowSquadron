using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;

namespace ssGame.PhysicsObjects
{
    public class acBeam : AssetConfig
    {
        public override bool LoadFromFile(string file)
        {
            base.LoadFromFile(file);
            this.Scale = new Microsoft.Xna.Framework.Vector3(.1f, .1f, .1f);

            return true;
        }
    }
}
