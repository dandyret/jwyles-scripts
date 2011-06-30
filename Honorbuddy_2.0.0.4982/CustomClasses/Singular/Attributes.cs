﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: exemplar $
// $Date: 2011-04-14 11:12:40 +0200 (to, 14 apr 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/Attributes.cs $
// $LastChangedBy: exemplar $
// $LastChangedDate: 2011-04-14 11:12:40 +0200 (to, 14 apr 2011) $
// $LastChangedRevision: 281 $
// $Revision: 281 $

#endregion

using System;

using Styx.Combat.CombatRoutine;

namespace Singular
{
    [Flags]
    public enum WoWContext
    {
        Normal = 0x1,
        Instances = 0x2,
        Battlegrounds = 0x4,

        All = Normal | Instances | Battlegrounds,
    }

    public enum BehaviorType
    {
        Rest,
        PreCombatBuffs,
        PullBuffs,
        Pull,
        Heal,
        CombatBuffs,
        Combat,
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int thePriority)
        {
            PriorityLevel = thePriority;
        }

        public int PriorityLevel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class ClassAttribute : Attribute
    {
        public ClassAttribute(WoWClass specificClass)
        {
            SpecificClass = specificClass;
        }

        public WoWClass SpecificClass { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class BehaviorAttribute : Attribute
    {
        public BehaviorAttribute(BehaviorType type)
        {
            Type = type;
        }

        public BehaviorType Type { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class SpecAttribute : Attribute
    {
        public SpecAttribute(TalentSpec spec)
        {
            SpecificSpec = spec;
        }

        public TalentSpec SpecificSpec { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class ContextAttribute : Attribute
    {
        public ContextAttribute(WoWContext context)
        {
            SpecificContext = context;
        }

        public WoWContext SpecificContext { get; private set; }
    }
}