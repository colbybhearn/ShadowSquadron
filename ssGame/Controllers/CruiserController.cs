using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ssGame.Controllers
{
    class CruiserController
    {

        internal static void Update(PhysicsObjects.EnemyCruiser enemyCruiser)
        {
            enemyCruiser.Update();
        }
    }
}
