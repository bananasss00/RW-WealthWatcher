using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using RimWorld;
using Verse;

namespace WealthWatcher
{
    [HarmonyPatch(typeof(TargetingParameters), nameof(TargetingParameters.CanTarget))]
    public static class TargetingParameters_CanTarget_Patch
    {
        public static bool IsPatched { get; private set; } = false;

        private static readonly FieldInfo TargetingParameters_canTargetItems = typeof(TargetingParameters).GetField("canTargetItems");
        private static readonly FieldInfo TargetingParameters_mapObjectTargetsMustBeAutoAttackable = typeof(TargetingParameters).GetField("mapObjectTargetsMustBeAutoAttackable");
        private static readonly FieldInfo Thing_def = typeof(Thing).GetField("def");
        private static readonly FieldInfo ThingDef_isAutoAttackableMapObject = typeof(ThingDef).GetField("isAutoAttackableMapObject");
        private static readonly MethodInfo TargetInfo_Thing = typeof(TargetInfo).GetProperty("Thing").GetGetMethod();

        public static bool CanTargetItems(bool canTargetItems, bool isAutoAttackableMapObject, bool mapObjectTargetsMustBeAutoAttackable)
        {
            if (Settings.UnlockManualDamageItems)
                return canTargetItems;

            return canTargetItems && (!mapObjectTargetsMustBeAutoAttackable || isAutoAttackableMapObject);
        }

        /*
        148	01EE	ldarg.0
        149	01EF	ldfld	bool RimWorld.TargetingParameters::canTargetItems
        150	01F4	brfalse	163 (021E) ldc.i4.0 
        151	01F9	ldarg.0
        152	01FA	ldfld	bool RimWorld.TargetingParameters::mapObjectTargetsMustBeAutoAttackable
        153	01FF	brfalse	161 (021C) ldc.i4.1 
        154	0204	ldarga.s	targ (1)
        155	0206	call	instance class Verse.Thing Verse.TargetInfo::get_Thing()
        156	020B	ldfld	class Verse.ThingDef Verse.Thing::def
        157	0210	ldfld	bool Verse.ThingDef::isAutoAttackableMapObject
         */
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> list = new List<CodeInstruction>(instr);
            int idx = list.FindIndex(ci => ci.opcode == OpCodes.Ldfld && ci.operand == TargetingParameters_canTargetItems);
            if (idx == -1)
            {
                Log.Warning("[WealthWatcher] Could not find canTargetItems transpiler anchor - not patching.");
                return list;
            }

            if (TargetInfo_Thing == null)
            {
                Log.Error("[WealthWatcher] Can't get TargetInfo_Thing - not patching.");
                return list;
            }

            if (Thing_def == null)
            {
                Log.Error("[WealthWatcher] Can't get Thing_def - not patching.");
                return list;
            }

            if (ThingDef_isAutoAttackableMapObject == null)
            {
                Log.Error("[WealthWatcher] Can't get ThingDef_isAutoAttackableMapObject - not patching.");
                return list;
            }

            if (TargetingParameters_mapObjectTargetsMustBeAutoAttackable == null)
            {
                Log.Error("[WealthWatcher] Can't get TargetingParameters_mapObjectTargetsMustBeAutoAttackable - not patching.");
                return list;
            }

            list.InsertRange(idx + 1, new []
            {
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Call, TargetInfo_Thing),
                new CodeInstruction(OpCodes.Ldfld, Thing_def),
                new CodeInstruction(OpCodes.Ldfld, ThingDef_isAutoAttackableMapObject),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, TargetingParameters_mapObjectTargetsMustBeAutoAttackable),

                new CodeInstruction(OpCodes.Call, typeof(TargetingParameters_CanTarget_Patch).GetMethod("CanTargetItems")),
                new CodeInstruction(OpCodes.Ret)
            });
            
            list.RemoveRange(idx + 9, list.Count - (idx + 9));

            IsPatched = true;

            return list;
        }
    }
}