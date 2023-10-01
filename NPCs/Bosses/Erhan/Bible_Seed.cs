using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Dusts;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class Bible_Seed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Seed of Virtue");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 0;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.Center.Y < player.Center.Y)
                fallThrough = true;
            else
                fallThrough = false;
            return true;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 30 * Projectile.direction;
            Projectile.velocity.Y += 0.25f;
            Projectile.velocity.X = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = new Color(255, 255, 120) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < 30; i++)
            {
                int num5 = Dust.NewDust(Projectile.position - new Vector2(90, 14), Projectile.width + 180, Projectile.height + 14, ModContent.DustType<GlowDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Color dustColor = new(255, 255, 209) { A = 0 };
                Main.dust[num5].fadeIn = 0.05f;
                Main.dust[num5].noGravity = true;
                Main.dust[num5].velocity.Y = -7;
                Main.dust[num5].color = dustColor * Projectile.Opacity;

                int dust = Dust.NewDust(Projectile.position - new Vector2(90, 14), Projectile.width + 180, Projectile.height + 14, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity.Y = -7;
            }
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Bible_SpearSpawner>(), Projectile.damage, 3, Projectile.owner);
            }
            return true;
        }
    }
    public class Bible_SpearSpawner : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Seed of Virtue");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 24;
            Projectile.alpha = 0;
        }
        public override void AI()
        {
            if (Projectile.localAI[1]++ % 2 == 0)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 origin = Projectile.Center;
                    origin.X += Projectile.localAI[1] * 5 * i;
                    int numtries = 0;
                    int x = (int)(origin.X / 16);
                    int y = (int)(origin.Y / 16);
                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                    {
                        y++;
                        origin.Y = y * 16;
                    }
                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                    {
                        numtries++;
                        y--;
                        origin.Y = y * 16;
                    }
                    if (numtries >= 20)
                        break;

                    SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);
                    if (Main.netMode != NetmodeID.Server && Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), origin, Vector2.Zero, ModContent.ProjectileType<Bible_SeedSpear>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Main.rand.NextFloat(0.5f, 1.5f));
                }
            }
        }
    }
}