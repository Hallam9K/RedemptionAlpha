using Microsoft.Xna.Framework.Graphics;
using Redemption.Tiles.Tiles;
using System;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Plants
{
    public class PurityWastelandVine : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.TileCutIgnore.Regrowth[Type] = true;
            TileID.Sets.IsVine[Type] = true;
            TileID.Sets.ReplaceTileBreakDown[Type] = true;
            TileID.Sets.VineThreads[Type] = true;

            AddMapEntry(new Color(91, 85, 73));

            HitSound = SoundID.Grass;
            DustType = DustID.Ash;

            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);
            return false;
        }
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -2;
        }
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 0)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }
    public class PurityWastelandVineGlobalTile : GlobalTile
    {
        private int Vine;
        private int Grass;

        public override void SetStaticDefaults()
        {
            Vine = TileType<PurityWastelandVine>();
            Grass = TileType<IrradiatedGrassTile>();
        }

        private bool ValidTile(int tile) => tile == Grass;
        public override void RandomUpdate(int i, int j, int type)
        {
            if (j >= Main.worldSurface - 1)
                return;

            Tile tile = Main.tile[i, j];
            if (!tile.HasUnactuatedTile)
                return;

            if ((tile.TileType == Vine || ValidTile(tile.TileType)) && WorldGen.GrowMoreVines(i, j))
            {
                int growChance = 70;
                if (tile.TileType == Vine)
                    growChance = 7;

                int below = j + 1;
                Tile tileBelow = Main.tile[i, below];
                if (WorldGen.genRand.NextBool(growChance) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
                {
                    bool vineIsHangingOffValidTile = false;
                    for (int above = j; above > j - 10; above--)
                    {
                        Tile tileAbove = Main.tile[i, above];
                        if (tileAbove.BottomSlope)
                        {
                            return;
                        }

                        if (tileAbove.HasTile && ValidTile(tileAbove.TileType) && !tileAbove.BottomSlope)
                        {
                            vineIsHangingOffValidTile = true;
                            break;
                        }
                    }

                    if (vineIsHangingOffValidTile)
                    {
                        tileBelow.TileType = (ushort)Vine;
                        tileBelow.HasTile = true;
                        tileBelow.CopyPaintAndCoating(tile);
                        WorldGen.SquareTileFrame(i, below);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, i, below);
                    }
                }
            }
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (!TileID.Sets.IsVine[type])
                return true;

            Tile tile = Main.tile[i, j];
            Tile tileAbove = Main.tile[i, j - 1];

            int aboveTileType = tileAbove.HasUnactuatedTile && !tileAbove.BottomSlope ? tileAbove.TileType : -1;

            if (type != aboveTileType)
            {
                if ((ValidTile(aboveTileType) || aboveTileType == Vine) && type != Vine)
                {
                    tile.TileType = (ushort)Vine;
                    WorldGen.SquareTileFrame(i, j);
                    return true;
                }

                if (type == Vine && !ValidTile(aboveTileType))
                {
                    if (aboveTileType == -1)
                        WorldGen.KillTile(i, j);
                    else
                        tile.TileType = TileID.Vines;
                }
            }

            return true;
        }
    }
}