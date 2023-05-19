using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Ores;
using Redemption.Tiles.Plants;
using Redemption.Items.Tools.PostML;

namespace Redemption.Tiles.Tiles
{
    public class AncientSlateTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            DustType = ModContent.DustType<SlateDust>();
            MinPick = 350;
            MineResist = 18f;
            HitSound = CustomSounds.StoneHit;
            AddMapEntry(new Color(180, 170, 180));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(800) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<PaleBrittlecapTile>(), true, Main.rand.Next(5));
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<PaleBrittlecapTile>(), Main.rand.Next(5), 0, -1, -1);
            }
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(1200) && Main.tile[i, j - 1].LiquidAmount == 0)
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