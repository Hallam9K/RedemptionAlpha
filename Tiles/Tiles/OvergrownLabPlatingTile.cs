using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Plants;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Tiles.Tiles
{
    public class OvergrownLabPlatingTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe>()][Type] = true;
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
        public override void FloorVisuals(Player player)
        {
            if (player.velocity.X != 0f && Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(player.Bottom, 0, 0, DustID.GrassBlades, 0f, -Main.rand.NextFloat(2f));
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.HasTile && Main.rand.NextBool(15) && tileAbove.LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<LabShrub>(), true, Main.rand.Next(7));
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<LabShrub>(), Main.rand.Next(7), 0, -1, -1);
            }
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<BabyHiveTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<BabyHiveTile>(), 0, 0, -1, -1);
            }
            if (Main.rand.NextBool(200))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<LabPlatingTileUnsafe>(), Type, false);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
    public class OvergrownLabPlatingTile2 : ModTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/OvergrownLabPlatingTile";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileID.Sets.DisableSmartCursor[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile>()][Type] = true;
            MinPick = 200;
            MineResist = 5f;
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.HasTile && Main.rand.NextBool(15) && tileAbove.LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<LabShrub>(), true, Main.rand.Next(7));
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<LabShrub>(), Main.rand.Next(7), 0, -1, -1);
            }
            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<BabyHiveTile>(), true);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<BabyHiveTile>(), 0, 0, -1, -1);
            }
            if (Main.rand.NextBool(200))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<LabPlatingTileUnsafe2>(), Type, false);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}
