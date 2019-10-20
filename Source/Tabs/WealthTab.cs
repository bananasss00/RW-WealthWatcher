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

        public virtual float DefaultColumnWidth => Text.LineHeight * 2.5f;
        public virtual float LineHeight => Text.LineHeight * 1.2f;
        public virtual int LinesCount => items?.Count ?? 0;
        public virtual float ViewHeight => LinesCount > 0 ? (LinesCount + 1) * LineHeight : 0f;
        public virtual void Close() => items?.Clear();

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

            float nameColumnWidth = width - height - DefaultColumnWidth * 3; // height(icon)
            Rect elementRect = new Rect(x: 0f, y: 0f, width: width, height: height);
            
            DrawElementOfLineHead(ref elementRect, height, null);
            DrawElementOfLineHead(ref elementRect, nameColumnWidth, "EntryLabel".Translate());
            DrawElementOfLineHead(ref elementRect, DefaultColumnWidth, "CountLabel".Translate());
            DrawElementOfLineHead(ref elementRect, DefaultColumnWidth, "MarketValueLabel".Translate());
            DrawElementOfLineHead(ref elementRect, DefaultColumnWidth, "InfoLabel".Translate());
            
            GUI.EndGroup();
            y += height;
        }

        private void DrawLine(WealthItem wi, float width, float height, ref float y)
        {
            if (wi.thing != null && wi.MarketValueAll < 1f) return;

            GUI.BeginGroup(new Rect(x: 0f, y: y, width: width, height: height));

            float nameColumnWidth = width - height - DefaultColumnWidth * 3; // height(icon)
            Rect elementRect = new Rect(x: 0f, y: 0f, width: width, height: height);
            
            Widgets.DrawHighlightIfMouseover(elementRect);
            TooltipHandler.TipRegion(elementRect, wi.thing.DescriptionFlavor);

            // icon, lambda because Widgets.ThingIcon has 3 arguments with default float value 1
            DrawElementOfLine(ref elementRect, height, (rect, thing) => Widgets.ThingIcon(rect, thing), wi.thing);
            // label
            DrawElementOfLine(ref elementRect, nameColumnWidth, Widgets.Label, wi.Name);
            // count
            DrawElementOfLine(ref elementRect, DefaultColumnWidth, Widgets.Label, $"x{wi.Count}");
            // marketValue
            DrawElementOfLine(ref elementRect, DefaultColumnWidth, Widgets.Label, Mathf.RoundToInt(wi.MarketValueAll).ToString());
            // infoCardButton
            Widgets.InfoCardButton(elementRect.x, elementRect.y, wi.thing);
            
            GUI.EndGroup();
            y += height;
        }
    }
}