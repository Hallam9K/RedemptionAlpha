using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class GathicGladestoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientHallBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientDirtTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[Type][TileID.Mudstone] = true;
            Main.tileMerge[TileID.Mudstone][Type] = true;
            DustType = ModContent.DustType<SlateDust>();
            HitSound = CustomSounds.StoneHit;
            MinPick = 0;
            MineResist = 1f;
            AddMapEntry(new Color(81, 72, 65));
        }
        public override void RandomUpdate(int i, int j)
        {
            if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(15) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                int rand = Main.rand.Next(7);
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<AncientStoneFoliage>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<AncientStoneFoliage>(), rand, 0, -1, -1);
            }
            if (!Framing.GetTileSafely(i, j + 1).HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(25) && Main.tile[i, j + 1].LiquidAmount == 0)
            {
                int rand = Main.rand.Next(7);
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<AncientStoneFoliageC>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<AncientStoneFoliageC>(), rand, 0, -1, -1);
            }
        }
    }
}