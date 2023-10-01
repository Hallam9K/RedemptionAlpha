using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Rockslide_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rockslide");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjPsychic[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.rotation = RedeHelper.RandomRotation();
            Projectile.alpha = 255;
            Projectile.frame = Main.rand.Next(4);
            Rand = Main.rand.Next(50, 100);
            Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
            Projectile.extraUpdates = 1;
            double angle = Main.rand.NextDouble() * 2d * Math.PI;
            MoveVector2.X = (float)(Math.Sin(angle) * Rand);
            MoveVector2.Y = (float)(Math.Cos(angle) * Rand);
        }
        public override bool? CanCutTiles() => Projectile.ai[0] != 0;
        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[0] != 0 ? null : false;
        public Vector2 MoveVector2;
        public Vector2 pos = new(0, -5);
        public ref float Rand => ref Projectile.localAI[0];
        private bool shoot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += (Projectile.ai[0] == 0 ? 0.01f : 0.2f) * Projectile.spriteDirection;
            if (Projectile.alpha > 0)
                Projectile.alpha -= 5;
            if (Projectile.ai[1]++ < 60)
                pos *= 0.98f;
            else
            {
                if (Projectile.localAI[1] == 0)
                {
                    pos.Y += 0.03f;
                    if (pos.Y > .7f)
                        Projectile.localAI[1] = 1;
                }
                else if (Projectile.localAI[1] == 1)
                {
                    pos.Y -= 0.03f;
                    if (pos.Y < -.7f)
                        Projectile.localAI[1] = 0;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] == 0)
                {
                    Projectile.timeLeft = 200;
                    Projectile.position = player.Center + MoveVector2;
                    MoveVector2 += pos;
                    if (shoot && Main.rand.NextBool(10) && Projectile.alpha <= 0)
                    {
                        Projectile.tileCollide = true;
                        SoundEngine.PlaySound(SoundID.Item70, Projectile.position);
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 20;
                        Projectile.ai[0] = 1;
                    }
                }
            }
            if (!player.channel)
                shoot = true;
        }
        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 4;

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.SpreadUp(7), ModContent.ProjectileType<RockslidePebble_Proj>(), Projectile.damage / 2, 1, Main.myPlayer);
                }
            }
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone,
                    -Projectile.velocity.X * 0.3f, -Projectile.velocity.Y * 0.3f, Scale: 2);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            if (Projectile.ai[0] == 1)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(Color.Pink) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.Pink, Projectile.rotation, drawOrigin, Projectile.scale);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class RockslidePebble_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pebble");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 40 * Projectile.direction;
            Projectile.velocity.Y += 0.35f;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Pink) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}