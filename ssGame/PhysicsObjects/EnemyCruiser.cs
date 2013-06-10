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
    public class EnemyCruiser : Gobject
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

        public float NominalSpeed;

        BoostController VertJet;
        BoostController RotJetY;
        BoostController RotJetX;
        BoostController RotJetZ;
        const float MAX_VERT_MAGNITUDE=30;
        const float MAX_ROT_JET=10;

        public EnemyCruiser(Vector3 position, Vector3 scale, Matrix orient, Model model, int asset)
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
    }
}
