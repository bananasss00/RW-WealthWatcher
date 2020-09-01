using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace WealthWatcher
{
    internal class WealthWatcherMod : Mod
    {
        public WealthWatcherMod(ModContentPack content) : base(content)
        {
            new Harmony("pirateby.wealthwatcher").PatchAll(Assembly.GetExecutingAssembly());
            GetSettings<Settings>();
            Log.Message($"WealthWatcher :: Initialized");
        }

        public override void DoSettingsWindowContents(Rect inRect) => Settings.DoSettingsWindowContents(inRect);

        public override string SettingsCategory() => "WealthWatcher";
    }
}
