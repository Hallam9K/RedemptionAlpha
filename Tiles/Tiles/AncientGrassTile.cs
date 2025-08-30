using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class AncientGrassTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileSolid[Type] = true;

            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<AncientDirtTile>();
            TileID.Sets.ForcedDirtMerging[Type] = true;
            TileID.Sets.Conversion.MergesWithDirtInASpecialWay[Type] = true;
            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = false;
            TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
            AddMapEntry(new Color(69, 119, 38));
            MinPick = 10;
            MineResist = 0.1f;
            DustType = DustID.GrassBlades;
            RegisterItemDrop(ModContent.ItemType<AncientDirt>());
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
        public override bool CanExplode(int i, int j)
        {
            WorldGen.KillTile(i, j, false, false, true);
            return true;
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                fail = true;
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<AncientDirtTile>();
                WorldGen.SquareTileFrame(i, j);

                for (int k = 0; k < 3; k++)
                    Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.GrassBlades);
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileAbove2 = Framing.GetTileSafely(i + 1, j - 1);
            Tile tile = Framing.GetTileSafely(i, j);

            if (Main.rand.NextBool(15) && !tileAbove.HasTile && tileAbove.LiquidAmount == 0)
            {
                int rand = Main.rand.Next(11);
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<AncientShrub>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<AncientShrub>(), rand, 0, -1, -1);
                tileAbove.TileColor = tile.TileColor;
            }
            if (Main.rand.NextBool(15) && !tileAbove.HasTile && !Framing.GetTileSafely(i, j - 2).HasTile && tileAbove.LiquidAmount == 0)
            {
                int rand = Main.rand.Next(10);
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<AncientShrub2>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<AncientShrub2>(), rand, 0, -1, -1);
                tileAbove.TileColor = tile.TileColor;
                Framing.GetTileSafely(i, j - 2).TileColor = tile.TileColor;
            }
            if (Main.rand.NextBool(30) && Framing.GetTileSafely(i + 1, j).TileType == Type && !tileAbove.HasTile && !tileAbove2.HasTile && !Framing.GetTileSafely(i, j - 2).HasTile && !Framing.GetTileSafely(i + 1, j - 2).HasTile && tileAbove.LiquidAmount == 0)
            {
                int rand = Main.rand.Next(6);
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<AncientShrub3>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<AncientShrub3>(), rand, 0, -1, -1);
                if (BaseTile.IsType(i, j - 2, 2, 2, ModContent.TileType<AncientShrub3>()))
                {
                    tileAbove.TileColor = tile.TileColor;
                    Framing.GetTileSafely(i, j - 2).TileColor = tile.TileColor;
                    tileAbove2.TileColor = tile.TileColor;
                    Framing.GetTileSafely(i + 1, j - 2).TileColor = tile.TileColor;
                }
            }
            if (Main.rand.NextBool(4))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<AncientDirtTile>(), Type, false, tile.BlockColorAndCoating());
        }
    }
}