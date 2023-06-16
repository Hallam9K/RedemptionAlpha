using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class LabPlatingTileUnsafe : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe2>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe2>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile2>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile2>()][Type] = true;
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 1000;
            MineResist = 3f;
            HitSound = CustomSounds.MetalHit;
            AddMapEntry(new Color(202, 210, 210));
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<BabyHiveTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<BabyHiveTile>(), 0, 0, -1, -1);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
    public class LabPlatingTileUnsafe2 : LabPlatingTileUnsafe
    {
        public override string Texture => "Redemption/Tiles/Tiles/LabPlatingTileUnsafe";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileID.Sets.DisableSmartCursor[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe>()][Type] = true;
            MinPick = 200;
            MineResist = 5f;
        }
    }
}
