﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-04-10 20:54:58 +0200 (sö, 10 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Hunter/Lowbie.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-04-10 20:54:58 +0200 (sö, 10 apr 2011) $
// $LastChangedRevision: 266 $
// $Revision: 266 $

#endregion

using Styx.Combat.CombatRoutine;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.Hunter)]
        [Spec(TalentSpec.Lowbie)]
        [Behavior(BehaviorType.Combat)]
        [Behavior(BehaviorType.Pull)]
        [Context(WoWContext.All)]
        public Composite CreateLowbieCombat()
        {
            WantedPet = "1";
            return new PrioritySelector(
                CreateEnsureTarget(),
                CreateWaitForCast(true),
                CreateHunterBackPedal(),
                CreateMoveToAndFace(35f, ret => Me.CurrentTarget),
                CreateSpellCast("Raptor Strike", ret => Me.CurrentTarget.DistanceSqr < 5 * 5),
                // Always keep it up on our target!
                CreateSpellBuff("Hunter's Mark"),
                // Heal pet when below 70
                CreateSpellCast("Mend Pet", ret => Me.Pet.HealthPercent < 70 && !Me.Pet.HasAura("Mend Pet")),
                CreateSpellCast(
                    "Concussive Shot",
                    ret => Me.CurrentTarget.CurrentTarget == null || Me.CurrentTarget.CurrentTarget == Me),
                CreateSpellCast("Arcane Shot"),
                CreateSpellCast("Steady Shot"),
				CreateAutoAttack(true)
                );
        }
    }
}