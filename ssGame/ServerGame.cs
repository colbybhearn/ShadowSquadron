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

namespace ssGame
{
    class ServerGame : GameHelper.Base.ServerBase
    {
        List<EnemyFighter> EnemyFighters = new List<EnemyFighter>();
        EnemyCruiser c;
        Feather feather;
        #region Initialization
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
            List<KeyBinding> cameraDefaults = new List<KeyBinding>();
            cameraDefaults.Add(new KeyBinding("Forward", Keys.NumPad8, false, false, false, KeyEvent.Down, CameraMoveForward));
            cameraDefaults.Add(new KeyBinding("Left", Keys.NumPad4, false, false, false, KeyEvent.Down, CameraMoveLeft));
            cameraDefaults.Add(new KeyBinding("Backward", Keys.NumPad5, false, false, false, KeyEvent.Down, CameraMoveBackward));
            cameraDefaults.Add(new KeyBinding("Right", Keys.NumPad6, false, false, false, KeyEvent.Down, CameraMoveRight));
            cameraDefaults.Add(new KeyBinding("Speed Increase", Keys.NumPad7, false, false, false, KeyEvent.Pressed, CameraMoveSpeedIncrease));
            cameraDefaults.Add(new KeyBinding("Speed Decrease", Keys.NumPad1, false, false, false, KeyEvent.Pressed, CameraMoveSpeedDecrease));
            cameraDefaults.Add(new KeyBinding("Height Increase", Keys.NumPad9, false, false, false, KeyEvent.Down, CameraMoveHeightIncrease));
            cameraDefaults.Add(new KeyBinding("Height Decrease", Keys.NumPad3, false, false, false, KeyEvent.Down, CameraMoveHeightDecrease));

            cameraDefaults.Add(new KeyBinding("Change Mode", Keys.Decimal, false, false, false, KeyEvent.Pressed, CameraModeCycle));
            cameraDefaults.Add(new KeyBinding("Home", Keys.Multiply, false, false, false, KeyEvent.Pressed, CameraMoveHome));
            //
            cameraDefaults.Add(new KeyBinding("Toggle Debug Info", Keys.F1, false, false, false, KeyEvent.Pressed, ToggleDebugInfo));
            cameraDefaults.Add(new KeyBinding("Toggle Physics Debug", Keys.F2, false, false, false, KeyEvent.Pressed, TogglePhsyicsDebug));
            KeyMap camControls = new KeyMap(GenericInputGroups.Camera.ToString(), cameraDefaults);
            kmc.AddMap(camControls);

            List<KeyBinding> flightDefaults = new List<KeyBinding>();
            flightDefaults.Add(new KeyBinding("Forward", Keys.W, false, false, false, KeyEvent.Down, FeatherPitchDown));
            flightDefaults.Add(new KeyBinding("Left", Keys.A, false, false, false, KeyEvent.Down, FeatherRollLeft));
            flightDefaults.Add(new KeyBinding("Backward", Keys.S, false, false, false, KeyEvent.Down, FeatherPitchUp));
            flightDefaults.Add(new KeyBinding("Right", Keys.D, false, false, false, KeyEvent.Down, FeatherRollRight));
            KeyMap flightControls = new KeyMap("flight", flightDefaults);
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
            int count = 3;
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
            o.Position = pos;
            return o;
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
