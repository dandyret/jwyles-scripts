﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/DeathKnight/Blood.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $LastChangedRevision: 265 $
// $Revision: 265 $

#endregion

using System.Linq;

using Styx.Combat.CombatRoutine;
using Styx.Logic.Combat;

using TreeSharp;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.DeathKnight)]
        [Spec(TalentSpec.BloodDeathKnight)]
        [Behavior(BehaviorType.Combat)]
        [Context(WoWContext.All)]
        public Composite CreateBloodDeathKnightCombat()
        {
            NeedTankTargeting = true;
            return new PrioritySelector(
                CreateEnsureTarget(),
                CreateAutoAttack(true),
                CreateFaceUnit(),
                // Blood DKs are tanks. NOT DPS. If you're DPSing as blood, go respec right now, because you fail hard.
                // Death Grip is used at all times in this spec, so don't bother with an instance check, like the other 2 specs.
                CreateSpellCast("Death Grip", ret => Me.CurrentTarget.Distance > 15),
                //Make sure we're in range, and facing the damned target. (LOS check as well)
                CreateMoveToAndFace(5f, ret => Me.CurrentTarget),
                CreateSpellBuffOnSelf("Bone Shield"),
                CreateSpellCast("Rune Strike"),
                CreateSpellCast("Mind Freeze", ret => Me.CurrentTarget.IsCasting || Me.CurrentTarget.ChanneledCastingSpellId != 0),
                CreateSpellCast("Strangulate", ret => Me.CurrentTarget.IsCasting || Me.CurrentTarget.ChanneledCastingSpellId != 0),
                CreateSpellBuffOnSelf("Rune Tap", ret => Me.HealthPercent <= 60),
                CreateSpellCast(
                    "Pestilence", ret => Me.CurrentTarget.HasAura("Blood Plague") && Me.CurrentTarget.HasAura("Frost Fever") &&
                                         (from add in NearbyUnfriendlyUnits
                                          where !add.HasAura("Blood Plague") && !add.HasAura("Frost Fever") && add.Distance < 10
                                          select add).Count() > 0),
                new Decorator(
                    ret => SpellManager.CanCast("Death and Decay") && NearbyUnfriendlyUnits.Count(a => a.Distance < 8) > 1,
                    new Action(
                        ret =>
                            {
                                SpellManager.Cast("Death and Decay");
                                LegacySpellManager.ClickRemoteLocation(Me.CurrentTarget.Location);
                            })),
                CreateSpellCast("Icy Touch"),
                CreateSpellCast("Plague Strike", ret => !Me.CurrentTarget.HasAura("Blood Plague")),
                CreateSpellCast("Death Strike", ret => Me.HealthPercent < 80),
                CreateSpellCast("Blood Boil", ret => NearbyUnfriendlyUnits.Count(a => a.Distance < 8) > 1),
                CreateSpellCast("Heart Strike"),
                CreateSpellCast("Death Coil"));
        }
    }
}