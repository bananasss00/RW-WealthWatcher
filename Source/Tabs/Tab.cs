using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public abstract class Tab
    {
        public static readonly string CAPTION = "capSelectTab".Translate();

        protected List<WealthItem> items;

        public abstract string Caption { get; }
        
        public abstract void Update();

        public virtual float LineHeight => Text.LineHeight * 2.25f + 2f;

        public virtual int LinesCount => items?.Count ?? 0;

        public virtual float ViewHeight => LinesCount > 0 ? (LinesCount + 1) * LineHeight : 0f;

        public static readonly GUIStyle HeaderStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.BoldAndItalic };

        public virtual void Draw(Rect viewRect)
        {
            if (items == null) return;

            Rect lineRect = new Rect(viewRect) { height = LineHeight};

            if (items.Count > 0)
                DrawHeadEntry(ref lineRect);

            items.ForEach(wi => DrawEntry(ref lineRect, wi));
        }

        private void DrawHeadEntry(ref Rect lineRect)
        {
            Rect iconRect = new Rect(lineRect)
            {
                width = lineRect.height
            };

            Rect nameRect = new Rect(lineRect)
            {
                x = lineRect.x + iconRect.width,
                width = lineRect.width - iconRect.width - lineRect.height * 3f
            };
            GUI.Label(nameRect, "EntryLabel".Translate(), HeaderStyle);

            Rect countRect = new Rect(lineRect)
            {
                x = nameRect.x + nameRect.width,
                width = lineRect.height
            };
            GUI.Label(countRect, "CountLabel".Translate(), HeaderStyle);

            Rect marketValueAllRect = new Rect(lineRect)
            {
                x = countRect.x + countRect.width,
                width = lineRect.height
            };
            GUI.Label(marketValueAllRect, "MarketValueLabel".Translate(), HeaderStyle);

            Rect infoButtonRect = new Rect(lineRect)
            {
                x = marketValueAllRect.x + marketValueAllRect.width,
                width = lineRect.height
            };
            GUI.Label(infoButtonRect, "InfoLabel".Translate(), HeaderStyle);

            lineRect.y += lineRect.height;
        }

        private void DrawEntry(ref Rect lineRect, WealthItem wi)
        {
            if (wi.thing != null && wi.MarketValueAll < 1f)
                return;

            Widgets.DrawHighlightIfMouseover(lineRect);

            Rect iconRect = new Rect(lineRect)
            {
                width = lineRect.height
            };
            if (wi.thing != null)
            {
                Widgets.ThingIcon(iconRect, wi.thing/*.def*/);
            }

            Rect nameRect = new Rect(lineRect)
            {
                x = lineRect.x + iconRect.width,
                width = lineRect.width - iconRect.width - lineRect.height * 3f
            };
            Widgets.Label(nameRect, wi.Name);

            Rect countRect = new Rect(lineRect)
            {
                x = nameRect.x + nameRect.width,
                width = lineRect.height
            };
            if (wi.thing != null)
            {
                Widgets.Label(countRect, "x" + wi.Count);
            }

            Rect marketValueAllRect = new Rect(lineRect)
            {
                x = countRect.x + countRect.width,
                width = lineRect.height
            };
            Widgets.Label(marketValueAllRect, Mathf.RoundToInt(wi.MarketValueAll).ToString());

            Rect infoButtonRect = new Rect(lineRect)
            {
                x = marketValueAllRect.x + marketValueAllRect.width,
                width = lineRect.height
            };
            if (wi.thing != null)
            {
                Widgets.InfoCardButton(infoButtonRect.x + infoButtonRect.width / 2f - 12f, infoButtonRect.y + infoButtonRect.height / 2f - 12f, wi.thing);
                TooltipHandler.TipRegion(lineRect, wi.thing.DescriptionFlavor);
            }

            lineRect.y += lineRect.height;
        }
    }

    /*
        GUI.BeginGroup(new Rect(0, viewRect.y + i * (Text.LineHeight + 2), viewRect.width, height: Text.LineHeight));
        Widgets.Label(new Rect(0, 0, 600f, height: Text.LineHeight), $"sakldllksadllalkwqooweowqoeoqwoeo: {i} {Text.LineHeight}");
        GUI.EndGroup();
        //////////////////
        Widgets.Label(new Rect(0, viewRect.y + i * (Text.LineHeight + 2), 600f, Text.LineHeight), $"sakldllksadllalkwqooweowqoeoqwoeo: {i} {Text.LineHeight}");
     */
}