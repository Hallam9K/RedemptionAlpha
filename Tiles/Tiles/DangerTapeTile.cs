using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class DangerTapeTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe2>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe2>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile2>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile2>()][Type] = true;
            DustType = DustID.Electric;
            MinPick = 1000;
            MineResist = 3f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(49, 49, 52));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
    public class DangerTape2Tile : ModTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/DangerTapeTile";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe2>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe2>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile2>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile2>()][Type] = true;
            DustType = DustID.Electric;
            MinPick = 50;
            MineResist = 1f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(49, 49, 52));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
