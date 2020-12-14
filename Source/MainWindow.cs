using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using WealthWatcher.Tabs.Sorting;

namespace WealthWatcher
{
    using Tabs;

    public class MainWindow : Window
    {
        private static ITab _activeTab = null;
        private Vector2 _scrollPosition = new Vector2(0, 0);
        private string _raidPoints = null;
        private TabComparer _sort1 = new TabComparer_MarketValueAll(), _sort2 = new TabComparer_None();
        private List<TabComparer> _listComparers;

        public override Vector2 InitialSize => new Vector2(740f, 580f);

        public MainWindow()
        {
            //optionalTitle = "WealthWatcher";
            preventCameraMotion = false;
            absorbInputAroundWindow = false;
            draggable = true;
            doCloseX = true;
            _listComparers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x != typeof(TabComparer) && typeof(TabComparer).IsAssignableFrom(x))
                .Select(x => (TabComparer)Activator.CreateInstance(x))
                .ToList();
        }

        public override void PostClose()
        {
            base.PostClose();
            _activeTab?.Close();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            if (Settings.ShowRadePoints)
            {
                IIncidentTarget incidentTarget = Find.CurrentMap;
                if (incidentTarget != null)
                {
                    Find.CurrentMap?.wealthWatcher?.ForceRecount();
                    _raidPoints = "CurrentRaidPoints".Translate(
                        StorytellerUtility.DefaultThreatPointsNow(incidentTarget).ToString("F0"));
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;

            _activeTab?.Update();
            _activeTab?.Sort(_sort1, _sort2);
        }

        public override void DoWindowContents(Rect rect)
        {
            float y = 0f;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            // draw button select tab
            float btnWidth = 150f;
            float btnHeight = 30f;
            float lblOrderWidth = 80f;
            float btnOrderWidth = 120f;
            Rect btnSelectTabRect = new Rect(x: 10f, y: y, width: btnWidth, height: btnHeight);
            Rect lblOrderRect = new Rect(x: btnSelectTabRect.xMax + 10f, y: y, width: lblOrderWidth, height: btnHeight);
            Rect btnOrder1Rect = new Rect(x: lblOrderRect.xMax, y: y, width: btnOrderWidth, height: btnHeight);
            Rect btnOrder2Rect = new Rect(x: btnOrder1Rect.xMax, y: y, width: btnOrderWidth, height: btnHeight);
            string btnCaption = _activeTab?.Caption ?? "capSelectTab".Translate();
            
            if (Widgets.ButtonText(btnSelectTabRect, "btnSelectTab".Translate(btnCaption)))
            {
                Find.WindowStack.Add( new FloatMenu(this.GetTabsList().ToList()));
            }

            Widgets.Label(lblOrderRect, "lblOrder".Translate());

            if (Widgets.ButtonText(btnOrder1Rect, _sort1.Name))
            {
                Find.WindowStack.Add(new FloatMenu(this.GetSortersList(comparer =>
                {
                    _sort1 = comparer;
                    _activeTab?.Sort(_sort1, _sort2);
                }).ToList()));
            }
            if (Widgets.ButtonText(btnOrder2Rect, _sort2.Name))
            {
                Find.WindowStack.Add(new FloatMenu(this.GetSortersList(comparer =>
                {
                    _sort2 = comparer;
                    _activeTab?.Sort(_sort1, _sort2);
                }).ToList()));
            }

            // draw raid points
            if (_raidPoints != null)
            {
                Rect labelRaidPointsRect = new Rect(x: btnOrder2Rect.xMax + 10f, y: y, width: 100f, height: btnHeight);
                Widgets.Label(labelRaidPointsRect, _raidPoints);
            }

            y += btnHeight + 20f;

            // draw content in scroll view
            Rect outRect = new Rect(x: 0f, y: y, width: rect.width, height: rect.height - y);
            Rect viewRect = new Rect(x: 0f, y: y, width: rect.width - 30f, height: _activeTab?.ViewHeight ?? 0f);

            Widgets.BeginScrollView(outRect: outRect, scrollPosition: ref _scrollPosition, viewRect: viewRect);
            _activeTab?.Draw(outRect, viewRect, _scrollPosition);
            Widgets.EndScrollView();

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void SelectTab<T>() where T: ITab, new()
        {
            _activeTab = new T();
            _activeTab.Update();
            _activeTab.Sort(_sort1, _sort2);
        }

        private IEnumerable<FloatMenuOption> GetTabsList()
        {
            yield return new FloatMenuOption("capSelectTab".Translate(), () => _activeTab = null);
            yield return new FloatMenuOption(ItemsTab.CAPTION, SelectTab<ItemsTab>);
            yield return new FloatMenuOption(BuildingsTab.CAPTION, SelectTab<BuildingsTab>);
            yield return new FloatMenuOption(PawnsTab.CAPTION, SelectTab<PawnsTab>);
            
            if (Settings.ShowRadePoints)
                yield return new FloatMenuOption(RadePointsTab.CAPTION, SelectTab<RadePointsTab>);
        }

        private IEnumerable<FloatMenuOption> GetSortersList(Action<TabComparer> setSort)
        {
            foreach (var comparer in _listComparers)
            {
                yield return new FloatMenuOption(comparer.Name, () => setSort(comparer));
            }
        }
    }
}