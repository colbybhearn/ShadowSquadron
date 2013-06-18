using System;
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
            cameraManager.SetCurrentCamera((int)CameraModes.Chase);
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
        }

        public override GameHelper.Input.KeyMapCollection GetDefaultControls()
        {
            KeyMapCollection kmc = new KeyMapCollection();
            List<KeyBinding> flight = new List<KeyBinding>();
            //flight.Add(new KeyBinding("right", Microsoft.Xna.Framework.Input.Keys.D, false, false, false, KeyEvent.Down, TurnRight));
            KeyMap map = new KeyMap("null", flight);
            kmc.AddMap(map);
            
            //Camera
            KeyMap camControls = new KeyMap(GenericInputGroups.Camera.ToString());
            camControls.AddKeyBinding(new KeyBinding("Forward", Keys.NumPad8, KeyEvent.Down, CameraMoveForward));
            camControls.AddKeyBinding(new KeyBinding("Left", Keys.NumPad4, KeyEvent.Down, CameraMoveLeft));
            camControls.AddKeyBinding(new KeyBinding("Backward", Keys.NumPad5, KeyEvent.Down, CameraMoveBackward));
            camControls.AddKeyBinding(new KeyBinding("Right", Keys.NumPad6, KeyEvent.Down, CameraMoveRight));
            camControls.AddKeyBinding(new KeyBinding("Speed Increase", Keys.NumPad7, KeyEvent.Pressed, CameraMoveSpeedIncrease));
            camControls.AddKeyBinding(new KeyBinding("Speed Decrease", Keys.NumPad1, KeyEvent.Pressed, CameraMoveSpeedDecrease));
            camControls.AddKeyBinding(new KeyBinding("Height Increase", Keys.NumPad9, KeyEvent.Down, CameraMoveHeightIncrease));
            camControls.AddKeyBinding(new KeyBinding("Height Decrease", Keys.NumPad3, KeyEvent.Down, CameraMoveHeightDecrease));

            camControls.AddKeyBinding(new KeyBinding("Change Mode", Keys.Decimal, KeyEvent.Pressed, CameraModeCycle));
            camControls.AddKeyBinding(new KeyBinding("Home", Keys.Multiply, KeyEvent.Pressed, CameraMoveHome));

            camControls.AddKeyBinding(new KeyBinding("Toggle Debug Info", Keys.F1, KeyEvent.Pressed, ToggleDebugInfo));
            camControls.AddKeyBinding(new KeyBinding("Toggle Physics Debug", Keys.F2, KeyEvent.Pressed, TogglePhsyicsDebug));

            kmc.AddMap(camControls);

            KeyMap flightControls = new KeyMap("flight");
            flightControls.AddKeyBinding(new KeyBinding("Forward", Keys.W, KeyEvent.Down, FeatherPitchDown));
            flightControls.AddKeyBinding(new KeyBinding("Left", Keys.A, KeyEvent.Down, FeatherRollLeft));
            flightControls.AddKeyBinding(new KeyBinding("Backward", Keys.S, KeyEvent.Down, FeatherPitchUp));
            flightControls.AddKeyBinding(new KeyBinding("Right", Keys.D, KeyEvent.Down, FeatherRollRight));
            flightControls.AddKeyBinding(new KeyBinding("Accelerate", Keys.OemPlus, KeyEvent.Down, FeatherAccelerate));
            flightControls.AddKeyBinding(new KeyBinding("Decelerate", Keys.OemMinus, KeyEvent.Down, FeatherDecelerate));
            flightControls.AddKeyBinding(new KeyBinding("Fire", Keys.Space, KeyEvent.Pressed, FeatherFire));

            kmc.AddMap(flightControls);

            return kmc;
        }
        #endregion

        public override void Start()
        {
            base.Start();
            SpawnFeather();
            SpawnEnemies();
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
            int count = 15;
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
            o.Init(pos, o.BodyOrientation());
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

            Beam b = (Beam)GetBeam(feather.BeamSpawnLocation(), feather.BodyOrientation());
            Matrix Orientation = feather.BodyOrientation();
            physicsManager.AddNewObject((Entity)b);
            b.SetOrientation(Orientation);
            b.SetForwardSpeed();
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
                    if (Vector3.Distance(b.BodyPosition(), Vector3.Zero) > 500)
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
                cameraManager.currentCamera.TargetPosition = feather.BodyPosition() + feather.BodyVelocity();
                cameraManager.currentCamera.CurrentPosition = feather.BodyPosition();
                cameraManager.currentCamera.SetCurrentOrientation(feather.BodyOrientation());
            }
        }
    }
}
