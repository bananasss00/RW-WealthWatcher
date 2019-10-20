using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WealthWatcher.Tabs
{
    public interface ITab
    {
        string Caption { get; }
        float ViewHeight { get; }
        void Update();
        void Draw(Rect viewRect);
        void Close();
    }
}