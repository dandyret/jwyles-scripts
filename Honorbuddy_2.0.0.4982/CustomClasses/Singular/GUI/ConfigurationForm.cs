﻿#region Revision Info

// This file is part of Singular - A community driven Honorbuddy CC
// $Author: apoc $
// $Date: 2011-03-27 01:02:25 +0100 (sö, 27 mar 2011) $
// $HeadURL: http://svn.apocdev.com/singular/tags/v1/Singular/GUI/ConfigurationForm.cs $
// $LastChangedBy: apoc $
// $LastChangedDate: 2011-03-27 01:02:25 +0100 (sö, 27 mar 2011) $
// $LastChangedRevision: 235 $
// $Revision: 235 $

#endregion

using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Singular.ClassSpecific.Shaman;
using Singular.Settings;

using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Logic;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Singular.GUI
{
    public partial class ConfigurationForm : Form
    {
        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            lblVersion.Text = string.Format("v{0}", Assembly.GetExecutingAssembly().GetName().Version) + " [$Revision: 235 $]";
            //HealTargeting.Instance.OnTargetListUpdateFinished += new Styx.Logic.TargetListUpdateFinishedDelegate(Instance_OnTargetListUpdateFinished);
            pgGeneral.SelectedObject = SingularSettings.Instance;
            SingularSettings main = SingularSettings.Instance;
            Styx.Helpers.Settings toSelect = null;
            switch (StyxWoW.Me.Class)
            {
                case WoWClass.Warrior:
                    toSelect = main.Warrior;
                    break;
                case WoWClass.Paladin:
                    toSelect = main.Paladin;
                    break;
                case WoWClass.Hunter:
                    toSelect = main.Hunter;
                    break;
                case WoWClass.Rogue:
                    toSelect = main.Rogue;
                    break;
                case WoWClass.Priest:
                    toSelect = main.Priest;
                    break;
                case WoWClass.DeathKnight:
                    toSelect = main.DeathKnight;
                    break;
                case WoWClass.Shaman:
                    toSelect = main.Shaman;
                    break;
                case WoWClass.Mage:
                    toSelect = main.Mage;
                    break;
                case WoWClass.Warlock:
                    toSelect = main.Warlock;
                    break;
                case WoWClass.Druid:
                    toSelect = main.Druid;
                    break;
                default:
                    break;
            }
            if (toSelect != null)
            {
                pgClass.SelectedObject = toSelect;
            }
        }

        private void Instance_OnTargetListUpdateFinished(object context)
        {
            if (InvokeRequired)
            {
                Invoke(new TargetListUpdateFinishedDelegate(Instance_OnTargetListUpdateFinished), context);
                return;
            }

            var sb = new StringBuilder();
            foreach (WoWPlayer u in HealTargeting.Instance.HealList)
            {
                sb.AppendLine(u.Name + " - " + u.HealthPercent);
            }
            lblHealTargets.Text = sb.ToString();
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            ((Styx.Helpers.Settings)pgGeneral.SelectedObject).Save();
            if (pgClass.SelectedObject != null)
            {
                ((Styx.Helpers.Settings)pgClass.SelectedObject).Save();
            }
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!ObjectManager.IsInGame)
                return;

            var sb = new StringBuilder();
            foreach (WoWPlayer u in HealTargeting.Instance.HealList.ToArray())
            {
                sb.AppendLine(u.Name + " - " + u.HealthPercent);
            }
            lblHealTargets.Text = sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ObjectManager.Update();
            //TotemManager.RecallTotems();
            Logger.Write(TotemManager.TotemsInRange + " totems in range.");
        }
    }
}