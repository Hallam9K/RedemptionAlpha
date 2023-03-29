using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class SlayerShipPanelTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = ModContent.DustType<LabPlatingDust>();
            ItemDrop = ModContent.ItemType<SlayerShipPanel>();
            MinPick = 500;
            MineResist = 7f;
            HitSound = CustomSounds.MetalHit;
            AddMapEntry(new Color(72, 70, 79));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;
    }
    public class SlayerShipPanelTile2 : SlayerShipPanelTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/SlayerShipPanelTile";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileMerge[Type][ModContent.TileType<SlayerShipPanelTile>()] = true;
            Main.tileMerge[ModContent.TileType<SlayerShipPanelTile>()][Type] = true;
            ItemDrop = ModContent.ItemType<SlayerShipPanel2>();
            MinPick = 100;
            MineResist = 3f;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => true;
    }
}