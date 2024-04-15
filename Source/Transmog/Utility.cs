using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Transmog
{
    static class Utility
    {
        public static CompTransmog Preset(this Pawn pawn) => pawn.GetComp<CompTransmog>();

        public static List<Apparel> TransmogApparel(Pawn pawn) => pawn.Preset()?.Enabled ?? false ? pawn.Preset().Apparel : pawn.apparel.WornApparel;

        static Dictionary<ThingDef, List<ThingStyleDef>> styles = new Dictionary<ThingDef, List<ThingStyleDef>>();

        public static List<ThingStyleDef> GetStyles(this ThingDef def)
        {
            if (!styles.ContainsKey(def))
            {
                // TryPlaceNearThingWithStyle
                styles[def] = new List<ThingStyleDef>() { null };
                if (!def.randomStyle.NullOrEmpty())
                    foreach (var styleChance in def.randomStyle)
                        if (styleChance.StyleDef.graphicData != null)
                            styles[def].Add(styleChance.StyleDef);
                foreach (var styleCat in DefDatabase<StyleCategoryDef>.AllDefsListForReading)
                    styles[def].Add(styleCat.GetStyleForThingDef(def));
                styles[def].RemoveDuplicates();
            }
            return styles[def];
        }

        public static Color toColor(this string hexcode) =>
            new Color(
                Convert.ToInt32(hexcode.Substring(0, 2), 16) * 1.0f / 255,
                Convert.ToInt32(hexcode.Substring(2, 2), 16) * 1.0f / 255,
                Convert.ToInt32(hexcode.Substring(4, 2), 16) * 1.0f / 255
            );
    }
}
