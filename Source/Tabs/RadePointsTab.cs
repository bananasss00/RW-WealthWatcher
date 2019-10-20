using RimWorld;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public class RadePointsTab : Tab
    {
        public new static readonly string CAPTION = "capRadePointsTab".Translate();
        
        public override string Caption => CAPTION;

        public override float ViewHeight => 350f;

        private RadePointsSummary rps;

        public override void Update()
        {
            IIncidentTarget incidentTarget = Find.CurrentMap;
            if (incidentTarget != null)
            {
                rps = RadePointsSummary.Get(Find.CurrentMap);
            }
        }

        public override void Draw(Rect viewRect)
        {
            if (rps == null) return;

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(viewRect);

            //Text.Font = GameFont.Tiny;
            listingStandard.TextEntry("PointsFormula".Translate());
            //Text.Font = GameFont.Small;
            listingStandard.Gap();
            listingStandard.Label("PointsLabel".Translate());
            listingStandard.LabelDouble("WealthLabel".Translate(), $"{rps.pointsWealth:F0}", "WealthDescription"
                .Translate($"{rps.wealthItems:F0}", $"{rps.wealthBuildings:F0}", $"{rps.wealthPawns:F0}"));
            listingStandard.LabelDouble("ColonistsLabel".Translate(), $"{rps.pointsColonist:F0}", rps.colonistsSummary);
            listingStandard.LabelDouble("AnimalsLabel".Translate(), $"{rps.pointsAnimal:F0}", rps.animalsSummary);

            listingStandard.Gap();
            listingStandard.Label("FactorsLabel".Translate());
            listingStandard.LabelDouble("AdaptationLabel".Translate(), $"{rps.adaptationFactor:F2}");
            listingStandard.LabelDouble("DifficultyLabel".Translate(), $"{rps.difficultyFactor:F2}");
            listingStandard.LabelDouble("DaysPassedLabel".Translate(), $"{rps.daysPassedFactor:F2}");

            listingStandard.End();
        }
    }
}