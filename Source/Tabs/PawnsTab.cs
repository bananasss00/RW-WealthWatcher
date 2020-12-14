using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public class PawnsTab : WealthTab, ITab
    {
        public static readonly string CAPTION = "capPawnsTab".Translate();
        
        public string Caption => CAPTION;

        public void Update()
        {
            items = new List<WealthItem>();

            Map map = Find.CurrentMap;

            foreach (Pawn pawn in map.mapPawns.PawnsInFaction(Faction.OfPlayer))
            {
                items.AddWealth(pawn);
            }

            //items.Sort((a, b) => b.MarketValueAll.CompareTo(a.MarketValueAll));
        }
    }
}