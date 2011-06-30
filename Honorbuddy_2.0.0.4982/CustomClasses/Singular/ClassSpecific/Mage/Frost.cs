﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: raphus $
// $Date: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/ClassSpecific/Mage/Frost.cs $
// $LastChangedBy: raphus $
// $LastChangedDate: 2011-04-10 20:54:20 +0200 (sö, 10 apr 2011) $
// $LastChangedRevision: 265 $
// $Revision: 265 $

#endregion

using System.Linq;

using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic.Pathing;

using TreeSharp;
using Styx.WoWInternals.WoWObjects;

namespace Singular
{
    partial class SingularRoutine
    {
        [Class(WoWClass.Mage)]
        [Spec(TalentSpec.FrostMage)]
        [Behavior(BehaviorType.Combat)]
        [Context(WoWContext.All)]
        public Composite CreateFrostMageCombat()
        {
            WantedPet = "Water Elemental";
            return new PrioritySelector(
                CreateEnsureTarget(),
                //Move away from frozen targets
                new Decorator(
                    ret => (Me.CurrentTarget.HasAura("Frost Nova") || Me.CurrentTarget.HasAura("Freeze")) && Me.CurrentTarget.DistanceSqr < 5 * 5,
                    new Action(
                        ret =>
                            {
                                Logger.Write("Getting away from frozen target");
                                WoWPoint moveTo = WoWMathHelper.CalculatePointFrom(Me.Location, Me.CurrentTarget.Location, 10f);

                                if (Navigator.CanNavigateFully(Me.Location, moveTo))
                                {
                                    Navigator.MoveTo(moveTo);
                                }
                            })),
                CreateMoveToAndFace(34f, ret => Me.CurrentTarget),
                CreateWaitForCast(true),
                CreateCastPetActionOnLocation("Freeze", ret => !Me.CurrentTarget.HasAura("Frost Nova")),
                CreateSpellBuff("Frost Nova", ret => NearbyUnfriendlyUnits.Any(u => u.DistanceSqr <= 8 * 8)),
                CreateMagePolymorphOnAddBehavior(),
                new Decorator(
                    ret => !Me.GotAlivePet,
                    new Action(ret => PetManager.CallPet(WantedPet))),
                CreateSpellCast("Evocation", ret => Me.ManaPercent < 20),
                CreateSpellCast("Counterspell", ret => Me.CurrentTarget.IsCasting),
                CreateSpellCast("Mirror Image"),
                CreateSpellCast("Time Warp"),
                new Decorator(
                    ret => Me.CurrentTarget.HealthPercent > 50,
                    new Sequence(
                        new Action(ctx => Me.CurrentTarget.Face()),
                        new Action(ctx => StyxWoW.SleepForLagDuration()),
                        new PrioritySelector(CreateSpellCast("Flame Orb"))
                        )),
                CreateSpellBuffOnSelf("Ice Barrier", ret => !Me.Auras.ContainsKey("Mana Shield")),
                CreateSpellBuffOnSelf("Mana Shield", ret => !Me.Auras.ContainsKey("Ice Barrier") && Me.HealthPercent <= 50),
                CreateSpellCast(
                    "Deep Freeze",
                    ret =>
                    (Me.ActiveAuras.ContainsKey("Fingers of Frost") || Me.CurrentTarget.HasAura("Frost Nova") || Me.CurrentTarget.HasAura("Freeze"))),
                CreateSpellCast(
                    "Ice Lance",
                    ret =>
                    (Me.ActiveAuras.ContainsKey("Fingers of Frost") || Me.CurrentTarget.ActiveAuras.ContainsKey("Frost Nova") ||
                     Me.CurrentTarget.ActiveAuras.ContainsKey("Freeze"))),
                //CreateSpellCast("Fireball", Me.ActiveAuras.ContainsKey("Brain Freeze")),
                CreateSpellCast("Arcane Missiles", ret => Me.ActiveAuras.ContainsKey("Arcane Missiles!")),
                new Decorator(
                    ret => Me.ActiveAuras.ContainsKey("Brain Freeze"),
                    new PrioritySelector(
                        CreateSpellCast("Frostfire Bolt"),
                        CreateSpellCast("Fireball")
                        )),
                CreateSpellBuff("Fire Blast", ret => Me.CurrentTarget.HealthPercent < 10),
                CreateSpellCast("Frostbolt")
                );
        }

        [Class(WoWClass.Mage)]
        [Spec(TalentSpec.FrostMage)]
        [Behavior(BehaviorType.Pull)]
        [Context(WoWContext.All)]
        public Composite CreateFrostMagePull()
        {
            return
                new PrioritySelector(
                    // Make sure we're in range, and facing the damned target. (LOS check as well)
                    CreateMoveToAndFace(34f, ret => Me.CurrentTarget),
                    CreateSpellCast("Arcane Missiles", ret => Me.HasAura("Arcane Missiles!")),
                    CreateSpellCast("Frostbolt")
                    );
        }

        //[Class(WoWClass.Mage)]
        //[Spec(TalentSpec.FrostMage)]
        //[Behavior(BehaviorType.PreCombatBuffs)]
        //[Context(WoWContext.All)]
        //public Composite CreateFrostMagePreCombatBuffs()
        //{
        //    return
        //        new PrioritySelector(
        //            CreateSpellBuffOnSelf("Arcane Brilliance", ret => (!Me.HasAura("Arcane Brilliance") && !Me.HasAura("Fel Intelligence"))),
        //            CreateSpellBuffOnSelf("Molten Armor", ret => (!Me.HasAura("Molten Armor"))));
        //}
    }
}