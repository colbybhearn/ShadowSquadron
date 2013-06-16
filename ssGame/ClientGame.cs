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

        public override KeyMapCollection GetDefaultControls()
        {
            KeyMapCollection kmc = new KeyMapCollection();
            List<KeyBinding> flight = new List<KeyBinding>();
            flight.Add(new KeyBinding("right", Keys.D, KeyEvent.Down, TurnRight));
            KeyMap map = new KeyMap("flight", flight);
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

            return kmc;
        }

        #region Initialization
        public override void InitializeContent()
        {
            base.InitializeContent();
            assetManager.AddAssetType(AssetTypes.EnemyFighter, CreateFighter, typeof(EnemyFighter));
            //LoadModel(ref modelEnemyFighter, "Airplane", AssetTypes.EnemyFighter, CreateFighter);
            //LoadModel(ref modelFeatherFighter, "Airplane", AssetTypes.Feather1, CreateFighter);
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
        public Gobject CreateFighter()
        {
            return Assets.CreateFighter();
        }
        public Gobject CreateFeather1()
        {
            return Assets.CreateFighter();
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
