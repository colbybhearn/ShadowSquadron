using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Input;
using GameHelper.Physics.PhysicsObjects;
using GameHelper.Objects;
using ssGame.PhysicsObjects;
using Microsoft.Xna.Framework.Graphics;
using GameHelper.Camera.Cameras;
using Microsoft.Xna.Framework.Input;

namespace ssGame
{
    // I had to add WindowsGameLibrary to the solution (and Content), and add to this project, a reference to WindowsGameLibrary, for models to load.
    public class ClientGame : GameHelper.Base.ClientBase
    {
        Model modelEnemyFighter;
        Model modelFeatherFighter;

        public override GameHelper.Input.KeyMapCollection GetDefaultControls()
        {
            KeyMapCollection kmc = new KeyMapCollection();
            List<KeyBinding> flight = new List<KeyBinding>();
            flight.Add(new KeyBinding("right", Keys.D, false, false, false, KeyEvent.Down, TurnRight));
            KeyMap map = new KeyMap("flight", flight);
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

        #region Initialization
        public override void InitializeContent()
        {
            base.InitializeContent();
            LoadModel(ref modelEnemyFighter, "Airplane", AssetTypes.EnemyFighter, CreateFighter);
            LoadModel(ref modelFeatherFighter, "Airplane", AssetTypes.Feather1, CreateFighter);
        }
        public override void InitializeCameras()
        {
            base.InitializeCameras();
            cameraManager.AddCamera((int)CameraModes.FreeLook, new FreeCamera());
            cameraManager.SetCurrentCamera((int)CameraModes.FreeLook);
            
        }
        public override void InitializeInputs()
        {
            base.InitializeInputs();
            inputManager.EnableKeyMap(GenericInputGroups.Camera.ToString());
        }
        public override void InitializeSound()
        {
            base.InitializeSound();
        }

        #endregion

        #region Camera Control        
        #endregion

        #region CallBacks
        public Gobject CreateFighter()
        {
            return Assets.CreateEnemyFighter(modelEnemyFighter);
        }
        public Gobject CreateFeather1()
        {
            return Assets.CreateEnemyFighter(modelFeatherFighter);
        }

        public void TurnRight()
        {

        }
        #endregion

        public void StartGame()
        {

        }
    }
}
