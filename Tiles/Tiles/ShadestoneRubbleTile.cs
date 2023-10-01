using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;
using Terraria.DataStructures;
using System;

namespace Redemption.Tiles.Tiles
{
    public class ShadestoneRubbleTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.Suffocate[Type] = true;
            TileID.Sets.Falling[Type] = true;
            DustType = ModContent.DustType<ShadestoneDust>();
            MinPick = 225;
            MineResist = 4f;
            AddMapEntry(new Color(65, 68, 83));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (WorldGen.noTileActions)
                return true;

            Tile above = Main.tile[i, j - 1];
            Tile below = Main.tile[i, j + 1];
            bool canFall = true;

            if (below == null || below.HasTile)
                canFall = false;

            if (above.HasTile && (TileID.Sets.BasicChest[above.TileType] || TileID.Sets.BasicChestFake[above.TileType] || above.TileType == TileID.PalmTree))
                canFall = false;

            if (canFall)
            {
                int projectileType = ModContent.ProjectileType<ShadestoneRubbleBall>();
                float positionX = i * 16 + 8;
                float positionY = j * 16 + 8;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.tile[i, j].ClearTile();
                    int proj = Projectile.NewProjectile(new EntitySource_TileBreak(i, j), positionX, positionY, 0f, 0.41f, projectileType, 10, 0f, Main.myPlayer);
                    Main.projectile[proj].ai[0] = 1f;
                    WorldGen.SquareTileFrame(i, j);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    Tile Mtile = Main.tile[i, j];
                    Mtile.HasTile = false;
                    bool spawnProj = true;

                    for (int k = 0; k < 1000; k++)
                    {
                        Projectile otherProj = Main.projectile[k];

                        if (otherProj.active && otherProj.owner == Main.myPlayer && otherProj.type == projectileType && Math.Abs(otherProj.timeLeft - 3600) < 60 && otherProj.Distance(new Vector2(positionX, positionY)) < 4f)
                        {
                            spawnProj = false;
                            break;
                        }
                    }

                    if (spawnProj)
                    {
                        int proj = Projectile.NewProjectile(new EntitySource_TileBreak(i, j), positionX, positionY, 0f, 2.5f, projectileType, 10, 0f, Main.myPlayer);
                        Main.projectile[proj].velocity.Y = 0.5f;
                        Main.projectile[proj].position.Y += 2f;
                        Main.projectile[proj].netUpdate = true;
                    }

                    NetMessage.SendTileSquare(-1, i, j, 1);
                    WorldGen.SquareTileFrame(i, j);
                }
                return false;
            }
            return true;
        }
    }
    public class ShadestoneRubbleBall : ModProjectile
    {
        protected bool falling = true;
        protected int tileType;
        protected int dustType;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadestone Rubble");
            ProjectileID.Sets.ForcePlateDetection[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.knockBack = 6f;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            tileType = ModContent.TileType<ShadestoneRubbleTile>();
            dustType = ModContent.DustType<ShadestoneDust>();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[dust].velocity.X *= 0.4f;
            }

            Projectile.tileCollide = true;
            Projectile.localAI[1] = 0f;

            if (Projectile.ai[0] == 1f)
            {
                if (!falling)
                {
                    Projectile.ai[1] += 1f;

                    if (Projectile.ai[1] >= 60f)
                    {
                        Projectile.ai[1] = 60f;
                        Projectile.velocity.Y += 0.2f;
                    }
                }
                else
                    Projectile.velocity.Y += 0.41f;
            }
            else if (Projectile.ai[0] == 2f)
            {
                Projectile.velocity.Y += 0.2f;

                if (Projectile.velocity.X < -0.04f)
                    Projectile.velocity.X += 0.04f;
                else if (Projectile.velocity.X > 0.04f)
                    Projectile.velocity.X -= 0.04f;
                else
                    Projectile.velocity.X = 0f;
            }

            Projectile.rotation += 0.1f;

            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hithoxCenterFrac)
        {
            if (falling)
                Projectile.velocity = Collision.AnyCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, true);
            else
                Projectile.velocity = Collision.TileCollision(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, fallThrough, fallThrough, 1);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && !Projectile.noDropItem)
            {
                int tileX = (int)(Projectile.position.X + Projectile.width / 2) / 16;
                int tileY = (int)(Projectile.position.Y + Projectile.width / 2) / 16;

                Tile tile = Framing.GetTileSafely(tileX, tileY);
                Tile tileBelow = Main.tile[tileX, tileY + 1];

                if (tile.IsHalfBlock && Projectile.velocity.Y > 0f && Math.Abs(Projectile.velocity.Y) > Math.Abs(Projectile.velocity.X))
                    tileY--;

                if (!tile.HasTile)
                {
                    bool onMinecartTrack = tileY < Main.maxTilesY - 2 && tileBelow != null && tileBelow.HasTile && tileBelow.TileType == TileID.MinecartTrack;

                    if (!onMinecartTrack)
                    {
                        WorldGen.PlaceTile(tileX, tileY, tileType, false, true);
                    }

                    if (!onMinecartTrack && tile.HasTile && tile.TileType == tileType)
                    {
                        if (tileBelow.IsHalfBlock || tileBelow.Slope != 0)
                        {
                            WorldGen.SlopeTile(tileX, tileY + 1, 0);

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 14, tileX, tileY + 1);
                        }

                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, tileX, tileY, tileType);
                    }
                }
            }
        }
    }
}