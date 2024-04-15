using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Transmog
{
    class Dialog_AddTransmog : Window
    {
        static IEnumerable<ThingDef> apparel = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.IsApparel);
        IEnumerable<ThingDef> apparelForPawn;
        IEnumerable<ThingDef> filtered => apparelForPawn.Where(apparel => apparel.LabelCap.ToString().IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0);
        Pawn pawn;
        public override Vector2 InitialSize => new Vector2(360, 720);
        Vector2 scrollPosition = Vector2.zero;
        string filter = "";
        bool focused;

        public Dialog_AddTransmog(Pawn pawn)
        {
            preventCameraMotion = false;
            draggable = true;
            doCloseX = true;
            this.pawn = pawn;
            apparelForPawn = apparel.Where(apparel => apparel.apparel.PawnCanWear(pawn));
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            var height = 32;
            var gap = 8;
            var curY = inRect.y;
            var scrollviewHeight = filtered.Count() * height;
            var selected = false;
            GUI.SetNextControlName("Filter");
            filter = Widgets.TextField(new Rect(inRect.x, inRect.yMax - height, inRect.width, height), filter);
            if (!focused)
            {
                UI.FocusControl("Filter", this);
                focused = true;
            }
            Widgets.BeginScrollView(
                new Rect(inRect.x, inRect.y, inRect.width, inRect.height - height - gap),
                ref scrollPosition,
                new Rect(inRect.x, inRect.y, inRect.width - 20f, scrollviewHeight)
            );
            foreach (var apparel in filtered)
            {
                var rowRect = new Rect(inRect.x, curY, inRect.width, height);
                if (Mouse.IsOver(rowRect))
                    GUI.DrawTexture(rowRect, TexUI.HighlightTex);
                Widgets.ThingIcon(new Rect(rowRect.x, rowRect.y, height, height), apparel);
                Widgets.Label(new Rect(rowRect.x + height + gap, rowRect.y + 5f, rowRect.width, height - 10f), apparel.LabelCap);
                if (Widgets.ButtonInvisible(rowRect))
                {
                    Select(apparel);
                    selected = true;
                }
                curY += height;
            }
            Widgets.EndScrollView();
            if (selected)
                Find.WindowStack.TryRemove(this);
        }

        public override void OnAcceptKeyPressed()
        {
            if (!filtered.EnumerableNullOrEmpty())
                Select(filtered.First());
            Find.WindowStack.TryRemove(this);
        }

        void Select(ThingDef apparel)
        {
            var transmog = new TransmogApparel
            {
                Pawn = pawn,
                ApparelDef = apparel,
                StyleDef = null,
                Color = Color.white
            };
            pawn.Preset().Add(transmog);
            Find.WindowStack.Add(new Dialog_EditTransmog(transmog));
        }
    }
}
