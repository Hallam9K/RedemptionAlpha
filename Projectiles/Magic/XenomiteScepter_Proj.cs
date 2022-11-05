using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Terraria.Audio;
using Redemption.Base;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Buffs.Debuffs;

namespace Redemption.Projectiles.Magic
{
    public class XenomiteScepter_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Helix Bolt");
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }
        public float vectorOffset = 0f;
        public Vector2 originalVelocity;

        public override void AI()
        {
            ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new GlowParticle2(), Color.Green, 0.6f, 0, 1);

            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;
            if (Projectile.ai[0] == 0)
            {
                vectorOffset -= 0.4f;
                if (vectorOffset <= -1.3f)
                {
                    vectorOffset = -1.3f;
                    Projectile.ai[0] = 1;
                }
            }
            else
            {
                vectorOffset += 0.4f;
                if (vectorOffset >= 1.3f)
                {
                    vectorOffset = 1.3f;
                    Projectile.ai[0] = 0;
                }
            }
            float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
            Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 300);
            else if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 150);
        }
        public override void Kill(int timeLeft)
        {
            int pieCut = 20;
            for (int m = 0; m < pieCut; m++)
            {
                ParticleManager.NewParticle(Projectile.Center, BaseUtility.RotateVector(default, new Vector2(2f, 0f), m / (float)pieCut * 6.28f), new GlowParticle2(), Color.Green, 0.6f, 0, 1);
            }
            for (int m = 0; m < pieCut; m++)
            {
                ParticleManager.NewParticle(Projectile.Center, BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)pieCut * 6.28f), new GlowParticle2(), Color.Green, 0.6f, 0, 1);
            }
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
        }
    }
}