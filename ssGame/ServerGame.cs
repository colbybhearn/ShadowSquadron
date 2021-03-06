﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Input;
using GameHelper;
using Microsoft.Xna.Framework.Graphics;
using ssGame.PhysicsObjects;
using GameHelper.Camera.Cameras;
using Microsoft.Xna.Framework.Input;
using GameHelper.Objects;
using Microsoft.Xna.Framework;
using ssGame.Controllers;
using System.Diagnostics;
using JigLibX.Collision;

namespace ssGame
{
    class ServerGame : GameHelper.Base.ServerBase
    {
        List<EnemyFighter> EnemyFighters = new List<EnemyFighter>();
        EnemyCruiser c;
        Feather feather;
        List<Beam> Beams = new List<Beam>();

        private long lastFire = 0;

        #region Initialization
        public ServerGame() : base()
        {
            this.BackColor = Color.Black;
        }
        public override void InitializeMultiplayer()
        {
            base.InitializeMultiplayer();
        }

        public override void InitializeContent()
        {
            base.InitializeContent();
            assetManager.AddAssetType(AssetTypes.EnemyFighter, typeof(EnemyFighter));
            assetManager.AddAssetType(AssetTypes.EnemyCruiser, typeof(EnemyCruiser));
            assetManager.AddAssetType(AssetTypes.Feather, typeof(Feather));
            assetManager.AddAssetType(AssetTypes.Beam, typeof(Beam));
            assetManager.LoadAssets(Content);
        }

        public override void InitializeCameras()
        {
            base.InitializeCameras();
            cameraManager.AddCamera((int)CameraModes.FreeLook, new FreeCamera());
            cameraManager.AddCamera((int)CameraModes.Chase, new ChaseCameraRelative());
            cameraManager.SetCurrentCamera((int)CameraModes.FreeLook);
        }

        public override void InitializeEnvironment()
        {
            base.InitializeEnvironment();
            physicsManager.PhysicsSystem.Gravity= new Vector3(0,0,0);
        }

        public override void InitializeInputs()
        {
            base.InitializeInputs();

            inputManager.EnableKeyMap(GenericInputGroups.Camera.ToString());
            inputManager.EnableKeyMap("flight");
            inputManager.EnableKeyMap("flight_gp");
        }

        public override GameHelper.Input.InputCollection GetDefaultControls()
        {
            InputCollection ic = new InputCollection();
            List<ButtonBinding> flight = new List<ButtonBinding>();
            //flight.Add(new KeyBinding("right", Microsoft.Xna.Framework.Input.Keys.D, false, false, false, KeyEvent.Down, TurnRight));
            ButtonMap map = new ButtonMap("null", flight);
            ic.AddMap(map);
            
            //Camera
            ButtonMap camControls = new ButtonMap(GenericInputGroups.Camera.ToString());
            camControls.AddBinding(new KeyBinding("Forward", Keys.NumPad8, ButtonEvent.Down, CameraMoveForward));
            camControls.AddBinding(new KeyBinding("Left", Keys.NumPad4, ButtonEvent.Down, CameraMoveLeft));
            camControls.AddBinding(new KeyBinding("Backward", Keys.NumPad5, ButtonEvent.Down, CameraMoveBackward));
            camControls.AddBinding(new KeyBinding("Right", Keys.NumPad6, ButtonEvent.Down, CameraMoveRight));
            camControls.AddBinding(new KeyBinding("Speed Increase", Keys.NumPad7, ButtonEvent.Pressed, CameraMoveSpeedIncrease));
            camControls.AddBinding(new KeyBinding("Speed Decrease", Keys.NumPad1, ButtonEvent.Pressed, CameraMoveSpeedDecrease));
            camControls.AddBinding(new KeyBinding("Height Increase", Keys.NumPad9, ButtonEvent.Down, CameraMoveHeightIncrease));
            camControls.AddBinding(new KeyBinding("Height Decrease", Keys.NumPad3, ButtonEvent.Down, CameraMoveHeightDecrease));

            camControls.AddBinding(new KeyBinding("Change Mode", Keys.Decimal, ButtonEvent.Pressed, CameraModeCycle));
            camControls.AddBinding(new KeyBinding("Home", Keys.Multiply, ButtonEvent.Pressed, CameraMoveHome));

            camControls.AddBinding(new KeyBinding("Toggle Debug Info", Keys.F1, ButtonEvent.Pressed, ToggleDebugInfo));
            camControls.AddBinding(new KeyBinding("Toggle Physics Debug", Keys.F2, ButtonEvent.Pressed, TogglePhsyicsDebug));

            ic.AddMap(camControls);

            ButtonMap flightControls = new ButtonMap("flight");
            flightControls.AddBinding(new KeyBinding("Forward", Keys.W, ButtonEvent.Down, FeatherPitchDown));
            flightControls.AddBinding(new KeyBinding("Left", Keys.A, ButtonEvent.Down, FeatherRollLeft));
            flightControls.AddBinding(new KeyBinding("Backward", Keys.S, ButtonEvent.Down, FeatherPitchUp));
            flightControls.AddBinding(new KeyBinding("Right", Keys.D, ButtonEvent.Down, FeatherRollRight));
            flightControls.AddBinding(new KeyBinding("Accelerate", Keys.OemPlus, ButtonEvent.Down, FeatherAccelerate));
            flightControls.AddBinding(new KeyBinding("Decelerate", Keys.OemMinus, ButtonEvent.Down, FeatherDecelerate));
            flightControls.AddBinding(new KeyBinding("Fire", Keys.Space, ButtonEvent.Pressed, FeatherFire));

            flightControls.AddBinding(new GamePadButtonBinding("Accelerate_gp", Buttons.RightShoulder, ButtonEvent.Down, FeatherAccelerate));
            flightControls.AddBinding(new GamePadButtonBinding("Decelerate_gp", Buttons.LeftShoulder, ButtonEvent.Down, FeatherDecelerate));
            //flightControls.AddBinding(new GamePadButtonBinding("Fire_gp", Buttons.RightTrigger, ButtonEvent.Down, FeatherFire)); // Fires too easily, using the analog method


            ic.AddMap(flightControls);

            AnalogMap flightControlsGamePad = new AnalogMap("flight_gp");

            flightControlsGamePad.AddBinding(new GamePadThumbStickBinding("Fly", ThumbStick.Left, AnalogEvent.Always, AnalogData.Absolute, FeatherMove));
            flightControlsGamePad.AddBinding(new GamePadTriggerBinding("Fire", Trigger.Right, AnalogEvent.Always, AnalogData.Absolute, FeatherFireAnalog));

            ic.AddMap(flightControlsGamePad);


            return ic;
        }
        #endregion

