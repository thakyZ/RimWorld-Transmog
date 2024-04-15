using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Transmog
{
    class CompProperties_Transmog : CompProperties
    {
        public CompProperties_Transmog() => compClass = typeof(CompTransmog);
    }

    class CompTransmog : ThingComp, IExposable
    {
        bool enabled;
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                Update();
            }
        }
        public List<TransmogApparel> apparel = new List<TransmogApparel>();

        Pawn Pawn => parent as Pawn;

        public int ApparelCount => apparel.Count();

        public List<Apparel> Apparel => apparel.Select(transmog => transmog.GetApparel()).ToList();

        public void CopyFromApparel()
        {
            apparel = Pawn.apparel.WornApparel.Select(
                apparel =>
                    new TransmogApparel()
                    {
                        Pawn = Pawn,
                        ApparelDef = apparel.def,
                        StyleDef = apparel.StyleDef,
                        Color = apparel.DrawColor
                    }
            )
                .ToList();
            Update();
        }

        public void CopyFromPreset(CompTransmog preset)
        {
            apparel = new List<TransmogApparel>();
            foreach (var apparel in preset.apparel)
                if (apparel.ApparelDef?.apparel.PawnCanWear(Pawn) ?? false)
                    this.apparel.Add(apparel.DuplicateForPawn(Pawn));
            enabled = preset.enabled;
            Update();
        }

        public void Add(TransmogApparel transmog)
        {
            apparel.Add(transmog);
            Update();
        }

        public void Remove(TransmogApparel transmog)
        {
            apparel.Remove(transmog);
            Update();
        }

        public void Update() => Pawn.apparel.Notify_ApparelChanged();

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref enabled, "enabled");
            Scribe_Collections.Look(ref apparel, "apparel");
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref enabled, "enabled");
            Scribe_Collections.Look(ref apparel, "apparel");
        }
    }
}
