using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public class WealthItem
    {
        public string Name;
        public int Count;
        public float MarketValue;
        public float MarketValueAll;
        public Thing thing;
    }

    public abstract class WealthTab
    {
        public static readonly GUIStyle HeaderStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.BoldAndItalic, alignment = TextAnchor.MiddleLeft};
        protected List<WealthItem> items;

        public virtual float LineHeight => Text.LineHeight * 2.25f + 2f;
        public virtual int LinesCount => items?.Count ?? 0;
        public virtual float ViewHeight => LinesCount > 0 ? (LinesCount + 1) * LineHeight : 0f;

        public virtual void Draw(Rect viewRect)
        {
            if (items == null) return;

            float y = viewRect.y;
            DrawHeadLine(viewRect.width, LineHeight, ref y);
            items.ForEach(wi => DrawLine(wi, viewRect.width, LineHeight, ref y));
        }

        #region Draw helpers

        private void DrawElementOfLineHead(ref Rect elementRect, float width, string label)
        {
            elementRect.width = width;

            if (label != null)
            {
                GUI.Label(elementRect, label, HeaderStyle);
            }

            elementRect.x = elementRect.xMax;
        }

        private void DrawElementOfLine<SecondParamType>(ref Rect elementRect, float width, Action<Rect, SecondParamType> widgetFunc, SecondParamType secondParameter)
            where SecondParamType : class
        {
            elementRect.width = width;

            if (secondParameter != null)
            {
                widgetFunc(elementRect, secondParameter);
            }

            elementRect.x = elementRect.xMax;
        }

        #endregion
        
        private void DrawHeadLine(float width, float height, ref float y)
        {
            GUI.BeginGroup(new Rect(x: 0f, y: y, width: width, height: height));

            Rect
                elementRect = new Rect(x: 0f, y: 0f, width: width, height: height);

            float
                defaultColumnWidth = height,
                nameColumnWidth = width - height * 4; // 4 = icon+count+value+info width

            DrawElementOfLineHead(ref elementRect, defaultColumnWidth, null);
            DrawElementOfLineHead(ref elementRect, nameColumnWidth, "EntryLabel".Translate());
            DrawElementOfLineHead(ref elementRect, defaultColumnWidth, "CountLabel".Translate());
            DrawElementOfLineHead(ref elementRect, defaultColumnWidth, "MarketValueLabel".Translate());
            DrawElementOfLineHead(ref elementRect, defaultColumnWidth, "InfoLabel".Translate());
            
            GUI.EndGroup();
            y += height;
        }

        private void DrawLine(WealthItem wi, float width, float height, ref float y)
        {
            if (wi.thing != null && wi.MarketValueAll < 1f) return;

            GUI.BeginGroup(new Rect(x: 0f, y: y, width: width, height: height));

            float
                defaultColumnWidth = height,
                nameColumnWidth = width - height * 4; // 4 = icon+count+value+info width

            Rect
                elementRect = new Rect(x: 0f, y: 0f, width: width, height: height),
                lineRect = elementRect;
            
            Widgets.DrawHighlightIfMouseover(lineRect);
            // icon, lambda because Widgets.ThingIcon has 3 arguments with default float value 1
            DrawElementOfLine(ref elementRect, defaultColumnWidth, (rect, thing) => Widgets.ThingIcon(rect, thing), wi.thing);
            // label
            DrawElementOfLine(ref elementRect, nameColumnWidth, Widgets.Label, wi.Name);
            // count
            DrawElementOfLine(ref elementRect, defaultColumnWidth, Widgets.Label, $"x{wi.Count}");
            // marketValue
            DrawElementOfLine(ref elementRect, defaultColumnWidth, Widgets.Label, Mathf.RoundToInt(wi.MarketValueAll).ToString());
            // infoCardButton
            Widgets.InfoCardButton(elementRect.x, elementRect.y, wi.thing);
            TooltipHandler.TipRegion(lineRect, wi.thing.DescriptionFlavor);

            GUI.EndGroup();
            y += height;
        }
    }
}