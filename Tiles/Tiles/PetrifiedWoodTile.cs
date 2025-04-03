using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class PetrifiedWoodTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][TileID.SpookyWood] = true;
            Main.tileMerge[TileID.SpookyWood][Type] = true;
            DustType = DustID.Ash;
            MinPick = 0;
            MineResist = 1.5f;
            AddMapEntry(new Color(111, 100, 93));
        }
        public override void Convert(int i, int j, int conversionType)
        {
            if (conversionType == GetInstance<WastelandSolutionConversion>().Type)
                return;
            WorldGen.ConvertTile(i, j, TileID.WoodBlock);
        }
    }
}