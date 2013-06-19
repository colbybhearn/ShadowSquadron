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

        public override InputCollection GetDefaultControls()
        {
            InputCollection ic = new InputCollection();
            List<ButtonBinding> flight = new List<ButtonBinding>();
            flight.Add(new KeyBinding("right", Keys.D, ButtonEvent.Down, TurnRight));
            ButtonMap map = new ButtonMap("flight", flight);
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

            return ic;
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
