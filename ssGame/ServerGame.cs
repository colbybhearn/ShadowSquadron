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
            assetManager.AddAssetType(AssetTypes.EnemyFighter, CreateFighter);
            assetManager.AddAssetType(AssetTypes.EnemyCruiser, CreateCruiser);
            assetManager.AddAssetType(AssetTypes.Feather, CreateFeather);
            assetManager.AddAssetType(AssetTypes.Beam, CreateBeam);
            assetManager.LoadAssets(Content);
        }

        public override void InitializeCameras()
        {
            base.InitializeCameras();
            cameraManager.AddCamera((int)CameraModes.FreeLook, new FreeCamera());
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
            physicsManager.AddNewObject(feather);
        }

        private void SpawnEnemies()
        {
            Random r = new Random((int)DateTime.Now.ToOADate());
            float x, z;
            int count = 13;
            x = 30;
            z = 30;
            c = (EnemyCruiser)GetEnemyCruiser(new Vector3(x + 10, 15 + 0, z + 0));
            //c.SetVelocity(new Vector3(0, 0, -20f));
            physicsManager.AddNewObject(c);

            for (int i = 0; i < count; i++)
            {
                x = (float)(r.NextDouble() - .5);
                z = (float)(r.NextDouble() - .5);

                x = x * 250;
                z = z * 250;

                Gobject f = GetEnemyFighter(new Vector3(x, 15, z));
                physicsManager.AddNewObject(f);

            }
        }

        public Gobject CreateFeather()
        {
            return Assets.CreateFeather();
        }

        public Gobject CreateFighter()
        {
            return Assets.CreateFighter();
        }

        public Gobject CreateCruiser()
        {
            Gobject o = (EnemyCruiser)Assets.CreateCruiser();
            o.Scale = new Vector3(5, 5, 5);
            return o;
        }

        public Gobject CreateBeam()
        {
            Gobject o = (Beam)Assets.CreateBeam();
            o.Scale = new Vector3(3, 3, 3);
            return o;
        }

        private Gobject GetFeather(Vector3 pos)
        {
            Gobject o = assetManager.GetNewInstance(AssetTypes.Feather);
            o.Position = pos;
            return o;
        }

        private Gobject GetEnemyCruiser(Vector3 pos)
        {
            Gobject o = assetManager.GetNewInstance(AssetTypes.EnemyCruiser);
            o.Position = pos;
            return o;
        }

        private Gobject GetEnemyFighter(Vector3 pos)
        {
            Gobject o = assetManager.GetNewInstance(AssetTypes.EnemyFighter);
            o.Scale = new Vector3(.1f, .1f, .1f);
            o.Position = pos;
            return o;
        }

        private Gobject GetBeam(Vector3 pos, Matrix Orientation)
        {
            Gobject o = assetManager.GetNewInstance(AssetTypes.Beam);
            o.Scale = new Vector3(.1f, .1f, .1f);
            o.Position = pos;
            o.SetOrientation(Orientation);
            
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
            
            
            physicsManager.AddNewObject((Gobject)b);
            //b.SetOrientation(Orientation);
            //b.SetForwardSpeed();

            //Gobject f = GetEnemyFighter(new Vector3(x, 15, z));
            //physicsManager.AddNewObject(f);
        }
        

        public override void DoAiLogic()
        {
            foreach (Gobject go in gameObjects.Values)
            {
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
            }
        }
    }
}
