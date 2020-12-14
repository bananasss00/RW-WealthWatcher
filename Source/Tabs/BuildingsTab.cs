using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public class BuildingsTab : WealthTab, ITab
    {
        public static readonly string CAPTION = "capBuildingsTab".Translate();
        
        public string Caption => CAPTION;

        public void Update()
        {
            items = new List<WealthItem>();

            Map map = Find.CurrentMap;
            List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);

            list.ForEach(thing =>
            {
                if (thing.Faction != Faction.OfPlayer) return;
                items.AddWealth(thing, true);
            });

            //items.Sort((a, b) => b.MarketValueAll.CompareTo(a.MarketValueAll));
        }
    }
}