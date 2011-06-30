﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: nuok $
// $Date: 2011-03-21 15:50:52 +0100 (må, 21 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/DeathKnight/Frost.cs $
// $LastChangedBy: nuok $
// $LastChangedDate: 2011-03-21 15:50:52 +0100 (må, 21 mar 2011) $
// $LastChangedRevision: 199 $
// $Revision: 199 $

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
        [Spec(TalentSpec.FrostDeathKnight)]
        [Behavior(BehaviorType.Combat)]
        [Context(WoWContext.All)]
        public Composite CreateFrostDeathKnightCombat()
        {
            return new PrioritySelector(
                CreateEnsureTarget(),
                CreateAutoAttack(true),
                CreateFaceUnit(),
                // Note: You should have this in 2 different methods. Hence the reason for WoWContext being a [Flags] enum.
                // In this case, since its only one spell being changed, we can live with it.
                CreateSpellCast("Death Grip", ret => Me.CurrentTarget.Distance > 15 && !Me.IsInInstance),
                //Make sure we're in range, and facing the damned target. (LOS check as well)
                CreateMoveToAndFace(5f, ret => Me.CurrentTarget),
                CreateSpellCast("Raise Dead", ret => !Me.GotAlivePet),
                CreateSpellCast("Rune Strike"),
                CreateSpellCast("Mind Freeze", ret => Me.CurrentTarget.IsCasting || Me.CurrentTarget.ChanneledCastingSpellId != 0),
                CreateSpellCast("Strangulate", ret => Me.CurrentTarget.IsCasting || Me.CurrentTarget.ChanneledCastingSpellId != 0),
                CreateSpellCast("Death Strike", ret => Me.HealthPercent < 80),
                CreateSpellCast("Pillar of Frost"),
                CreateSpellCast("Howling Blast", ret => Me.HasAura("Freezing Fog") || !Me.CurrentTarget.HasAura("Frost Fever")),
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
                CreateSpellCast("Outbreak", ret => Me.CurrentTarget.HasAura("Frost Fever") || Me.CurrentTarget.HasAura("Blood Plague")),
                CreateSpellCast("Plague Strike", ret => !Me.CurrentTarget.HasAura("Blood Plague")),
                CreateSpellCast(
                    "Obliterate",
                    ret => (Me.FrostRuneCount == 2 && Me.UnholyRuneCount == 2) || Me.DeathRuneCount == 2 || Me.HasAura("Killing Machine")),
                CreateSpellCast("Blood Strike", ret => Me.BloodRuneCount == 2),
                CreateSpellCast("Frost Strike", ret => Me.HasAura("Freezing Fog") || Me.CurrentRunicPower == Me.MaxRunicPower),
                CreateSpellCast("Blood Tap", ret => Me.BloodRuneCount < 2),
                CreateSpellCast("Obliterate"),
                CreateSpellCast("Blood Strike"),
                CreateSpellCast("Frost Strike"));
        }
    }
}