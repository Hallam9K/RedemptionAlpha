using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals.Players;
using Redemption.Items.Accessories.HM;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedHardenedSandTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSandstoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSandTile>()] = true;
            Main.tileMerge[Type][TileID.HardenedSand] = true;
            Main.tileMerge[TileID.HardenedSand][Type] = true;
            Main.tileMerge[Type][TileID.CorruptHardenedSand] = true;
            Main.tileMerge[TileID.CorruptHardenedSand][Type] = true;
            Main.tileMerge[Type][TileID.CrimsonHardenedSand] = true;
            Main.tileMerge[TileID.CrimsonHardenedSand][Type] = true;
            Main.tileMerge[Type][TileID.HallowHardenedSand] = true;
            Main.tileMerge[TileID.HallowHardenedSand][Type] = true;
            TileID.Sets.Conversion.HardenedSand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.isDesertBiomeSand[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(149, 133, 121));
            MineResist = 1.5f;
            DustType = DustID.Ash;
        }
        public override void FloorVisuals(Player player)
        {
            if (player.velocity.X != 0f && Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(player.Bottom, 0, 0, DustType, 0f, -Main.rand.NextFloat(2f));
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (!fail && dist <= 4)
                player.RedemptionRad().Irradiate(.05f, 0, 2, 1, 6);
        }
    }
}