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
    public class EnemyCruiser : Entity
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

        public float MIN_SPEED = .5f;
        public float MAX_SPEED = 10;
        public float SpeedTarget = 4;
        public float SpeedCurrent = 6;
        public Vector3 YawPitchRoll;

        public Modes mode;

        public float NominalSpeed;

        public EnemyCruiser()
            : base()
        {
            config = new acCruiser();
        }

        public EnemyCruiser(Vector3 position, Vector3 scale, Matrix orient, Model model, int asset)
            : base()
        {
            Vector3 sides = new Vector3(1f * scale.X, 1.75f * scale.Y, 1f * scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion
            sides = new Vector3(scale.X * 2.1f, scale.Y * 1.15f, scale.Z * 2.1f);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -1.45f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Legs
            CommonInit(position, scale / 2, model, true, asset);
            
        }

        public void Init(Vector3 pos, Matrix orient)
        {
            Vector3 sides = new Vector3(.4f * config.Scale.X, .15f * config.Scale.Y, .5f * config.Scale.Z);
            Skin.AddPrimitive(new Box(new Vector3(sides.X * -.5f, sides.Y * -.5f, sides.Z * -.5f), orient, sides), (int)MaterialTable.MaterialID.NotBouncyNormal); // Top portion            
            CommonInit(pos, orient, true);
        }

        public override void FinalizeBody()
        {
            try
            {
                //Vector3 com = SetMass(2.0f);
                //Skin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
                BodyInit(Position);
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
            
            YawPitchRoll.X += (float)Math.PI/10000;

            Quaternion Orientation = Quaternion.CreateFromYawPitchRoll(YawPitchRoll.X, YawPitchRoll.Y, YawPitchRoll.Z);
            Matrix mHeadingCurrent = Matrix.CreateFromQuaternion(Orientation);
            Vector3 HeadingCurrent = mHeadingCurrent.Forward;
            HeadingCurrent.Normalize();


            // Appy the new current(s)
            this.SetVelocity(HeadingCurrent * this.SpeedCurrent);
            this.Orientation = Matrix.CreateFromQuaternion(Orientation);
        }
    }
}
