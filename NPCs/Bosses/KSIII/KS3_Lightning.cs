using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Projectiles;
using Redemption.Textures;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Lightning : ModRedeProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            ElementID.ProjThunder[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.rotation = RedeHelper.RandomRotation();
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[2]];
            if (!npc.active || npc.ModNPC is not KS3 || npc.ai[3] != 3)
                Projectile.Kill();

            Vector2 pos = new(npc.Center.X + 2 * npc.spriteDirection, npc.Center.Y - 16);
            Projectile.Center = pos;

            Vector2 targetPos = (Projectile.Center + RedeHelper.PolarVector(Projectile.ai[1], Projectile.rotation));
            if (Projectile.ai[0]++ == 0)
            {
                int maxTime = Main.rand.Next(20, 30);
                Vector2 direction = targetPos - Projectile.Center;
                ParticleSystem.NewParticle(Projectile.Center, direction * 1f, new ElectricSparkParticle(maxTime, 42), Color.Cyan * .3f, 1);
            }
            if (Projectile.ai[0] == 40)
            {
                Projectile.alpha = 0;
                if (Main.rand.NextBool())
                    SoundEngine.PlaySound(CustomSounds.Zap1 with { PitchVariance = .4f }, Projectile.position);
                else
                    SoundEngine.PlaySound(CustomSounds.Zap2.WithVolumeScale(.6f) with { PitchVariance = .4f }, Projectile.position);
                int maxTime = Main.rand.Next(20, 30);
                Vector2 direction = targetPos - Projectile.Center;
                ParticleSystem.NewParticle(Projectile.Center, direction * 1f, new ElectricSparkParticle(maxTime, 80, .4f, 12), Color.LightCyan, 1);
                RedeParticleManager.CreateDevilsPactParticle(targetPos, Vector2.Zero, 2f, new(161, 255, 253, 0));
            }
            if (Projectile.ai[0] >= 40)
                Projectile.alpha += 51;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> flare = CommonTextures.BigFlare;
            Vector2 flareOrigin = flare.Size() / 2;

            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, Color.Cyan.WithAlpha(0) * .5f * Projectile.Opacity, 0, flareOrigin, 1f, 0, 0);
            Main.EntitySpriteDraw(flare.Value, Projectile.Center - Main.screenPosition, null, Color.White.WithAlpha(0) * Projectile.Opacity, 0, flareOrigin, .6f, 0, 0);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] < 40)
                return false;
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + RedeHelper.PolarVector(Projectile.ai[1], Projectile.rotation), 20, ref point))
                return true;
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Electrified, 120);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffType<ElectrifiedDebuff>(), 120);
        }
    }
}