using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper.Input;
using Helper.Physics.PhysicsObjects;
using Helper.Objects;
using ssGame.PhysicsObjects;

namespace ssGame
{
    // I had to add WindowsGameLibrary to the solution (and Content), and add to this project, a reference to WindowsGameLibrary, for models to load.
    public class ClientGame : GameHelper.Base.ClientBase
    {
        Aircraft myPlane;

        public override Helper.Input.KeyMapCollection GetDefaultControls()
        {
            KeyMapCollection kmc = new KeyMapCollection();
            List<KeyBinding> flight = new List<KeyBinding>();
            flight.Add(new KeyBinding("right", Microsoft.Xna.Framework.Input.Keys.D, false, false, false, KeyEvent.Down, TurnRight));
            KeyMap map = new KeyMap("flight", flight);
            kmc.AddMap(map);
            return kmc;
        }

        public override void InitializeContent()
        {
            base.InitializeContent();
            assetManager.AddAsset(Assets.Types.EnemyFighter, CreateFighter);
        }


        #region CallBacks
        public Gobject CreateFighter()
        {
            return Assets.CreateEnemyFighter(lander);
        }
        public void TurnRight()
        {

        }
        #endregion
    }
}
