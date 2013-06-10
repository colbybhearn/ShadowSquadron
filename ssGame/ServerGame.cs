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
            return kmc;
        }
        #endregion

        public override void Start()
        {
            base.Start();
            
            SpawnFighters();
        }

        private void SpawnFighters()
        {
            Random r = new Random((int)DateTime.Now.ToOADate());
            float x, z;
            int count = 1;

            for (int i = 0; i < count; i++)
            {
                x = (float)(r.NextDouble() - .5);
                z = (float)(r.NextDouble() - .5);

                x = x * 250;
                z = z * 250;

                c = (EnemyCruiser)GetEnemyCruiser(new Vector3(x+2, 126, z-4));
                //c.SetVelocity(new Vector3(0, 0, -20f));
                physicsManager.AddNewObject(c);

                Gobject f = GetEnemyFighter(new Vector3(x, 10, z));
                physicsManager.AddNewObject(f);
            }
        }

        public Gobject CreateFighter()
        {
            return Assets.CreateEnemyFighter();
        }
        public Gobject CreateCruiser()
        {
            Gobject o = (EnemyCruiser)Assets.CreateEnemyCruiser();
            o.Scale = new Vector3(2, 2, 2);
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

        public override void DoAiLogic()
        {
            foreach (Gobject go in gameObjects.Values)
            {
                if (go is EnemyFighter)
                {
                    FighterController.Update(go as EnemyFighter, c, null);
                }
            }
        }

    }
}
