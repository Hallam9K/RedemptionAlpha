using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class AncientHallBrickTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientDirtTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            DustType = ModContent.DustType<SlateDust>();
            MinPick = 5000;
            MineResist = 10f;
            HitSound = CustomSounds.StoneHit;
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
    public class AncientHallBrickTileSafe : AncientHallBrickTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/AncientHallBrickTile";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileID.Sets.DisableSmartCursor[Type] = false;
            MinPick = 10;
            MineResist = 1.5f;
        }
    }
}