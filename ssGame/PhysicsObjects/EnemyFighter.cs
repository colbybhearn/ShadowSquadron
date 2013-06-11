using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;
using GameHelper.Physics;
using Microsoft.Xna.Framework;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;
using Microsoft.Xna.Framework.Graphics;

namespace ssGame.PhysicsObjects
{
    public class EnemyFighter : Gobject
    {

        /*
         * Purpose
         *  - defend an object
         * 
         * Behavior 
         *  - patrol that object
         *    - stay within range of it
         *    - 
         *  - detect a thread
         *    - some
         *  - decide whether or not to engage
         *    - if threat is within a range, engage
         *  - engage a threat
         *    - target the threat and shoot
         *  - they avoid crashing into objects 
         *    - move around but avoid colliding into anything, based on things' movement and distance
         *  - take evasive action when getting shot
         *    - when hit, change headings drastically
         * 
         */

        public enum Modes
        {
            Patrol,
            Attack,
            Evade
        }

        public Modes mode;

        public float MIN_SPEED = .5f;
        public float MAX_SPEED = 10;
        public float SpeedTarget = 7;
        public float SpeedCurrent = 7;
        public Vector3 YawPitchRoll;
        //public Quaternion Orientation;

        
        


        BoostController VertJet;
        BoostController RotJetY;
        BoostController RotJetX;
        BoostController RotJetZ;
        const float MAX_VERT_MAGNITUDE=30;
        const float MAX_ROT_JET=10;
        public Vector3 PositionTarget = new Vector3(0,0,-1);
        public Vector3 HeadingCurrent = new Vector3(0,0,-1);
        public Vector3 OffsetFromCruiser;

