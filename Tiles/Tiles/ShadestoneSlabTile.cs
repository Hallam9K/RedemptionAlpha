using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Redemption.Dusts.Tiles;
using Redemption.Items.Tools.PostML;

namespace Redemption.Tiles.Tiles
{
    public class ShadestoneSlabTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            DustType = ModContent.DustType<ShadestoneDust>();
            MinPick = 1000;
            MineResist = 18f;
            HitSound = CustomSounds.BrickHit;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadestone Slab");
            AddMapEntry(new Color(59, 61, 87));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<NanoAxe2>())
                return true;
            return false;
        }
    }
}