using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class Thorn_ClawSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjNature[Type] = true;
            ElementID.ProjBlood[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.Pi;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.Kill();
            }
            if (Projectile.frame < 2)
            {
                Projectile.velocity *= 1.2f;
                Projectile.scale *= 1.1f;
            }
            else if (Projectile.frame > 2)
            {
                Projectile.velocity *= .8f;
                Projectile.scale *= .9f;
                Projectile.alpha += 10;
            }
            if (Projectile.ai[0] != 0 || Main.getGoodWorld)
            {
                if (Projectile.ai[0] == 0)
                    Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;
                Projectile.velocity = Projectile.velocity.RotatedBy(.035f * Projectile.ai[0]);
            }
        }
        public override bool CanHitPlayer(Player target) => Projectile.frame < 3;
        public override bool? CanHitNPC(NPC target) => Projectile.frame < 3 ? null : false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = rect.Size() / 2;

            bool dance = Projectile.ai[0] != 0 || Main.getGoodWorld;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k];
                Color color = Projectile.GetAlpha(Color.White with { A = 0 }) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                float scale = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;

                Main.EntitySpriteDraw(texture, drawPos + Projectile.Size / 2f - Main.screenPosition, new Rectangle?(rect), color * (dance ? .5f : 1), Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale * scale, 0, 0f);
                if (dance)
                {
                    color = Projectile.GetAlpha(Color.Purple with { A = 0 }) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos + Projectile.Size / 2f - Main.screenPosition, new Rectangle?(rect), color, Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale * scale, 0, 0f);
                }
            }
            return false;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(3) || Main.expertMode)
                target.AddBuff(BuffID.Bleeding, 300);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 directionTo = target.DirectionTo(Projectile.Center);
            for (int i = 0; i < 16; i++)
            {
                Vector2 velocity = directionTo.RotatedBy(Main.rand.NextFloat(-1f, 1f) + 3.14f + Projectile.spriteDirection * MathHelper.PiOver4) * Main.rand.NextFloat(1f, 1.5f) + (Projectile.velocity / 16);
                Dust.NewDustPerfect(target.Center + directionTo * 10 + Projectile.velocity, DustID.Blood, velocity, 0, Scale: Main.rand.NextFloat(1f, 2f));
                ParticleSystem.NewParticle(Main.rand.NextVector2FromRectangle(target.Hitbox), velocity * 10, new SpeedParticle(isAdditive: false), Color.DarkRed, 1f);
            }
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Slash2, Projectile.position);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Vector2 directionTo = target.DirectionTo(Projectile.Center);
            for (int i = 0; i < 16; i++)
            {
                Vector2 velocity = directionTo.RotatedBy(Main.rand.NextFloat(-1f, 1f) + 3.14f + Projectile.spriteDirection * MathHelper.PiOver4) * Main.rand.NextFloat(1f, 1.5f) + (Projectile.velocity / 16);
                Dust.NewDustPerfect(target.Center + directionTo * 10 + Projectile.velocity, DustID.Blood, velocity, 0, Scale: Main.rand.NextFloat(1f, 2f));
                ParticleSystem.NewParticle(Main.rand.NextVector2FromRectangle(target.Hitbox), velocity * 10, new SpeedParticle(isAdditive: false), Color.DarkRed, 1f);
            }
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Slash2, Projectile.position);
        }
    }
}