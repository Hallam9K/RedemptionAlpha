using Redemption.BaseExtension;
using Redemption.Items.Placeable.Tiles;
using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedLivingWoodTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedGrassTile>()] = true;
            Main.tileMerge[TileType<IrradiatedGrassTile>()][Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedCorruptGrassTile>()] = true;
            Main.tileMerge[TileType<IrradiatedCorruptGrassTile>()][Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedCrimsonGrassTile>()] = true;
            Main.tileMerge[TileType<IrradiatedCrimsonGrassTile>()][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            AddMapEntry(new Color(111, 100, 93));
            DustType = DustID.Ash;
            MineResist = 2.5f;
            RegisterItemDrop(ItemType<PetrifiedWood>(), 0);
        }
        public override void Convert(int i, int j, int conversionType)
        {
            if (conversionType == GetInstance<WastelandSolutionConversion>().Type)
                return;
            WorldGen.ConvertTile(i, j, TileID.LivingWood);
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