        public override void Start()
        {
            base.Start();
            SpawnFeather();
            //SpawnEnemies();
        }

        private void SpawnFeather()
        {
            feather = (Feather)GetFeather(new Vector3(0, 0, 0));
            //feather = (Feather)assetManager.GetNewInstance(AssetTypes.Feather);
            feather.Init(new Vector3(0, 0, 0), feather.Orientation);
            physicsManager.AddNewObject(feather);
        }

        private void SpawnEnemies()
        {
            Random r = new Random((int)DateTime.Now.ToOADate());
            float x, z;
            int count = 5;
            x = 30;
            z = 30;
            c = (EnemyCruiser)GetEnemyCruiser(new Vector3(x + 10, 15 + 0, z + 0));
            physicsManager.AddNewObject(c);

            for (int i = 0; i < count; i++)
            {
                x = (float)(r.NextDouble() - .5);
                z = (float)(r.NextDouble() - .5);

                x = x * 250;
                z = z * 250;

                EnemyFighter fighter = (EnemyFighter)GetEnemyFighter(new Vector3(x, 15, z));
                //fighter.AddCollisionCallback(new JigLibX.Collision.CollisionCallbackFn(EnemyHit));
                fighter.Init(new Vector3(x, 15, z), Matrix.Identity);
                physicsManager.AddNewObject(fighter);
            }
        }
        /*
        public bool EnemyHit(CollisionSkin skin0, CollisionSkin skin1)
        {
            EnemyFighter fighter = null;
            Gobject obj = null;
            if (skin0.Owner.ExternalData is EnemyFighter)
                fighter = skin0.Owner.ExternalData as EnemyFighter;
            if (skin1.Owner == null)
                return true;
            if (skin1.Owner.ExternalData is Gobject)
                obj = skin1.Owner.ExternalData as Gobject;

            if (fighter == null || obj == null)
                return true;

            if (objectsToDelete.Contains(obj.ID)) // if the object is going to be deleted soon,
                return false; // don't bother doing any collision with it

            int type = obj.aType.Id;
            if ((AssetTypes)type == AssetTypes.EnemyFighter)
            {
                //fighter.SetLaser(true);
                DeleteObject(obj.ID);
                return false;
            }
            if ((AssetTypes)type == AssetTypes.Radar1Pickup)
            {
                //fighter.SetRadar(true);
                DeleteObject(obj.ID);
                return false;
            }
            return true;
        }
        */


