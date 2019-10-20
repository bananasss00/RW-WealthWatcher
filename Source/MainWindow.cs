using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WealthWatcher
{
    using Tabs;

    public class MainWindow : Window
    {
        private static ITab _activeTab = null;
        private Vector2 _scrollPosition = new Vector2(0, 0);
        private string _raidPoints = null;

        public override Vector2 InitialSize => new Vector2(640f, 480f);

        public MainWindow()
        {
            //optionalTitle = "WealthWatcher";
            preventCameraMotion = false;
            absorbInputAroundWindow = false;
            draggable = true;
            doCloseX = true;
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
        }

        public override void DoWindowContents(Rect rect)
        {
            float y = 0f;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            // draw button select tab
            float btnWidth = 200f;
            float btnHeight = 30f;
            Rect btnSelectTabRect = new Rect(x: 10f, y: y, width: btnWidth, height: btnHeight);
            string btnCaption = _activeTab?.Caption ?? "capSelectTab".Translate();
            
            if (Widgets.ButtonText(btnSelectTabRect, "btnSelectTab".Translate(btnCaption)))
            {
                Find.WindowStack.Add( new FloatMenu(this.GetTabsList().ToList()) );
            }

            // draw raid points
            if (_raidPoints != null)
            {
                Rect labelRaidPointsRect = new Rect(x: 10f + btnWidth + 10, y: y, width: 200f, height: Text.LineHeight);
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
    }
}