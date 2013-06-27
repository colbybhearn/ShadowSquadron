using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ssGame.PhysicsObjects;
using GameHelper.Camera.Cameras;
using Microsoft.Xna.Framework;
using GameHelper.Input;
using Microsoft.Xna.Framework.Input;
using GameHelper.Objects;
using JigLibX.Collision;

namespace ssGame
{
    class ExConstraint : GameHelper.Base.ServerBase
    {
        #region Initialization
        public ExConstraint()
            : base()
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
            //assetManager.AddAssetType(AssetTypes.EnemyFighter, typeof(EnemyFighter));
            //assetManager.AddAssetType(AssetTypes.EnemyCruiser, typeof(EnemyCruiser));
            assetManager.AddAssetType(AssetTypes.Feather, typeof(Feather));
            //assetManager.AddAssetType(AssetTypes.Beam, typeof(Beam));
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

            ic.AddMap(flightControls);

            AnalogMap flightControlsGamePad = new AnalogMap("flight_gp");

            flightControlsGamePad.AddBinding(new GamePadThumbStickBinding("Fly", ThumbStick.Left, AnalogEvent.Always, AnalogData.Absolute, FeatherMove));

            ic.AddMap(flightControlsGamePad);


            return ic;
        }
        #endregion

        public override void Start()
        {
            base.Start();
            SpawnFeather();
        }
        Feather feather;
        private void SpawnFeather()
        {
            feather = (Feather)GetFeather(new Vector3(0, 0, 0));
            //feather = (Feather)assetManager.GetNewInstance(AssetTypes.Feather);
            feather.Init(new Vector3(0, 0, 0), feather.Orientation);
            physicsManager.AddNewObject(feather);
        }
        
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

            Beam b = (Beam)GetBeam(feather.BeamSpawnLocation(), feather.Orientation);
            Matrix Orientation = feather.Orientation;
            physicsManager.AddNewObject((Entity)b);
            b.Orientation = Orientation;
            b.SetForwardSpeed();
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
                    //FighterController.Update(go as EnemyFighter, c, null);
                }
                else if (go is EnemyCruiser)
                {
                    //CruiserController.Update(go as EnemyCruiser);
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
