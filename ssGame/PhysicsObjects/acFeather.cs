using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;

namespace ssGame.PhysicsObjects
{
    public class acFeather : EntityConfig
    {
        public override bool LoadFromFile(string file)
        {
            base.LoadFromFile(file);
            this.Scale = new Microsoft.Xna.Framework.Vector3(2, 2, 2);

            return true;
        }

    }
}
