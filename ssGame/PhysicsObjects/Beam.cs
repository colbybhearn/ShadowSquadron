using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;
using Microsoft.Xna.Framework;
using JigLibX.Geometry;
using JigLibX.Collision;
using Microsoft.Xna.Framework.Graphics;

namespace ssGame.PhysicsObjects
{
    public class Beam : Entity
    {
        public float SpeedCurrent = 500;
        public Vector3 YawPitchRoll;
        public Vector3 HeadingCurrent = new Vector3(0, 0, -1);
        public Vector3 OffsetFromCruiser;
        public Beam()
            : base()
        {
            config = new acBeam();
        }

        public Beam(Vector3 position, Vector3 scale, Matrix orient, Model model, int asset)
            : base()
        {
            Vector3 sides = new Vector3(1f * scale.X, 1f * scale.Y, 1f * scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion
            //sides = new Vector3(scale.X * 2.1f, scale.Y * 1.15f, scale.Z * 2.1f);
            //Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -1.45f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Legs
            CommonInit(position, scale / 2, model, true, asset);
            Orientation = orient;
        }

        public void Init(Vector3 pos, Matrix orient)
        {
            Vector3 sides = new Vector3(2f * config.Scale.X, 2f * config.Scale.Y, 3f * config.Scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion
            CommonInit(pos, orient, true);
        }

        public override void FinalizeBody()
        {
            try
            {
                //Vector3 com = SetMass(2.0f);
                //Skin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
                
                body.MoveTo(Position, Orientation);
                SetForwardSpeed();
                //body.EnableBody(); // adds to CurrentPhysicsSystem
                EnableParts();
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine(E.StackTrace);
            }
        }

        internal void SetForwardSpeed()
        {
            this.SetVelocity(Vector3.Normalize(Orientation.Forward) * this.SpeedCurrent);
        }
    }
}
