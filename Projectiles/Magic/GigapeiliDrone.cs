using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Dusts;

namespace Redemption.Projectiles.Magic
{
    public class GigapeiliDrone : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Minions/MicroshieldDrone";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gigapeili Drone");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.localAI[0] = 10;
        }
        private NPC target;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.7f, 0f, 0f);
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
                Projectile.Kill();

            if (Projectile.ai[0] is 1)
            {
                Projectile.localAI[0]++;
                Projectile.rotation.SlowRotation((player.Center - Projectile.Center).ToRotation(), MathHelper.Pi / 90);
                Projectile.Move(player.Center, Projectile.localAI[0], 1);
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    SoundEngine.PlaySound(CustomSounds.ShootChange, player.position);
                    Projectile.Kill();
                }
                return;
            }
            if (Projectile.timeLeft < 90)
                Projectile.ai[0] = 1;
            if (Projectile.timeLeft > 3540)
            {
                Projectile.rotation = Projectile.localAI[0];
                Projectile.localAI[0] *= 0.9f;
            }
            else
            {
                if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center))
                    Projectile.rotation.SlowRotation((Projectile.Center - target.Center).ToRotation() - MathHelper.PiOver2, MathHelper.Pi / 10);
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                    Projectile proj = Main.projectile[j];
                    if (!proj.active || proj.type != ModContent.ProjectileType<GigapeiliBolt>() || proj.ai[0] == 1)
                        continue;
                    if (!Projectile.Hitbox.Intersects(proj.Hitbox))
                        continue;

                    for (int i = 0; i < 3; i++)
                    {
                        int dust = Dust.NewDust(proj.Center - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, .5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0;
                        Color dustColor = new(255, 176, 70) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }
                    SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
                    proj.velocity = RedeHelper.PolarVector(16, Projectile.rotation - MathHelper.PiOver2);
                    proj.damage = (int)(proj.damage * 1.5f);
                    proj.timeLeft = 120;
                    proj.ai[0] = 1;
                }
            }
            Projectile.velocity *= .96f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), (Projectile.Center - player.Center).ToRotation() - MathHelper.PiOver2, drawOrigin, Projectile.scale, effects, 0);
            return true;
        }
    }
}