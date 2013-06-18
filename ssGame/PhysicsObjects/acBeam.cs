using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;

namespace ssGame.PhysicsObjects
{
    public class acBeam : EntityConfig
    {
        public override bool LoadFromFile(string file)
        {
            base.LoadFromFile(file);
            this.Scale = new Microsoft.Xna.Framework.Vector3(.3f, .3f, .5f);

            return true;
        }
    }
}
