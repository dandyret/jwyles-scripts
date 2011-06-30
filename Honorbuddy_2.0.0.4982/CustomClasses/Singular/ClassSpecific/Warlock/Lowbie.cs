﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Warlock/Lowbie.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $LastChangedRevision: 265 $
// $Revision: 265 $

#endregion

using Styx.Combat.CombatRoutine;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.Warlock)]
        [Spec(TalentSpec.Lowbie)]
        [Context(WoWContext.All)]
        [Behavior(BehaviorType.Combat)]
        [Behavior(BehaviorType.Pull)]
        public Composite CreateLowbieWarlockCombat()
        {
            WantedPet = "Imp";

            return new PrioritySelector(
                CreateEnsureTarget(),
                CreateMoveToAndFace(35f, ret => Me.CurrentTarget),
                CreateAutoAttack(true),
                CreateWaitForCast(true),
                CreateSpellCast("Life Tap", ret => Me.ManaPercent < 50 && Me.HealthPercent > 70),
                CreateSpellCast("Drain Life", ret => Me.HealthPercent < 70),
                CreateSpellBuff("Immolate", true),
                CreateSpellBuff("Corruption"),
                CreateSpellCast("Shadow Bolt")
                );
        }
    }
}