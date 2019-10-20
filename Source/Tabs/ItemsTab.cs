using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public class ItemsTab : Tab
    {
        public new static readonly string CAPTION = "capItemsTab".Translate();
        
        public override string Caption => CAPTION;

        public override void Update()
        {
            items = new List<WealthItem>();

            Map map = Find.CurrentMap;
            List<Thing> tmpThings = new List<Thing>();

            ThingOwnerUtility.GetAllThingsRecursively(map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), tmpThings, false, thing =>
            {
                return !(thing is PassingShip) && !(thing is MapComponent) && (!(thing is Pawn pawn) || pawn.Faction == Faction.OfPlayer);
            });

            //float wealthItems = 0f;
            tmpThings.ForEach(thing =>
            {
                if (!thing.SpawnedOrAnyParentSpawned || thing.PositionHeld.Fogged(map)) return;

                //wealthItems += thing.stackCount * thing.MarketValue;
                items.AddWealth(thing);
            });

            items.Sort((a, b) => b.MarketValueAll.CompareTo(a.MarketValueAll));
        }
    }
}