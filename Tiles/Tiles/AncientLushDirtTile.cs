using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class AncientLushDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientHallBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[Type][TileID.Mudstone] = true;
            Main.tileMerge[TileID.Mudstone][Type] = true;
            Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(115, 88, 69));
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
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(800) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<PaleBrittlecapTile>(), true, Main.rand.Next(5));
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<PaleBrittlecapTile>(), Main.rand.Next(5), 0, -1, -1);
            }
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(800) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<PaleBrittlecapTile2>(), true, Main.rand.Next(2));
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<PaleBrittlecapTile2>(), Main.rand.Next(2), 0, -1, -1);
            }
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(6000) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ToxicAngelTile>(), true, Main.rand.Next(5));
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<ToxicAngelTile>(), Main.rand.Next(5), 0, -1, -1);
            }
        }
    }
}