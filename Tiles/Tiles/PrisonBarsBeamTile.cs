using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;
using Redemption.Items.Tools.PostML;

namespace Redemption.Tiles.Tiles
{
    public class PrisonBarsBeamTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = false;
            TileID.Sets.IsBeam[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneMossyTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickMossyTile>()] = true;
            DustType = ModContent.DustType<ShadestoneDust>();
            MinPick = 1000;
            MineResist = 18f;
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Prison Bars");
            AddMapEntry(new Color(83, 87, 123));
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