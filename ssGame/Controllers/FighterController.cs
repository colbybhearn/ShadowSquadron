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

        static float PatrolRange = 20.0f;
        static float EngageRange = 40.0f;
        static float TargetPositionRange = 2;
        
        static Random r = new Random();

        public static void Update(EnemyFighter fighter, Entity cruiser, Entity player)
        {
            switch (fighter.mode)
            {
                case EnemyFighter.Modes.Patrol:
                    fighter.PositionTarget = cruiser.Position + fighter.OffsetFromCruiser;
                    // if we are near our destination
                    if (IsNearTargetPosition(fighter, cruiser as EnemyCruiser))
                    {
                        float scale = 25;
                        float bubblesize = 10;


                        fighter.OffsetFromCruiser = new Vector3((float)r.NextDouble() - .5f, (float)r.NextDouble() - .5f, (float)r.NextDouble() - .5f);
                        fighter.OffsetFromCruiser *= scale;
                        Vector3 dir = Vector3.Normalize(fighter.OffsetFromCruiser);
                        fighter.OffsetFromCruiser += (dir * bubblesize);
                        
                        // stay in range of the cruiser
                        if (IsNearCuiser(fighter, cruiser))
                        {
                            
                        }
                        else
                        { 

                        }
                    }

                    // this should really be picking a heading.
                    // what heading we pick depends on the environment.

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

            fighter.Update();
            
        }


        public static bool IsNearCuiser(Entity Fighter, Entity cruiser)
        {
            if (cruiser == null)
                return true;
            float dist = Vector3.Distance(cruiser.Position, Fighter.Position);
            if (dist < PatrolRange)
                return true;
            return false;
        }

        public static bool isTargetNear(Entity Fighter, Entity player)
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

        public static bool IsNearTargetPosition(EnemyFighter fighter, EnemyCruiser cruiser)
        {
            float dist = Vector3.Distance(fighter.Position, cruiser.Position + fighter.OffsetFromCruiser);
            if (dist < TargetPositionRange)
                return true;
            return false;
        }




    }
}
