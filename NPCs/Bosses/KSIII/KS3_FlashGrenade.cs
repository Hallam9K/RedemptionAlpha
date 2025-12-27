using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Projectiles;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_FlashGrenade : ModRedeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stun Grenade");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 24;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 90;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.4f, Projectile.Opacity * 0.4f, Projectile.Opacity);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation += Projectile.velocity.X / 40 * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Zap2, Projectile.position);
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 100, default, 4f);
                Main.dust[dustIndex].velocity *= 12f;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<FlashGrenadeBlast>(), Projectile.damage, 0, Main.myPlayer);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.98f;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Rectangle rect = texture.Frame(1, 3, 0, Projectile.frame);
            Vector2 drawOrigin = rect.Size() / 2;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture.Value, drawPos, rect, color, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            }

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            return true;
        }
    }
    public class FlashGrenadeBlast : ModRedeProjectile
    {
        public override string Texture => "Redemption/Textures/TransitionTex";
        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.scale = 2;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha <= 0)
                    Projectile.localAI[0] = 1;
            }
            else
            {
                if (Projectile.timeLeft < 200)
                    Projectile.alpha += 3;
            }

            if (Projectile.localAI[1]++ <= 6)
            {
                foreach (Player target in Main.ActivePlayers)
                {
                    if (target.dead || Projectile.DistanceSQ(target.Center) >= 60 * 60)
                        continue;

                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamagePlayer(target, Projectile.damage, 4.5f, hitDirection, Projectile);

                    target.AddBuff(BuffID.Confused, 180);
                    target.AddBuff(BuffType<StunnedDebuff>(), 60);
                }
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (!target.CanBeChasedBy() || Projectile.DistanceSQ(target.Center) >= 60 * 60)
                        continue;

                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamageNPC(target, Projectile.damage, 4.5f, hitDirection, Projectile);

                    target.AddBuff(BuffID.Confused, 180);
                    if (target.knockBackResist > 0)
                        target.AddBuff(BuffType<StunnedDebuff>(), 60);
                }
            }

            if (Projectile.alpha < 150)
            {
                foreach (Player target in Main.ActivePlayers)
                {
                    if (target.dead || Projectile.DistanceSQ(target.Center) >= 400 * 400)
                        continue;

                    target.AddBuff(BuffID.Obstructed, 10);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}