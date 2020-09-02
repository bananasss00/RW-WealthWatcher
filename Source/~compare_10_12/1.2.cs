public static float DefaultThreatPointsNow(IIncidentTarget target)
{
	float playerWealthForStoryteller = target.PlayerWealthForStoryteller;
	float num = StorytellerUtility.PointsPerWealthCurve.Evaluate(playerWealthForStoryteller);
	float num2 = 0f;
	foreach (Pawn pawn in target.PlayerPawnsForStoryteller)
	{
		if (!pawn.IsQuestLodger())
		{
			float num3 = 0f;
			if (pawn.IsFreeColonist)
			{
				num3 = StorytellerUtility.PointsPerColonistByWealthCurve.Evaluate(playerWealthForStoryteller);
			}
			else if (pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer && !pawn.Downed && pawn.training.CanAssignToTrain(TrainableDefOf.Release).Accepted)
			{
				num3 = 0.08f * pawn.kindDef.combatPower;
				if (target is Caravan)
				{
					num3 *= 0.7f;
				}
			}
			if (num3 > 0f)
			{
				if (pawn.ParentHolder != null && pawn.ParentHolder is Building_CryptosleepCasket)
				{
					num3 *= 0.3f;
				}
				num3 = Mathf.Lerp(num3, num3 * pawn.health.summaryHealth.SummaryHealthPercent, 0.65f);
				num2 += num3;
			}
		}
	}
	float num4 = (num + num2) * target.IncidentPointsRandomFactorRange.RandomInRange;
	float totalThreatPointsFactor = Find.StoryWatcher.watcherAdaptation.TotalThreatPointsFactor;
	float num5 = Mathf.Lerp(1f, totalThreatPointsFactor, Find.Storyteller.difficultyValues.adaptationEffectFactor);
	return Mathf.Clamp(num4 * num5 * Find.Storyteller.difficultyValues.threatScale * Find.Storyteller.def.pointsFactorFromDaysPassed.Evaluate((float)GenDate.DaysPassed), 35f, 10000f);
}