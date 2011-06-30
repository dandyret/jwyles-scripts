﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-03-25 21:10:07 +0100 (fr, 25 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/PetManager.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-03-25 21:10:07 +0100 (fr, 25 mar 2011) $
// $LastChangedRevision: 228 $
// $Revision: 228 $

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWCache;
using Styx.WoWInternals.WoWObjects;

namespace Singular
{
    public enum PetType
    {
        // These are CreatureFamily IDs. See 'CurrentPet' for usage.
        None = 0,
        Imp = 23,
        Felguard = 29,
        Voidwalker = 16,
        Felhunter = 15,
        Succubus = 17,
    }

    internal class PetManager
    {
        private static readonly WaitTimer CallPetTimer = WaitTimer.OneSecond;

        private static ulong _petGuid;
        private static readonly List<WoWPetSpell> _petSpells = new List<WoWPetSpell>();

        static PetManager()
        {
            // NOTE: This is a bit hackish. This fires VERY OFTEN in major cities. But should prevent us from summoning right after dismounting.
            Lua.Events.AttachEvent("COMPANION_UPDATE", (s, e) => CallPetTimer.Reset());
        }

        public static PetType CurrentPetType
        {
            get
            {
                WoWUnit myPet = StyxWoW.Me.Pet;
                if (myPet == null)
                {
                    return PetType.None;
                }
                WoWCache.CreatureCacheEntry c;
                myPet.GetCachedInfo(out c);
                return (PetType)c.FamilyID;
            }
        }

        public static bool HavePet { get { return StyxWoW.Me.GotAlivePet; } }

        internal static void Pulse()
        {
            if (!StyxWoW.Me.GotAlivePet)
            {
                _petSpells.Clear();
                return;
            }

            if (StyxWoW.Me.Pet != null && _petGuid != StyxWoW.Me.Pet.Guid)
            {
                _petGuid = StyxWoW.Me.Pet.Guid;
                _petSpells.Clear();
                // Cache the list. yea yea, we should just copy it, but I'd rather have shallow copies of each object, rather than a copy of the list.
                _petSpells.AddRange(StyxWoW.Me.PetSpells);
            }
        }

        public static bool CanCastPetAction(string action)
        {
            WoWPetSpell petAction = _petSpells.FirstOrDefault(p => p.ToString() == action);
            if (petAction == null || petAction.Spell == null)
            {
                return false;
            }

            return !petAction.Spell.Cooldown;
        }

        public static void CastPetAction(string action)
        {
            var spell = _petSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Logger.Write(string.Format("[Pet] Casting {0}", action));
            Lua.DoString("CastPetAction({0})", spell.ActionBarIndex + 1);
        }

        public static void CastPetAction(string action, WoWUnit on)
        {
            var spell = _petSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Logger.Write(string.Format("[Pet] Casting {0} on {1}", action, on.SafeName()));
            StyxWoW.Me.SetFocus(on);
            Lua.DoString("CastPetAction({0}, 'focus')", spell.ActionBarIndex + 1);
            StyxWoW.Me.SetFocus(0);
        }

        /// <summary>
        ///   Calls a pet by name, if applicable.
        /// </summary>
        /// <remarks>
        ///   Created 2/7/2011.
        /// </remarks>
        /// <param name = "petName">Name of the pet. This parameter is ignored for mages. Warlocks should pass only the name of the pet. Hunters should pass which pet (1, 2, etc)</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool CallPet(string petName)
        {
            if (!CallPetTimer.IsFinished)
            {
                return false;
            }

            switch (StyxWoW.Me.Class)
            {
                case WoWClass.Warlock:
                    if (SpellManager.CanCast("Summon " + petName))
                    {
                        Logger.Write(string.Format("[Pet] Calling out my {0}", petName));
                        return SpellManager.Cast("Summon " + petName);
                    }
                    break;

                case WoWClass.Mage:
                    if (SpellManager.CanCast("Summon Water Elemental"))
                    {
                        Logger.Write("[Pet] Calling out Water Elemental");
                        return SpellManager.Cast("Summon Water Elemental");
                    }
                    break;

                case WoWClass.Hunter:
                    if (SpellManager.CanCast("Call Pet " + petName))
                    {
                        if (!StyxWoW.Me.GotAlivePet)
                        {
                            Logger.Write(string.Format("[Pet] Calling out pet #{0}", petName));
                            return SpellManager.Cast("Call Pet " + petName);
                        }
                    }
                    break;
            }
            return false;
        }
    }
}