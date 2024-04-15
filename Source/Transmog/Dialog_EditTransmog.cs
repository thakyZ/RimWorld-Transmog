using System.Linq;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;

namespace Transmog
{
    class Dialog_EditTransmog : Window
    {
        TransmogApparel transmog;
        public override Vector2 InitialSize => new Vector2(360, 216);
        string hexcode;
        bool focused;

        public Dialog_EditTransmog(TransmogApparel transmog)
        {
            preventCameraMotion = false;
            draggable = true;
            doCloseX = true;
            this.transmog = transmog;
            hexcode = $"{(int)(transmog.Color.r * 255):X2}{(int)(transmog.Color.g * 255):X2}{(int)(transmog.Color.b * 255):X2}";
        }

        public override void OnAcceptKeyPressed()
        {
            base.OnAcceptKeyPressed();
            if (hexcode.Length == 6)
                transmog.Color = hexcode.toColor();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Widgets.ThingIcon(new Rect(inRect.x, inRect.y, 128, 128), transmog.GetApparel());
            if (
                transmog.ApparelDef.CanBeStyled()
                && transmog.ApparelDef.GetStyles().Count() > 1
                && Widgets.ButtonImage(new Rect(inRect.x + 112, inRect.y + 112, 16, 16), TexButton.SelectOverlappingNext)
            )
            {
                var styles = transmog.ApparelDef.GetStyles();
                transmog.StyleDef = styles[(styles.IndexOf(transmog.StyleDef) + 1) % styles.Count()];
            }
            // CanWearTogether
            if (Widgets.RadioButtonLabeled(new Rect(inRect.x + 160, inRect.y, 160, 32), "SetFavoriteColor".Translate(), transmog.FavoriteColor))
                transmog.FavoriteColor ^= true;
            if (Widgets.RadioButtonLabeled(new Rect(inRect.x + 160, inRect.y + 48, 160, 32), "SetIdeoColor".Translate(), transmog.IdeoColor))
                transmog.IdeoColor ^= true;
            if (
                Widgets.RadioButtonLabeled(new Rect(inRect.x + 160, inRect.y + 96, 160, 32), "Transmog.SetCustomColor".Translate(), !(transmog.FavoriteColor || transmog.IdeoColor))
            )
                transmog.FavoriteColor = transmog.IdeoColor = false;
            GUI.SetNextControlName("Hexcode");
            hexcode = Widgets.TextField(new Rect(inRect.x, inRect.y + 144, 128, 32), hexcode, 6, new Regex("^[0-9a-fA-F]*$"));
            if (!focused)
            {
                UI.FocusControl("Hexcode", this);
                focused = true;
            }
            if (hexcode.Length == 6)
                transmog.Color = hexcode.toColor();
            if (Widgets.ButtonText(new Rect(inRect.x + 160, inRect.y + 144, 160, 32), "Confirm".Translate()))
                Find.WindowStack.TryRemove(this);
        }
    }
}