        public bool BeamHitSomething(CollisionSkin skin0, CollisionSkin skin1)
        {
            
            Beam beam = null;
            Entity obj = null;
            if (skin0.Owner.ExternalData is Beam)
                beam = skin0.Owner.ExternalData as Beam;
            if (skin1.Owner == null)
                return true;
            if (skin1.Owner.ExternalData is Entity)
                obj = skin1.Owner.ExternalData as Entity;

            if (beam == null || obj == null)
                return true;

            if (objectsToDelete.Contains(obj.ID)) // if the object is going to be deleted soon,
                return false; // don't bother doing any collision with it

            DeleteObject(beam.ID);

            int type = obj.aType.Id;
            if ((AssetTypes)type == AssetTypes.EnemyFighter)
            {
                DeleteObject(obj.ID);
                //DeleteObject(obj.ID);
                return false;
            }
            if ((AssetTypes)type == AssetTypes.EnemyCruiser)
            {
                //DeleteObject(obj.ID);
                return false;
            }
            return true;
        }

        private Entity GetFeather(Vector3 pos)
        {
            Entity o = assetManager.GetNewInstance(AssetTypes.Feather);
            return o;
        }

        private Entity GetEnemyCruiser(Vector3 pos)
        {
            EnemyCruiser o = (EnemyCruiser)assetManager.GetNewInstance(AssetTypes.EnemyCruiser);
            o.Init(pos, Matrix.Identity);
            return o;
        }

        private Entity GetEnemyFighter(Vector3 pos)
        {
            Entity o = assetManager.GetNewInstance(AssetTypes.EnemyFighter);
            return o;
        }

        private Entity GetBeam(Vector3 pos, Matrix Orientation)
        {
            Beam o = (Beam)assetManager.GetNewInstance(AssetTypes.Beam);
            o.Init(pos, o.Orientation);
            o.AddCollisionCallback(BeamHitSomething);
            return o;
        }

        public void FeatherAccelerate()
        {
            if (feather != null)
                feather.IncreseSpeed();
        }

        public void FeatherDecelerate()
        {
            if (feather != null)
                feather.DecreaseSpeed();
        }

        public void FeatherPitchUp()
        {
            if (feather != null)
                feather.PitchUp();
        }

        public void FeatherPitchDown()
        {
            if (feather != null)
                feather.PitchDown();
        }

        public void FeatherRollLeft()
        {
            if (feather != null)
                feather.RollLeft();
        }

        public void FeatherRollRight()
        {
            if (feather != null)
                feather.RollRight();
        }

        public void FeatherFire()
        {
            if (feather == null)
                return;
            
            long currentTime = DateTime.UtcNow.Ticks;

            if (lastFire + (10000 * 200) > currentTime) // 10000 ticks in a milisecond, fire every 10 miliseconds (10 times a second)
                return;

            lastFire = currentTime;

            Beam b = (Beam)GetBeam(feather.BeamSpawnLocation(), feather.Orientation);
            Matrix Orientation = feather.Orientation;
            physicsManager.AddNewObject((Entity)b);
            b.Orientation = Orientation;
            b.SetForwardSpeed();
        }

        // Trigger!
        public void FeatherFireAnalog(double value, double _)
        {
            if (value > .7)
                FeatherFire();
        }

        // Joystick!
        public void FeatherMove(double x, double y)
        {
            if (feather == null)
                return;
            feather.Roll((float)x);
            feather.Pitch((float)y);
        }
        

        public override void DoAiLogic()
        {
            foreach (Entity go in gameObjects.Values)
            {
                // Removes spinning from collisions
                go.SetAngularVelocity(Vector3.Zero);

                if (go is EnemyFighter)
                {
                    FighterController.Update(go as EnemyFighter, c, null);
                }
                else if (go is EnemyCruiser)
                {
                    CruiserController.Update(go as EnemyCruiser);
                }
                else if (go is Feather)
                {
                    Feather f = go as Feather;
                    f.Update();                    
                }
                else if (go is Beam)
                {
                    Beam b = go as Beam;
                    if (Vector3.Distance(b.Position, Vector3.Zero) > 500)
                        physicsManager.RemoveObject(b.ID);
                }
            }
        }

        public override void UpdateCamera()
        {
            base.UpdateCamera();
            if (feather == null)
                return;

            if (!(cameraManager.currentCamera is FreeCamera))
            {
                cameraManager.currentCamera.TargetPosition = feather.Position + feather.Velocity;
                cameraManager.currentCamera.CurrentPosition = feather.Position;
                cameraManager.currentCamera.SetCurrentOrientation(feather.Orientation);
            }
        }
    }
}
