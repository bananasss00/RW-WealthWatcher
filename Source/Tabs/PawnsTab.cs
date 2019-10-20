using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public class PawnsTab : Tab
    {
        public new static readonly string CAPTION = "capPawnsTab".Translate();
        
        public override string Caption => CAPTION;

        public override void Update()
        {
            items = new List<WealthItem>();

            Map map = Find.CurrentMap;

            foreach (Pawn pawn in map.mapPawns.PawnsInFaction(Faction.OfPlayer))
            {
                items.AddWealth(pawn);
            }

            items.Sort((a, b) => b.MarketValueAll.CompareTo(a.MarketValueAll));
        }
    }
}