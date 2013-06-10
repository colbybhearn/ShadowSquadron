using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelper.Objects;
using ssGame.PhysicsObjects;
using Microsoft.Xna.Framework;

namespace ssGame.Controllers
{
    public class FighterController
    {

        /*
        * Purpose
        *  - defend an object
        * 
        * Behavior 
        *  - patrol that object
        *    - stay within range of it
        *    - 
        *  - detect a thread
        *    - some
        *  - decide whether or not to engage
        *    - if threat is within a range, engage
        *  - engage a threat
        *    - target the threat and shoot
        *  - they avoid crashing into objects 
        *    - move around but avoid colliding into anything, based on things' movement and distance
        *  - take evasive action when getting shot
        *    - when hit, change headings drastically
        * 
        */

        static float PatrolRange = 19.0f;
        static float EngageRange = 40.0f;

        public static void Update(EnemyFighter fighter, Gobject cruiser, Gobject player)
        {
            switch (fighter.mode)
            {
                case EnemyFighter.Modes.Patrol:
                    // stay in range
                    if (!IsNearCuiser(fighter, cruiser))
                    {
                        // redirect heading toward the cruiser
                        fighter.HeadingTarget = cruiser.BodyPosition() - fighter.BodyPosition();
                        fighter.HeadingTarget.Normalize();
                        
                    }

                    // if targets, attack
                    if (isTargetNear(fighter, player))
                    {
                        fighter.mode = EnemyFighter.Modes.Attack;
                    }

                    // if hit, evade
                    if (hitRecently())
                    {
                        fighter.mode = EnemyFighter.Modes.Evade;
                    }
                    break;
                case EnemyFighter.Modes.Attack:
                    // if hit, evade
                    if (hitRecently())
                    {
                        fighter.mode = EnemyFighter.Modes.Evade;
                    }

                    if (isTargetNear(fighter, player))
                    {
                        // align to attack
                    }
                    else // if no targets, patrol
                    {
                        fighter.mode = EnemyFighter.Modes.Patrol;
                    }
                    break;
                case EnemyFighter.Modes.Evade:
                    if (hitRecently())
                    {
                        // evade in some direction, once.
                        // clear hit recently.
                    }

                    // if clear, patrol
                    if (!isTargetNear(fighter, player))
                    {
                        fighter.mode = EnemyFighter.Modes.Patrol;
                    }                    
                    break;
                default:
                    break;
            }



            //if (fighter.BodyVelocity().Length() < fighter.NominalSpeed)
                //fighter.SetVelocity(new Vector3(0, 0, -fighter.NominalSpeed));

            float hDelta = Vector3.Dot(fighter.HeadingTarget, fighter.HeadingCurrent);
            if (hDelta > .5)
                fighter.SpeedTarget = fighter.MAX_SPEED;
            else if (hDelta > 0)
                fighter.SpeedTarget = (fighter.MAX_SPEED - fighter.MIN_SPEED) / 2;
            else
                fighter.SpeedTarget = fighter.MIN_SPEED;

            //if (hDelta <= 1 && hDelta >0)
            //{
            //    Vector3 diff = fighter.HeadingTarget - fighter.HeadingCurrent;
            //    fighter.HeadingCurrent += diff / 10.0f;
            //}
            //else
            //{
            if (hDelta < 1 && hDelta > .5)
            {

            }
            else
            {
                
                fighter.PitchYawRoll.X += .005f;
                //fighter.PitchYawRoll = Vector3.Clamp(fighter.PitchYawRoll, new Vector3(-1.57f, float.MinValue, float.MinValue), new Vector3(1.57f, float.MaxValue, float.MaxValue));

            }
            fighter.Update();
            
            // apply self-preservation logic

        }


        public static bool IsNearCuiser(Gobject Fighter, Gobject cruiser)
        {
            if (cruiser == null)
                return true;
            float dist = Vector3.Distance(cruiser.BodyPosition(), Fighter.BodyPosition());
            if (dist < PatrolRange)
                return true;
            return false;
        }

        public static bool isTargetNear(Gobject Fighter, Gobject player)
        {
            if (player == null)
                return false;
            if (Vector3.Distance(player.Position, Fighter.Position) < EngageRange)
                return true;
            return false;
        }

        public static bool hitRecently()
        {
            return false;
        }




    }
}
