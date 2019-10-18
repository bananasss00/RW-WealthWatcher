﻿using System.Linq;
using Verse;

namespace WealthWatcher
{
    public class WeatlthWatcherComponent : GameComponent
    {
        public WeatlthWatcherComponent() { }
        public WeatlthWatcherComponent(Game game) { this.game = game; }

        public override void GameComponentOnGUI()
        {
            if (WealthWatcherDefOf.WealthWatcher_Open != null && WealthWatcherDefOf.WealthWatcher_Open.IsDownEvent)
            {
                if (Find.WindowStack.Windows.Count(window => window is WeatlthWatcherWindow) <= 0)
                {
                    Find.WindowStack.Add(new WeatlthWatcherWindow());
                }
            }
        }

        public Game game;
    }
}
