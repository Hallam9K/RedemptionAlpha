using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Dusts.Tiles;
using Redemption.Items.Tools.PostML;
using Redemption.Globals;
using Terraria.ID;

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
            TileID.Sets.DisableSmartCursor[Type] = true;
            MinPick = 1000;
            MineResist = 7f;
            HitSound = CustomSounds.MetalHit;
            AddMapEntry(new Color(72, 70, 79));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<NanoAxe2>())
                return true;
            return WorldGen.gen || RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus;
        }
        public override bool Slope(int i, int j) => true;
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
            MinPick = 100;
            MineResist = 3f;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => true;
    }
}