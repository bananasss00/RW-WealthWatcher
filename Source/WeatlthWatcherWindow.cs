using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace WealthWatcher
{
    public class WealthItem
    {
        public string Name;
        public int Count;
        public float MarketValue;
        public float MarketValueAll;
        public Thing thing;
    }

    public enum SelectedTab
    {
        None,
        Items,
        Buildings,
        RadePoints
    }

    public class WeatlthWatcherWindow : Window
    {
        private static SelectedTab currentTab = SelectedTab.None;
        private static List<WealthItem> wealthItemsList = new List<WealthItem>(), wealthBuildingsList = new List<WealthItem>();

        private Vector2 resultsAreaScroll;
        private RadePointsSummary rps;

        public override Vector2 InitialSize => new Vector2(640f, 480f);

        public override void PostClose()
        {
            base.PostClose();
        }

        public WeatlthWatcherWindow()
        {
            if (headerStyle is null)
            {
                headerStyle = new GUIStyle(GUI.skin.label);
                //headerStyle.normal.textColor = Color.cyan;
                headerStyle.fontStyle = FontStyle.BoldAndItalic;
            }

            optionalTitle = "WealthWatcher";
            preventCameraMotion = false;
            absorbInputAroundWindow = false;
            draggable = true;
            doCloseX = true;

            if (Settings.ShowRadePoints) // TODO: refactor this shits
            {
                IIncidentTarget incidentTarget = Find.CurrentMap;
                if (incidentTarget != null)
                {
                    // recalc colony wealth when open window
                    Find.CurrentMap?.wealthWatcher?.ForceRecount();
                    optionalTitle += ".   " + "CurrentRaidPoints".Translate() + ": " +
                                     StorytellerUtility.DefaultThreatPointsNow(incidentTarget).ToString("F0");

                    rps = RadePointsSummary.Get(Find.CurrentMap);
                }
            }

            switch (currentTab) 
            {
                case SelectedTab.Items:
                    UpdateItems();
                    break;
                case SelectedTab.Buildings:
                    UpdateBuildings();
                    break;
            }
        }

        private void ResetLists()
        {
            wealthItemsList.Clear();
            wealthBuildingsList.Clear();
        }

        private void UpdateItems()
        {
            ResetLists();

            Map map = Find.CurrentMap;
            float wealthItems = 0f;
            //Переносимые
            List<Thing> tmpThings = new List<Thing>();
            ThingOwnerUtility.GetAllThingsRecursively(map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), tmpThings, false, x =>
            {
                if (x is PassingShip || x is MapComponent)
                    return false;

                return !(x is Pawn pawn) || pawn.Faction == Faction.OfPlayer;
            });
            foreach (var thing in tmpThings)
            {
                if (thing.SpawnedOrAnyParentSpawned && !thing.PositionHeld.Fogged(map))
                {
                    wealthItems += thing.stackCount * thing.MarketValue;
                    wealthItemsList.AddWealth(thing);
                }
            }

            //Колонисты
            float wealthPawns = 0f;
            foreach (Pawn pawn in map.mapPawns.PawnsInFaction(Faction.OfPlayer))
            {
                wealthPawns += pawn.MarketValue;
            }
            wealthItemsList.AddWealth("Pawns".Translate(), wealthPawns);

            wealthItemsList.Sort((a, b) =>
            {
                return b.MarketValueAll.CompareTo(a.MarketValueAll);
            });
        }

        private void UpdateBuildings()
        {
            ResetLists();

            Map map = Find.CurrentMap;
            List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
            foreach(var thing in list)
            {
                if (thing.Faction == Faction.OfPlayer)
                {
                    wealthBuildingsList.AddWealth(thing, true);
                }
            }

            wealthBuildingsList.Sort((a, b) => b.MarketValueAll.CompareTo(a.MarketValueAll));
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;

            Listing_Standard listing = new Listing_Standard();
            listing.ColumnWidth = inRect.width / (rps != null ? 3.3f : 2.1f);
            listing.Begin(inRect);

            if (listing.ButtonText("ShowItems".Translate()))
            {
                currentTab = SelectedTab.Items;
                UpdateItems();
            }

            listing.NewColumn();

            if (listing.ButtonText("ShowBuildings".Translate()))
            {
                currentTab = SelectedTab.Buildings;
                UpdateBuildings();
            }

            if (rps != null)
            {
                listing.NewColumn();

                if (listing.ButtonText("ShowRaidPoints".Translate()))
                {
                    currentTab = SelectedTab.RadePoints;
                    ResetLists();
                }
            }

            listing.End();

            Rect scrollOut = inRect;
            scrollOut.y += listing.CurHeight + 2f;
            scrollOut.height = inRect.height - scrollOut.y - 2f;

            DrawRadePoints(scrollOut);
            DrawItemsBuildings(scrollOut);

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DrawRadePoints(Rect rect)
        {
            if (currentTab != SelectedTab.RadePoints)
                return;

            Listing_Standard listing2 = new Listing_Standard();
            listing2.Begin(rect);

            Text.Font = GameFont.Tiny;
            listing2.TextEntry("PointsFormula".Translate());
            Text.Font = GameFont.Small;
            listing2.Gap();
            listing2.Label("PointsLabel".Translate());
            listing2.LabelDouble("WealthLabel".Translate(), $"{rps.pointsWealth:F0}", "WealthDescription"
                .Translate($"{rps.wealthItems:F0}", $"{rps.wealthBuildings:F0}", $"{rps.wealthPawns:F0}"));
            listing2.LabelDouble("ColonistsLabel".Translate(), $"{rps.pointsColonist:F0}", rps.colonistsSummary);
            listing2.LabelDouble("AnimalsLabel".Translate(), $"{rps.pointsAnimal:F0}", rps.animalsSummary);

            listing2.Gap();
            listing2.Label("FactorsLabel".Translate());
            listing2.LabelDouble("AdaptationLabel".Translate(), $"{rps.adaptationFactor:F2}");
            listing2.LabelDouble("DifficultyLabel".Translate(), $"{rps.difficultyFactor:F2}");
            listing2.LabelDouble("DaysPassedLabel".Translate(), $"{rps.daysPassedFactor:F2}");

            listing2.End();
        }

        private void DrawItemsBuildings(Rect rect)
        {
            if (currentTab != SelectedTab.Items && currentTab != SelectedTab.Buildings)
                return;

            Rect scrollView = rect;
            scrollView.width -= 16f;
            float lineHeight = Text.LineHeight * 2.25f + 2f;
            float itemsHeightAll = wealthItemsList.Count > 0 ? (wealthItemsList.Count + 1) * lineHeight : 0f;
            float buildingsHeightAll = wealthBuildingsList.Count > 0 ? (wealthBuildingsList.Count + 1) * lineHeight : 0f;
            scrollView.height = itemsHeightAll + buildingsHeightAll;

            Widgets.BeginScrollView(rect, ref resultsAreaScroll, scrollView);
            Rect lineRect = scrollView;
            lineRect.height = lineHeight;

            if (wealthItemsList.Count > 0 || wealthBuildingsList.Count > 0)
                DrawHeadEntry(ref lineRect);

            foreach (var i in wealthItemsList)
            {
                DrawEntry(ref lineRect, i);
            }

            foreach (var i in wealthBuildingsList)
            {
                DrawEntry(ref lineRect, i);
            }

            
            Widgets.EndScrollView();
        }

        private void DrawEntry(ref Rect lineRect, WealthItem i)
        {
            if (i.thing != null && i.MarketValueAll < 1f)
                return;

            Widgets.DrawHighlightIfMouseover(lineRect);
            Rect iconRect = lineRect;
            iconRect.width = iconRect.height;
            if (i.thing != null)
            {
                Widgets.ThingIcon(iconRect, i.thing);
            }

            Rect nameRect = lineRect;
            nameRect.width = nameRect.width - iconRect.width - lineRect.height * 3f;
            nameRect.x += iconRect.width;
            Widgets.Label(nameRect, i.Name);

            Rect countRect = lineRect;
            countRect.width = countRect.height;
            countRect.x = nameRect.x + nameRect.width;
            if (i.thing != null)
            {
                Widgets.Label(countRect, "x" + i.Count);
            }

            Rect marketValueAllRect = lineRect;
            marketValueAllRect.width = marketValueAllRect.height;
            marketValueAllRect.x = countRect.x + countRect.width;
            Widgets.Label(marketValueAllRect, Mathf.RoundToInt(i.MarketValueAll).ToString());

            Rect infoButtonRect = lineRect;
            infoButtonRect.width = infoButtonRect.height;
            infoButtonRect.x = marketValueAllRect.x + marketValueAllRect.width;

            if (i.thing != null)
            {
                Widgets.InfoCardButton(infoButtonRect.x + infoButtonRect.width / 2f - 12f, infoButtonRect.y + infoButtonRect.height / 2f - 12f, i.thing);
                TooltipHandler.TipRegion(lineRect, i.thing.DescriptionFlavor);
            }

            lineRect.y += lineRect.height;
        }

        private void DrawHeadEntry(ref Rect lineRect)
        {
            Rect iconRect = lineRect;
            iconRect.width = iconRect.height;

            Rect nameRect = lineRect;
            nameRect.width = nameRect.width - iconRect.width - lineRect.height * 3f;
            nameRect.x += iconRect.width;
            GUI.Label(nameRect, "EntryLabel".Translate(), headerStyle);

            Rect countRect = lineRect;
            countRect.width = countRect.height;
            countRect.x = nameRect.x + nameRect.width;
            GUI.Label(countRect, "CountLabel".Translate(), headerStyle);

            Rect marketValueAllRect = lineRect;
            marketValueAllRect.width = marketValueAllRect.height;
            marketValueAllRect.x = countRect.x + countRect.width;
            GUI.Label(marketValueAllRect, "MarketValueLabel".Translate(), headerStyle);

            Rect infoButtonRect = lineRect;
            infoButtonRect.width = infoButtonRect.height;
            infoButtonRect.x = marketValueAllRect.x + marketValueAllRect.width;
            GUI.Label(infoButtonRect, "InfoLabel".Translate(), headerStyle);

            lineRect.y += lineRect.height;
        }

        private static GUIStyle headerStyle;
    }
}
