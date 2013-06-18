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
    public class EnemyFighter : Entity
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

        public float SPEED_MIN = .5f;
        public float SPEED_MAX = 10;
        public float SpeedTarget = 7;
        public float SpeedCurrent = 7;


        public Vector3 YawPitchRoll;

        
        public Vector3 PositionTarget = new Vector3(0,0,-1);
        public Vector3 HeadingCurrent = new Vector3(0,0,-1);
        public Vector3 OffsetFromCruiser;


        public EnemyFighter()
            : base()
        {
            config = new acFighter();
        }

        public EnemyFighter(Vector3 position, Vector3 scale, Matrix orient, Model model, int asset)
            : base()
        {
            Vector3 sides = new Vector3(.4f * scale.X, .15f * scale.Y, .5f * scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion
            //sides = new Vector3(scale.X * 2.1f, scale.Y * 1.15f, scale.Z * 2.1f);
            //Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -1.45f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Legs
            CommonInit(position, scale / 2, model, true, asset);

            mode = Modes.Patrol;
        }

        public void Init(Vector3 pos, Matrix orient)
        {
            Vector3 sides = new Vector3(10f * config.Scale.X, 1.5f * config.Scale.Y, 6f * config.Scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion            
            CommonInit(pos, orient, true);
        }
        public enum Actions
        {
            ThrustUp,
            Roll,
            Pitch,
            Yaw
        }

        public override void FinalizeBody()
        {
            try
            {
                //Vector3 com = SetMass(2.0f);
                //Skin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
                body.MoveTo(Position, Matrix.Identity);
                EnableParts(); // adds to CurrentPhysicsSystem
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
