using System.Collections.Generic;
using UnityEngine;
using Verse;
using WealthWatcher.Tabs.Sorting;

namespace WealthWatcher.Tabs
{
    public interface ITab
    {
        string Caption { get; }
        float ViewHeight { get; }
        void Update();
        void Sort(TabComparer sort1, TabComparer sort2);
        void Draw(Rect outRect, Rect viewRect, Vector2 scrollPosition);
        void Close();
    }
}