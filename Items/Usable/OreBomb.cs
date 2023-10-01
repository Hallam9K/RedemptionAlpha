using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class OreBomb : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("A small explosion that will spread basic ores");
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DirtBomb);
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.shoot = ModContent.ProjectileType<OreBomb_Proj>();
        }
    }
    public class OreBomb_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Usable/OreBomb";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ore Bomb");
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DirtBomb);
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.hostile = false;
            Projectile.friendly = false;
            DrawOffsetX = 0;
            DrawOriginOffsetY = -8;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.soundDelay = 10;

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity.Y *= 0.3f;
            Projectile.velocity.X *= 0.84f;
            return false;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 1)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
            }
            if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                Projectile.velocity.X = 0f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.width = 10;
            Projectile.height = 10;
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                Main.dust[dustIndex].velocity *= 10f;
            }
            for (int i = 0; i < 40; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 8f;
                int[] OreDust = new int[] { DustID.Copper, DustID.Tin, DustID.Iron, DustID.Lead, DustID.Silver, DustID.Tungsten, DustID.Gold, DustID.Platinum };
                dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Utils.Next(Main.rand, OreDust), Scale: 2);
                Main.dust[dustIndex].velocity *= 3f;
            }
            for (int g = 0; g < 3; g++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64), 2f);
                Main.gore[goreIndex].velocity.X *= 5f;
            }

            for (int k = 0; k < 2; k++)
            {
                int explosionRadius = 7 - (k * 2);
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
                            if (Main.tileDungeon[Main.tile[i, j].TileType] || Main.tile[i, j].TileType == 88 || Main.tile[i, j].TileType == 21 || Main.tile[i, j].TileType == 26 || Main.tile[i, j].TileType == 107 || Main.tile[i, j].TileType == 108 || Main.tile[i, j].TileType == 111 || Main.tile[i, j].TileType == 226 || Main.tile[i, j].TileType == 237 || Main.tile[i, j].TileType == 221 || Main.tile[i, j].TileType == 222 || Main.tile[i, j].TileType == 223 || Main.tile[i, j].TileType == 211 || Main.tile[i, j].TileType == 404)
                                canKillTile = false;

                            if (!Main.hardMode && Main.tile[i, j].TileType == 58)
                                canKillTile = false;

                            if (!TileLoader.CanExplode(i, j))
                                canKillTile = false;

                            if (canKillTile)
                            {
                                int[] OreType = new int[] { TileID.Copper, TileID.Tin, TileID.Iron, TileID.Lead, TileID.Silver, TileID.Tungsten, TileID.Gold, TileID.Platinum };
                                WorldGen.PlaceTile(i, j, k == 0 ? TileID.Stone : Utils.Next(Main.rand, OreType), false, true);
                                if (!Main.tile[i, j].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
                            }
                        }
                    }
                }
            }
        }
    }
}