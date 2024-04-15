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

    class CompTransmog : ThingComp
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
        public List<TransmogApparel> transmog = new List<TransmogApparel>();

        Pawn Pawn => parent as Pawn;

        public int ApparelCount => transmog.Count();

        public List<Apparel> Apparel => transmog.Select(transmog => transmog.GetApparel()).ToList();

        public void CopyFromApparel()
        {
            transmog = Pawn.apparel.WornApparel.Select(
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
            enabled = true;
            Update();
        }

        public void CopyFromPreset(List<TransmogApparel> preset)
        {
            transmog = new List<TransmogApparel>();
            foreach (var apparel in preset)
                if (apparel.ApparelDef?.apparel.PawnCanWear(Pawn) ?? false)
                    transmog.Add(apparel.DuplicateForPawn(Pawn));
            enabled = true;
            Update();
        }

        public void Add(TransmogApparel transmog)
        {
            this.transmog.Add(transmog);
            Update();
        }

        public void Remove(TransmogApparel transmog)
        {
            this.transmog.Remove(transmog);
            Update();
        }

        public void Update() => Pawn.apparel.Notify_ApparelChanged();

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref enabled, "transmogEnabled");
            Scribe_Collections.Look(ref transmog, "transmog");
        }
    }
}
