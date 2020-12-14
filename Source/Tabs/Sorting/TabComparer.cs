using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WealthWatcher.Tabs.Sorting
{
    public abstract class TabComparer : IComparer<WealthItem>
    {
        public abstract string Name { get; }
        public abstract int Compare(WealthItem lhs, WealthItem rhs);
    }

    public class TabComparer_None : TabComparer
    {
        public override string Name => "TabComparer_None".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return 0;
        }
    }

    public class TabComparer_MarketValue : TabComparer
    {
        public override string Name => "TabComparer_MarketValue".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return lhs.MarketValue.CompareTo(rhs.MarketValue);
        }
    }

    public class TabComparer_MarketValueAll : TabComparer
    {
        public override string Name => "TabComparer_MarketValueAll".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return lhs.MarketValueAll.CompareTo(rhs.MarketValueAll);
        }
    }

    public class TabComparer_Count : TabComparer
    {
        public override string Name => "TabComparer_Count".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return lhs.Count.CompareTo(rhs.Count);
        }
    }

    public class TabComparer_Name : TabComparer
    {
        public override string Name => "TabComparer_Name".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return lhs.Name.CompareTo(rhs.Name);
        }
    }

    public class TabComparer_Category : TabComparer
    {
        public override string Name => "TabComparer_Category".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return TransferableComparer_Category.Compare(lhs.thing.def, rhs.thing.def);
        }

        public static int Compare(ThingDef lhsTh, ThingDef rhsTh)
        {
            if (lhsTh.category != rhsTh.category) return lhsTh.category.CompareTo(rhsTh.category);
            var num = TransferableUIUtility.DefaultListOrderPriority(lhsTh);
            var num2 = TransferableUIUtility.DefaultListOrderPriority(rhsTh);
            if (num != num2) return num.CompareTo(num2);
            var num3 = 0;
            if (!lhsTh.thingCategories.NullOrEmpty()) num3 = lhsTh.thingCategories[0].index;
            var value = 0;
            if (!rhsTh.thingCategories.NullOrEmpty()) value = rhsTh.thingCategories[0].index;
            return num3.CompareTo(value);
        }
    }

    public class TabComparer_HitPointsPercentage : TabComparer
    {
        public override string Name => "TabComparer_HitPointsPercentage".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return this.GetValueFor(lhs).CompareTo(this.GetValueFor(rhs));
        }

        private float GetValueFor(WealthItem t)
        {
            Thing anyThing = t.thing;
            Pawn pawn = anyThing as Pawn;
            if (pawn != null)
            {
                return pawn.health.summaryHealth.SummaryHealthPercent;
            }
            if (!anyThing.def.useHitPoints || !anyThing.def.healthAffectsPrice)
            {
                return 1f;
            }
            return (float)anyThing.HitPoints / (float)anyThing.MaxHitPoints;
        }
    }

    public class TabComparer_Quality : TabComparer
    {
        public override string Name => "TabComparer_Quality".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return this.GetValueFor(lhs).CompareTo(this.GetValueFor(rhs));
        }

        private int GetValueFor(WealthItem t)
        {
            QualityCategory result;
            if (!t.thing.TryGetQuality(out result))
            {
                return -1;
            }
            return (int)result;
        }
    }
	
    public class TabComparer_Mass : TabComparer
    {
        public override string Name => "TabComparer_Mass".Translate();

        public override int Compare(WealthItem lhs, WealthItem rhs)
        {
            return lhs.thing.GetStatValue(StatDefOf.Mass, true).CompareTo(rhs.thing.GetStatValue(StatDefOf.Mass, true));
        }
    }
}