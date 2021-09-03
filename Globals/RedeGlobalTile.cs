using Redemption.NPCs.Critters;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeGlobalTile : GlobalTile
    {
        public override bool Drop(int i, int j, int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !WorldGen.noTileActions && !WorldGen.gen)
            {
                if (type == TileID.Trees && Main.tile[i, j + 1].type == TileID.Grass)
                {
                    if (Main.rand.NextBool(6))
                        Projectile.NewProjectile(new ProjectileSource_TileBreak(i, j), i * 16, (j - 10) * 16, -4 + Main.rand.Next(0, 7), -3 + Main.rand.Next(-3, 0), ModContent.ProjectileType<TreeBugFall>(), 0, 0);
                }
                if (type == TileID.PalmTree && Main.tile[i, j + 1].type == TileID.Sand)
                {
                    if (Main.rand.NextBool(6))
                        Projectile.NewProjectile(new ProjectileSource_TileBreak(i, j), i * 16, (j - 10) * 16, -4 + Main.rand.Next(0, 7), -3 + Main.rand.Next(-3, 0), ModContent.ProjectileType<CoastScarabFall>(), 0, 0);
                }
            }
            return base.Drop(i, j, type);
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            if (type == TileID.Grass && !Main.dayTime && RedeBossDowned.downedThorn)
            {
                if (!Framing.GetTileSafely(i, j - 1).IsActive && Main.tile[i, j].IsActive && Main.tile[i, j - 1].LiquidAmount == 0 && Main.tile[i, j - 1].wall == 0)
                {
                    if (Main.rand.NextBool(300))
                    {
                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<NightshadeTile>(), true);
                    }
                }
            }
            if (type == TileID.Grass && Main.dayTime && RedeBossDowned.downedThorn)
            {
                if (!Framing.GetTileSafely(i, j - 1).IsActive && !Framing.GetTileSafely(i, j - 2).IsActive && !Framing.GetTileSafely(i + 1, j - 1).IsActive && !Framing.GetTileSafely(i + 1, j - 2).IsActive && Main.tile[i, j].IsActive && Main.tile[i + 1, j].IsActive && Main.tile[i, j - 1].LiquidAmount == 0 && Main.tile[i, j - 1].wall == 0)
                {
                    if (Main.rand.NextBool(6000))
                    {
                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<AnglonicMysticBlossomTile>(), true);
                    }
                }
            }
        }
    }
}