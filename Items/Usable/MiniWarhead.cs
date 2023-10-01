using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class MiniWarhead : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Warhead");
            /* Tooltip.SetDefault("A huge explosion that will destroy most tiles\n" +
                "'I don't want to set the world on fire'"); */
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 22;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<MiniWarhead_Proj>();
            Item.shootSpeed = 7f;
        }
    }
    public class MiniWarhead_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Usable/MiniWarhead";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Warhead");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.hostile = false;
            Projectile.friendly = false;
            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.expertMode && target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
                modifiers.FinalDamage /= 5;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.soundDelay = 10;

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity.Y *= 0.3f;
            Projectile.velocity.X *= 0.8f;
            return false;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 1)
            {
                RedeDraw.SpawnExplosion(Projectile.Center, Color.OrangeRed, DustID.Torch, 30, 0);
                Projectile.friendly = true;
                Projectile.hostile = true;
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.width = 500;
                Projectile.height = 500;
                Projectile.damage = 500;
                RedeDraw.SpawnExplosion(Projectile.Center, Color.OrangeRed, DustID.Torch, 30, 0);
                Rectangle boom = new((int)Projectile.Center.X - 250, (int)Projectile.Center.Y - 250, 500, 500);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                        continue;

                    target.immune[Projectile.whoAmI] = 20;
                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
                }
                Projectile.knockBack = 10f;
            }
            if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                Projectile.velocity.X = 0f;

            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.width = 10;
            Projectile.height = 10;
            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: 3f);
                Main.dust[dustIndex].velocity *= 20f;
            }
            for (int i = 0; i < 80; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 15f;
                dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 3f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            for (int g = 0; g < 18; g++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64), 2f);
                Main.gore[goreIndex].velocity.X *= 15f;
            }

            int explosionRadius = 26;
            int minTileX = (int)(Projectile.position.X / 16f - explosionRadius);
            int maxTileX = (int)(Projectile.position.X / 16f + explosionRadius);
            int minTileY = (int)(Projectile.position.Y / 16f - explosionRadius);
            int maxTileY = (int)(Projectile.position.Y / 16f + explosionRadius);
            if (minTileX < 0)
                minTileX = 0;
            if (maxTileX > Main.maxTilesX)
                maxTileX = Main.maxTilesX;
            if (minTileY < 0)
                minTileY = 0;
            if (maxTileY > Main.maxTilesY)
                maxTileY = Main.maxTilesY;
            bool canKillWalls = false;
            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int y = minTileY; y <= maxTileY; y++)
                {
                    float diffX = Math.Abs(x - Projectile.position.X / 16f);
                    float diffY = Math.Abs(y - Projectile.position.Y / 16f);
                    double distance = Math.Sqrt(diffX * diffX + diffY * diffY);
                    if (distance < explosionRadius && Main.tile[x, y] != null && Main.tile[x, y].WallType == 0)
                    {
                        canKillWalls = true;
                        break;
                    }
                }
            }
            AchievementsHelper.CurrentlyMining = true;
            for (int i = minTileX; i <= maxTileX; i++)
            {
                for (int j = minTileY; j <= maxTileY; j++)
                {
                    float diffX = Math.Abs(i - Projectile.position.X / 16f);
                    float diffY = Math.Abs(j - Projectile.position.Y / 16f);
                    double distanceToTile = Math.Sqrt(diffX * diffX + diffY * diffY);
                    if (distanceToTile < explosionRadius)
                    {
                        bool canKillTile = true;
                        if (Main.tile[i, j] != null && Main.tile[i, j].HasTile)
                        {
                            canKillTile = true;
                            if (Main.tileDungeon[Main.tile[i, j].TileType] || Main.tile[i, j].TileType == 88 || Main.tile[i, j].TileType == 21 || Main.tile[i, j].TileType == 26 || Main.tile[i, j].TileType == 107 || Main.tile[i, j].TileType == 108 || Main.tile[i, j].TileType == 111 || Main.tile[i, j].TileType == 226 || Main.tile[i, j].TileType == 237 || Main.tile[i, j].TileType == 221 || Main.tile[i, j].TileType == 222 || Main.tile[i, j].TileType == 223 || Main.tile[i, j].TileType == 211 || Main.tile[i, j].TileType == 404)
                                canKillTile = false;

                            if (!Main.hardMode && Main.tile[i, j].TileType == 58)
                                canKillTile = false;

                            if (!TileLoader.CanExplode(i, j))
                                canKillTile = false;

                            if (canKillTile)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                                if (!Main.tile[i, j].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
                            }
                        }
                        if (canKillTile)
                        {
                            for (int x = i - 1; x <= i + 1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    if (Main.tile[x, y] != null && Main.tile[x, y].WallType > 0 && canKillWalls && WallLoader.CanExplode(x, y, Main.tile[x, y].WallType))
                                    {
                                        WorldGen.KillWall(x, y, false);
                                        if (Main.tile[x, y].WallType == 0 && Main.netMode != NetmodeID.SinglePlayer)
                                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, x, y, 0f, 0, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            AchievementsHelper.CurrentlyMining = false;
        }
    }
}