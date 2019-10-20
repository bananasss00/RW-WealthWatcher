using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WealthWatcher
{
    public class Settings : ModSettings
	{
	    public static bool ShowRadePoints = true;
		public static bool UnlockManualDamageItems = false;
		public static bool Debug = false;
		
		public static void DoSettingsWindowContents(Rect rect)
		{
			Listing_Standard modOptions = new Listing_Standard();

			modOptions.Begin(rect);
			modOptions.Gap(20f);

		    modOptions.CheckboxLabeled("Settings_ShowRadePoints".Translate(), ref ShowRadePoints);
		    
		    if (TargetingParameters_CanTarget_Patch.IsPatched)
		    {
		        modOptions.CheckboxLabeled("Settings_UnlockManualDamageItems".Translate(), ref UnlockManualDamageItems);
		    }

            if (Prefs.DevMode)
                modOptions.CheckboxLabeled("Debug mode", ref Debug);

		    modOptions.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ShowRadePoints, "ShowRadePoints", true);
			Scribe_Values.Look(ref UnlockManualDamageItems, "UnlockManualDamageItems", false);
		}
	}
}