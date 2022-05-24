using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Plants;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class AncientHallBrickTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientDirtTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            DustType = ModContent.DustType<SlateDust>();
            MinPick = 500;
            MineResist = 10f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(81, 72, 65));
		}
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override void RandomUpdate(int i, int j)
        {
            if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(15) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<AncientStoneFoliage>(), true, Main.rand.Next(7));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<AncientStoneFoliage>(), Main.rand.Next(7), 0, -1, -1);
            }
            if (!Framing.GetTileSafely(i, j + 1).HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(25) && Main.tile[i, j + 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<AncientStoneFoliageC>(), true, Main.rand.Next(7));
                NetMessage.SendObjectPlacment(-1, i, j + 1, ModContent.TileType<AncientStoneFoliageC>(), Main.rand.Next(7), 0, -1, -1);
            }
        }
    }
}