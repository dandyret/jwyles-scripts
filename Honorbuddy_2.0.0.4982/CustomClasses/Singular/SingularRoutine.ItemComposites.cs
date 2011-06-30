﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: exemplar $
// $Date: 2011-04-14 11:12:40 +0200 (to, 14 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/SingularRoutine.ItemComposites.cs $
// $LastChangedBy: exemplar $
// $LastChangedDate: 2011-04-14 11:12:40 +0200 (to, 14 apr 2011) $
// $LastChangedRevision: 281 $
// $Revision: 281 $

#endregion

using System;
using System.Linq;

using CommonBehaviors.Actions;

using Singular.Settings;

using Styx;
using Styx.WoWInternals.WoWObjects;

using TreeSharp;

using Action = TreeSharp.Action;

namespace Singular
{
    partial class SingularRoutine
    {
        public Composite CreateUseAlchemyBuffsBehavior()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    SingularSettings.Instance.UseAlchemyFlaskOfEnhancement && Me.GetSkill(SkillLine.Alchemy).CurrentValue >= 400 &&
                    !Me.Auras.Any(aura => aura.Key.StartsWith("Enhanced ") || aura.Key.StartsWith("Flask of ")), // don't try to use the flask if we already have or if we're using a better one
                    new PrioritySelector(
                        ctx => Me.CarriedItems.FirstOrDefault(i => i.Entry == 58149) ?? Me.CarriedItems.FirstOrDefault(i => i.Entry == 47499),
                        // Flask of Enhancement / Flask of the North
                        new Decorator(
                            ret => ret != null,
                            new Sequence(
                                new Action(ret => Logger.Write(String.Format("Using {0}", ((WoWItem)ret).Name))),
                                new Action(ret => ((WoWItem)ret).UseContainerItem()),
                                new Action(ret => StyxWoW.SleepForLagDuration())))
                        ))
                );
        }

        public Composite CreateUseTrinketsBehavior()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => SingularSettings.Instance.UseFirstTrinket,
                    new Decorator(
                        ret => Miscellaneous.UseTrinket(true),
                        new ActionAlwaysSucceed())),
                new Decorator(
                    ret => SingularSettings.Instance.UseSecondTrinket,
                    new Decorator(
                        ret => Miscellaneous.UseTrinket(false),
                        new ActionAlwaysSucceed()))
                );
        }

        public Composite CreateUseEquippedItem(uint slotId)
        {
            return new PrioritySelector(
                new Decorator(
                    ret => Miscellaneous.UseEquippedItem(slotId),
                    new ActionAlwaysSucceed()));
        }

        /// <summary>
        ///   Creates a composite to use potions and healthstone.
        /// </summary>
        /// <param name = "healthPercent">Healthpercent to use health potions and healthstone</param>
        /// <param name = "manaPercent">Manapercent to use mana potions</param>
        /// <returns></returns>
        public Composite CreateUsePotionAndHealthstone(double healthPercent, double manaPercent)
        {
            return new PrioritySelector(
                new Decorator(
                    ret => Me.HealthPercent < healthPercent,
                    new PrioritySelector(
                        ctx => Miscellaneous.FindFirstUsableItemBySpell("Healthstone", "Healing Potion"),
                        new Decorator(
                            ret => ret != null,
                            new Sequence(
                                new Action(ret => Logger.Write(String.Format("Using {0}", ((WoWItem)ret).Name))),
                                new Action(ret => ((WoWItem)ret).UseContainerItem()),
                                new Action(ret => StyxWoW.SleepForLagDuration())))
                        )),
                new Decorator(
                    ret => Me.ManaPercent < manaPercent,
                    new PrioritySelector(
                        ctx => Miscellaneous.FindFirstUsableItemBySpell("Restore Mana"),
                        new Decorator(
                            ret => ret != null,
                            new Sequence(
                                new Action(ret => Logger.Write(String.Format("Using {0}", ((WoWItem)ret).Name))),
                                new Action(ret => ((WoWItem)ret).UseContainerItem()),
                                new Action(ret => StyxWoW.SleepForLagDuration())))))
                );
        }
    }
}