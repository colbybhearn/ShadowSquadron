using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper.Input;

namespace ssGame
{
    class ServerGame : GameHelper.Base.ServerBase
    {
        public override Helper.Input.KeyMapCollection GetDefaultControls()
        {
            KeyMapCollection kmc = new KeyMapCollection();
            List<KeyBinding> flight = new List<KeyBinding>();
            //flight.Add(new KeyBinding("right", Microsoft.Xna.Framework.Input.Keys.D, false, false, false, KeyEvent.Down, TurnRight));
            KeyMap map = new KeyMap("null", flight);
            kmc.AddMap(map);
            return kmc;
        }
    }
}