        public EnemyFighter(Vector3 position, Vector3 scale, Matrix orient, Model model, int asset)
            : base()
        {
            Vector3 sides = new Vector3(1f * scale.X, 1.75f * scale.Y, 1f * scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion
            sides = new Vector3(scale.X * 2.1f, scale.Y * 1.15f, scale.Z * 2.1f);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -1.45f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Legs
            CommonInit(position, scale / 2, model, true, asset);

            VertJet = new BoostController(Body, Vector3.Up, Vector3.Zero);
            RotJetX = new BoostController(Body, Vector3.Zero, Vector3.UnitZ);
            RotJetZ = new BoostController(Body, Vector3.Zero, Vector3.UnitX);
            RotJetY = new BoostController(Body, Vector3.Zero, Vector3.UnitY);

            PhysicsSystem.CurrentPhysicsSystem.AddController(VertJet);
            PhysicsSystem.CurrentPhysicsSystem.AddController(RotJetX);
            PhysicsSystem.CurrentPhysicsSystem.AddController(RotJetZ);
            PhysicsSystem.CurrentPhysicsSystem.AddController(RotJetY);

            actionManager.AddBinding((int)Actions.ThrustUp, new GameHelper.Input.ActionBindingDelegate(GenericThrustUp), 1);
            actionManager.AddBinding((int)Actions.Pitch, new GameHelper.Input.ActionBindingDelegate(GenericPitch), 1);
            actionManager.AddBinding((int)Actions.Roll, new GameHelper.Input.ActionBindingDelegate(GenericRoll), 1);
            actionManager.AddBinding((int)Actions.Yaw, new GameHelper.Input.ActionBindingDelegate(GenericYaw), 1);

            mode = Modes.Patrol;
        }

        public enum Actions
        {
            ThrustUp,
            Roll,
            Pitch,
            Yaw
        }

        private void GenericThrustUp(object[] v)
        {
            SetVertJetThrust((float)v[0]);
        }

        private void GenericPitch(object[] v)
        {
            SetRotJetXThrust((float)v[0]);
        }

        private void GenericRoll(object[] v)
        {
            SetRotJetZThrust((float)v[0]);
        }

        private void GenericYaw(object[] v)
        {
            SetRotJetYThrust((float)v[0]);
        }

        public void SetVertJetThrust(float v)
        {
            VertJet.SetForceMagnitude(v * MAX_VERT_MAGNITUDE);
            actionManager.SetActionValues((int)Actions.ThrustUp, new object[] { v });
        }

        public void SetRotJetXThrust(float v)
        {
            RotJetX.SetTorqueMagnitude(v * MAX_ROT_JET);
            actionManager.SetActionValues((int)Actions.Pitch, new object[] { v });
        }

        public void SetRotJetZThrust(float v)
        {
            RotJetZ.SetTorqueMagnitude(v * MAX_ROT_JET);
            actionManager.SetActionValues((int)Actions.Roll, new object[] { v });
        }

        public void SetRotJetYThrust(float v)
        {
            RotJetY.SetTorqueMagnitude(v * MAX_ROT_JET);
            actionManager.SetActionValues((int)Actions.Yaw, new object[] { v });
        }

        public override void SetNominalInput()
        {
            SetVertJetThrust(0);
            SetRotJetXThrust(0);
            SetRotJetYThrust(0);
            SetRotJetZThrust(0);
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
            /*
             * Controller just needs to set the target(s)
             * this function determines how to make the current(s) reach those target(s) and apply them.
             */

            SpeedCurrent += (SpeedTarget - SpeedCurrent) / 100.0f;

            //YawPitchRoll.X = 0;// (float)Math.PI;
            //YawPitchRoll.Y = 0;// (float)Math.PI / 400;
            //YawPitchRoll.Z = (float)Math.PI/4;

            Quaternion Orientation = Quaternion.CreateFromYawPitchRoll(YawPitchRoll.X, YawPitchRoll.Y, YawPitchRoll.Z);
            Matrix mHeadingCurrent = Matrix.CreateFromQuaternion(Orientation);
            this.HeadingCurrent = mHeadingCurrent.Forward;
            this.HeadingCurrent.Normalize();

            Vector3 shouldPointToward = this.PositionTarget - this.BodyPosition();
            shouldPointToward.Normalize();
            float fwDelta = Vector3.Dot(mHeadingCurrent.Forward, shouldPointToward);
            float upDelta = Vector3.Dot(mHeadingCurrent.Up, shouldPointToward);
            float lfDelta = Vector3.Dot(mHeadingCurrent.Left, shouldPointToward);
            float dnDelta = -upDelta;
            float rtDelta = -lfDelta;
            float highest = upDelta;

            
            int closest = 0;
            if (lfDelta > highest)
            {
                highest = lfDelta;
                closest = 1;
            }
            if (dnDelta > highest)
            {
                highest = dnDelta;
                closest = 2;
            }
            if (rtDelta > highest)
            {
                highest = rtDelta;
                closest = 3;
            }

            // applying this is a matter of local vs. world coordinates.

            //switch (closest)
            //{
            //    case 0:
            //        Matrix top = Matrix.CreateRotationX((float)Math.PI/20.0f);
            //        mHeadingCurrent = Matrix.Multiply(mHeadingCurrent, top);
            //        YawPitchRoll = mHeadingCurrent.Forward;
            //        break;
            //    case 1:
            //        Matrix left = Matrix.CreateRotationY((float)Math.PI/20.0f);
            //        mHeadingCurrent = Matrix.Multiply(mHeadingCurrent, left);
            //        YawPitchRoll = mHeadingCurrent.Forward;
            //        break;
            //    case 2:
            //        Matrix bot = Matrix.CreateRotationX((float)Math.PI / 20.0f);
            //        mHeadingCurrent = Matrix.Multiply(mHeadingCurrent, bot);
            //        YawPitchRoll = mHeadingCurrent.Forward;
            //        break;
            //    case 3:
            //        Matrix right = Matrix.CreateRotationY((float)Math.PI / 20.0f);
            //        //Quaternion qrt = Quaternion.CreateFromRotationMatrix(right);
                    
            //        mHeadingCurrent = Matrix.Multiply(mHeadingCurrent, right);
            //        YawPitchRoll = mHeadingCurrent.Forward;
            //        break;
            //    default:
            //        break;
            //}


            float turnRate = (float)Math.PI/100;
            turnRate *= (1.0f - fwDelta);
            
            switch (closest)
            {
                case 0:
                    YawPitchRoll.Y += turnRate;
                    break;
                case 1:
                    YawPitchRoll.X += turnRate;
                    break;
                case 2:
                    YawPitchRoll.Y -= turnRate;
                    break;
                case 3:
                    YawPitchRoll.X -= turnRate;
                    break;
                default:
                    break;
            }

            Orientation =
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, YawPitchRoll.X) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, YawPitchRoll.Y);

            mHeadingCurrent = Matrix.CreateFromQuaternion(Orientation);
            //Orientation = Quaternion.CreateFromYawPitchRoll(YawPitchRoll.X, YawPitchRoll.Y, YawPitchRoll.Z);

            // rotate, pull up , un-rotate
            // turn any way (whether by pitch or yaw, or combo) to head towards the heading.
            // how do I know from update to update what consistent decision to make ? > calculations should answer this.



            // Appy the new current(s)
            this.SetVelocity(this.HeadingCurrent * this.SpeedCurrent);
            this.Orientation = Matrix.CreateFromQuaternion(Orientation);
        }
    }
}
