﻿using System;
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
    public class Feather : Gobject
    {
        public float SPEED_MIN = .5f;
        public float SPEED_MAX = 10;
        public float SpeedTarget = 7;
        public float SpeedCurrent = 7;
        public float SpeedChangeRate = 1;
        public Vector3 YawPitchRoll;
        public Vector3 YawPitchRollRate = new Vector3((float)Math.PI / 2000.0f, (float)Math.PI / 2000.0f, (float)Math.PI / 2000.0f);
        public Vector3 HeadingCurrent = new Vector3(0, 0, -1);
        public Vector3 OffsetFromCruiser;

        public Feather(Vector3 position, Vector3 scale, Matrix orient, Model model, int asset)
            : base()
        {
            Vector3 sides = new Vector3(1f * scale.X, 1.75f * scale.Y, 1f * scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion
            sides = new Vector3(scale.X * 2.1f, scale.Y * 1.15f, scale.Z * 2.1f);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -1.45f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Legs
            CommonInit(position, scale / 2, model, true, asset);

        }

        public enum Actions
        {
            Thrust,
            Roll,
            Pitch,
            Yaw
        }

        public void IncreseSpeed()
        {
            SpeedTarget += SpeedChangeRate;
            Microsoft.Xna.Framework.MathHelper.Clamp(SpeedTarget, SPEED_MIN, SPEED_MAX);
        }
        public void DecreaseSpeed()
        {
            SpeedTarget -= SpeedChangeRate;
            Microsoft.Xna.Framework.MathHelper.Clamp(SpeedTarget, SPEED_MIN, SPEED_MAX);
        }
        public void RollLeft()
        {
            YawPitchRoll.Z += YawPitchRollRate.Z;
        }
        public void RollRight()
        {
            YawPitchRoll.Z -= YawPitchRollRate.Z;
        }
        public void PitchUp()
        {
            YawPitchRoll.Y += YawPitchRollRate.Y;
        }
        public void PitchDown()
        {
            YawPitchRoll.Y -= YawPitchRollRate.Y;
        }
        public void YawLeft()
        {
            YawPitchRoll.X -= YawPitchRollRate.X;
        }
        public void YawRight()
        {
            YawPitchRoll.X -= YawPitchRollRate.X;
        }
        public override void FinalizeBody()
        {
            try
            {
                //Vector3 com = SetMass(2.0f);
                //Skin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
                Body.MoveTo(Position, Matrix.Identity);
                Body.EnableBody(); // adds to CurrentPhysicsSystem
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine(E.StackTrace);
            }
        }

        public void Update()
        {
            //SpeedCurrent += (SpeedTarget - SpeedCurrent) / 100.0f;
            //Quaternion Orientation = Quaternion.CreateFromYawPitchRoll(YawPitchRoll.X, YawPitchRoll.Y, YawPitchRoll.Z);
            //Quaternion Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, YawPitchRoll.Z) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, YawPitchRoll.Y);
            //Quaternion Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, YawPitchRoll.Y) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, YawPitchRoll.Z);

            Matrix current = Orientation;
            //strange ness
            Orientation *= Matrix.CreateFromAxisAngle(current.Forward, YawPitchRoll.Z);
            Orientation *= Matrix.CreateFromAxisAngle(current.Right, YawPitchRoll.Y);
            YawPitchRoll = new Vector3(0, 0, 0);

            //HeadingCurrent = Orientation.Forward;
            //HeadingCurrent.Normalize();

            // Appy the new current(s)
            //this.SetVelocity(this.HeadingCurrent * this.SpeedCurrent);
            //this.Orientation = Matrix.CreateFromQuaternion(Orientation);
        }
    }
}
