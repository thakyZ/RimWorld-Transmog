using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using static HarmonyLib.AccessTools;

namespace Transmog
{
    [StaticConstructorOnStartup]
    static class Startup
    {
        static Startup()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.race?.Humanlike ?? false))
            {
                def.comps.Add(new CompProperties_Transmog());
                def.inspectorTabs.Add(typeof(ITab_Pawn_Transmog));
                def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Transmog)));
            }
            var harmony = new Harmony("localghost.transmog");
            var renderTranspiler = new HarmonyMethod(Method("Transmog.HarmonyPatches:RenderTranspiler"));
            foreach (
                var typeColonName in new List<string>
                {
                    "Verse.PawnRenderUtility:DrawEquipmentAndApparelExtras",
                    "Verse.PawnRenderTree:AdjustParms",
                    "Verse.PawnRenderTree:SetupApparelNodes"
                }
            )
                harmony.Patch(Method(typeColonName), transpiler: renderTranspiler);
            harmony.Patch(PropertyGetter("RimWorld.CompShield:ShouldDisplay"), prefix: new HarmonyMethod(Method("Transmog.HarmonyPatches:Prefix")));
            PresetManager.LoadPresets();
        }
    }

    static class HarmonyPatches
    {
        static IEnumerable<CodeInstruction> RenderTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); ++i)
            {
                var instruction = instructions.ElementAt(i);
                if (
                    i + 2 < instructions.Count()
                    && instructions.ElementAt(i + 2).opcode == OpCodes.Callvirt
                    && (MethodInfo)instructions.ElementAt(i + 2).operand == PropertyGetter("RimWorld.Pawn_ApparelTracker:WornApparel")
                )
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Callvirt, Method("Transmog.Utility:TransmogApparel"));
                    i += 2;
                }
                else
                    yield return instruction;
            }
        }

        static bool Prefix(ref CompShield __instance, ref bool __result) => __instance.parent is Apparel apparel && apparel.Wearer == null ? (__result = false) : true;
    }
}
