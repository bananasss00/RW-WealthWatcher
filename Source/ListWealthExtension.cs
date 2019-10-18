using System.Collections.Generic;
using Verse;

namespace WealthWatcher
{
    public static class ListExtension
    {
        public static void AddWealth(this List<WealthItem> l, Thing thing, bool building = false)
        {
            var label = building ? thing.LabelShort : thing.LabelNoCount;
            var item = l.Find(k => k.Name == label);
            if (item != null)
            {
                item.Count += thing.stackCount;
                if (building) // raid points calc for buildings with half marketMalue
                {
                    item.MarketValueAll += (thing.stackCount * thing.MarketValue) * 0.5f;
                }
                else
                {
                    item.MarketValueAll += thing.stackCount * thing.MarketValue;
                }
            }
            else
            {
                l.Add(new WealthItem()
                {
                    Name = label,
                    Count = thing.stackCount,
                    MarketValue = building ? thing.MarketValue * 0.5f : thing.MarketValue,
                    MarketValueAll = building ? (thing.stackCount * thing.MarketValue) * 0.5f : thing.stackCount * thing.MarketValue,
                    thing = thing
                });
            }
        }

        public static void AddWealth(this List<WealthItem> l, string name, float marketValueAll)
        {
            l.Add(new WealthItem()
            {
                Name = name,
                MarketValueAll = marketValueAll,
                thing = null
            });
        }
    }
}
