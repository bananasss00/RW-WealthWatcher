using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WealthWatcher
{
    public class RadePointsSummary
    {
        public float wealthItems, wealthBuildings, wealthPawns;
        public float pointsWealth;
        public float pointsPerColonist;
        public float pointsColonist;
        public float pointsAnimal;
        public float adaptationFactor;
        public float difficultyFactor;
        public float daysPassedFactor;
        public float points;
        public string colonistsSummary, animalsSummary;

        public static string GetSummary(List<KeyValuePair<float, string>> list, int maxCount)
        {
            var sb = new StringBuilder();
            
            foreach (var x in list.OrderByDescending(x => x.Key))
            {
                sb.AppendLine($"{x.Value}: {x.Key:F0}pts");

                if (--maxCount == 0) break;
            }

            return sb.ToString();
        }

        public static RadePointsSummary Get(/*IIncidentTarget*/Map m)
        {
            RadePointsSummary rps = new RadePointsSummary();

            //target.PlayerWealthForStoryteller
            {
                rps.wealthItems = m.wealthWatcher.WealthItems;
                rps.wealthBuildings = m.wealthWatcher.WealthBuildings * 0.5f;
                rps.wealthPawns = m.wealthWatcher.WealthPawns;
            }

            float playerWealthForStoryteller = rps.wealthItems + rps.wealthBuildings + rps.wealthPawns /* = m.PlayerWealthForStoryteller */;
            rps.pointsWealth = PointsPerWealthCurve.Evaluate(playerWealthForStoryteller);
            rps.pointsPerColonist = PointsPerColonistByWealthCurve.Evaluate(playerWealthForStoryteller);

            Assert("[WealthWatcher] PlayerWealthForStoryteller changed({0} != {1})", playerWealthForStoryteller, m.PlayerWealthForStoryteller);

            List<KeyValuePair<float, string>> colonistsPoints = new List<KeyValuePair<float, string>>();
            List<KeyValuePair<float, string>> animalsPoints = new List<KeyValuePair<float, string>>();
            foreach (Pawn pawn in m.PlayerPawnsForStoryteller)
            {
                bool isAnimal = false;

                float pawnPoints = 0f;
                if (pawn.IsFreeColonist)
                {
                    pawnPoints = rps.pointsPerColonist;
                }
                else if (pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer && !pawn.Downed && pawn.training.CanAssignToTrain(TrainableDefOf.Release).Accepted)
                {
                    pawnPoints = 0.08f * pawn.kindDef.combatPower;

                    // We calc only for CurrentMap
                    //if (target is Caravan) num3 *= 0.7f;
                    
                    isAnimal = true;
                }

                if (pawnPoints > 0f)
                {
                    if (pawn.ParentHolder != null && pawn.ParentHolder is Building_CryptosleepCasket)
                        pawnPoints *= 0.3f;
                    pawnPoints = Mathf.Lerp(pawnPoints, pawnPoints * pawn.health.summaryHealth.SummaryHealthPercent, 0.65f);

                    var kv = new KeyValuePair<float, string>(pawnPoints, pawn.Name.ToStringShort);
                    if (!isAnimal)
                    {
                        rps.pointsColonist += pawnPoints;
                        colonistsPoints.Add(kv);
                    }
                    else
                    {
                        rps.pointsAnimal += pawnPoints;
                        animalsPoints.Add(kv);
                    }
                }
            }

            rps.colonistsSummary = GetSummary(colonistsPoints, 50);
            rps.animalsSummary = GetSummary(animalsPoints, 50);

            rps.points = rps.pointsWealth + rps.pointsColonist + rps.pointsAnimal;
            
            // this factor on map always = 1; caravan = 0.7f, 0.9f
            // ws.points *= m.IncidentPointsRandomFactorRange.RandomInRange;

            rps.adaptationFactor = Mathf.Lerp(1f, Find.StoryWatcher.watcherAdaptation.TotalThreatPointsFactor, Find.Storyteller.difficulty.adaptationEffectFactor);
            rps.difficultyFactor = Find.Storyteller.difficulty.threatScale;
            rps.daysPassedFactor = Find.Storyteller.def.pointsFactorFromDaysPassed.Evaluate(GenDate.DaysPassed); // 0-10(days) = 0.7; 11-40(days) = 0.7...1.0; 40+ = 1.0
            rps.points *= rps.adaptationFactor;
            rps.points *= rps.difficultyFactor;
            rps.points *= rps.daysPassedFactor;
            rps.points = Mathf.Clamp(rps.points, 35f, 20000f);

            Assert("[WealthWatcher] Raid points math changed({0} != {1})", rps.points, StorytellerUtility.DefaultThreatPointsNow(m));

            return rps;
        }

        private static void Assert(string error, float v1, float v2)
        {
            if (Math.Abs(v1 - v2) > 0.01f)
            {
                Log.Error(String.Format(error, v1, v2));
            }
        }

        public static readonly SimpleCurve PointsPerWealthCurve = (SimpleCurve)AccessTools.Field(typeof(StorytellerUtility), "PointsPerWealthCurve").GetValue(null);
        public static readonly SimpleCurve PointsPerColonistByWealthCurve = (SimpleCurve)AccessTools.Field(typeof(StorytellerUtility), "PointsPerColonistByWealthCurve").GetValue(null);
    }
}