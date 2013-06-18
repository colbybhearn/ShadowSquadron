using GameHelper.Camera.Cameras;
using GameHelper.Input;
using GameHelper.Objects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using ssGame.PhysicsObjects;

namespace ssGame
{
    // I had to add WindowsGameLibrary to the solution (and Content), and add to this project, a reference to WindowsGameLibrary, for models to load.
    public class ClientGame : GameHelper.Base.ClientBase
    {
        Model modelEnemyFighter;
        Model modelFeatherFighter;

        public override ButtonMapCollection GetDefaultControls()
        {
            ButtonMapCollection bmc = new ButtonMapCollection();
            List<KeyBinding> flight = new List<KeyBinding>();
            flight.Add(new KeyBinding("right", Keys.D, KeyEvent.Down, TurnRight));
            ButtonMap map = new ButtonMap("flight", flight);
            bmc.AddMap(map);

            //Camera
            ButtonMap camControls = new ButtonMap(GenericInputGroups.Camera.ToString());
            camControls.AddBinding(new KeyBinding("Forward", Keys.NumPad8, KeyEvent.Down, CameraMoveForward));
            camControls.AddBinding(new KeyBinding("Left", Keys.NumPad4, KeyEvent.Down, CameraMoveLeft));
            camControls.AddBinding(new KeyBinding("Backward", Keys.NumPad5, KeyEvent.Down, CameraMoveBackward));
            camControls.AddBinding(new KeyBinding("Right", Keys.NumPad6, KeyEvent.Down, CameraMoveRight));
            camControls.AddBinding(new KeyBinding("Speed Increase", Keys.NumPad7, KeyEvent.Pressed, CameraMoveSpeedIncrease));
            camControls.AddBinding(new KeyBinding("Speed Decrease", Keys.NumPad1, KeyEvent.Pressed, CameraMoveSpeedDecrease));
            camControls.AddBinding(new KeyBinding("Height Increase", Keys.NumPad9, KeyEvent.Down, CameraMoveHeightIncrease));
            camControls.AddBinding(new KeyBinding("Height Decrease", Keys.NumPad3, KeyEvent.Down, CameraMoveHeightDecrease));

            camControls.AddBinding(new KeyBinding("Change Mode", Keys.Decimal, KeyEvent.Pressed, CameraModeCycle));
            camControls.AddBinding(new KeyBinding("Home", Keys.Multiply, KeyEvent.Pressed, CameraMoveHome));

            camControls.AddBinding(new KeyBinding("Toggle Debug Info", Keys.F1, KeyEvent.Pressed, ToggleDebugInfo));
            camControls.AddBinding(new KeyBinding("Toggle Physics Debug", Keys.F2, KeyEvent.Pressed, TogglePhsyicsDebug));
            bmc.AddMap(camControls);

            return bmc;
        }

        #region Initialization
        public override void InitializeContent()
        {
            base.InitializeContent();
            assetManager.AddAssetType(AssetTypes.EnemyFighter, typeof(EnemyFighter));
            assetManager.LoadAssets(Content);
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

        #region CallBacks

        public void TurnRight()
        {

        }
        #endregion

        public void StartGame()
        {

        }
    }
}
