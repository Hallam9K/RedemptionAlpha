using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class DragonCleaverSkull_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon Skull");
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 40;
            Projectile.timeLeft = 75;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        private int Dir;
        private float jawRot;
        private bool found;
        NPC target;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[1]++;

            if (Projectile.ai[0]++ == 0)
            {
                Dir = player.direction;
                Projectile.velocity *= 0.1f;
            }
            if (Projectile.ai[0] < 20)
            {
                Projectile.Center = Projectile.Center;
                Projectile.alpha -= 20;
            }
            if (Projectile.ai[0] == 10)
            {
                Projectile.friendly = true;
            }

            if (Projectile.ai[0] == 20)
            {
                Projectile.velocity *= 150f;
            }
            if (Projectile.ai[0] > 20 && Projectile.ai[0] < 60)
                jawRot += 0.1f;

            if (Projectile.ai[0] > 20 && RedeHelper.ClosestNPC(ref target, 900, Projectile.Center, true) && !found)
            {
                if (Projectile.DistanceSQ(target.Center) <= 50 * 50)
                {
                    Projectile.ai[0] = 50;
                    found = true;
                }
            }

            if (!found && Projectile.ai[1] <= 50)
                Projectile.ai[0] = MathHelper.Clamp(Projectile.ai[0], 0, 49);

            if (Projectile.ai[0] > 50)
                Projectile.velocity *= 0.75f;

            if (Projectile.ai[0] == 55)
                SoundEngine.PlaySound(SoundID.Item2 with { Pitch = -.1f }, Projectile.Center);
            if (Projectile.ai[0] > 55)
                jawRot -= 0.1f;

            if (Projectile.ai[0] > 65)
            {
                Projectile.friendly = false;
                Projectile.alpha += 40;
            }

            jawRot = MathHelper.Clamp(jawRot, 0, 0.5f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] > 20 && !found)
            {
                Projectile.ai[0] = 50;
                found = true;
            }
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = .8f }, Projectile.position);
            player.RedemptionScreen().ScreenShakeIntensity += 2;

            for (int i = 0; i < 4; i++)
            {
                float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                float randomVel = Main.rand.NextFloat(2f, 4f);
                Vector2 direction = Projectile.velocity.SafeNormalize(default);
                Vector2 position = target.Center - direction * 10;
                ParticleSystem.NewParticle(position, direction.RotatedBy(randomRotation) * randomVel * 10, new SpeedParticle(), Color.Salmon, 0.8f);
            }

            target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dragonlike.Contains(target.type))
                modifiers.FinalDamage *= 4;
        }
        Asset<Texture2D> topTex;
        Asset<Texture2D> bottomTex;
        public override bool PreDraw(ref Color lightColor)
        {
            topTex ??= ModContent.Request<Texture2D>("Redemption/Projectiles/Melee/DragonSkullTop2_Proj");
            bottomTex ??= ModContent.Request<Texture2D>("Redemption/Projectiles/Melee/DragonSkullBottom2_Proj");

            Vector2 drawOrigin = new(Projectile.width / 2, Projectile.height / 2);
            var effects = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 offset = Projectile.velocity.SafeNormalize(default) * 30f;

            Main.EntitySpriteDraw(bottomTex.Value, Projectile.Center - offset - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation + (jawRot * Dir), drawOrigin + new Vector2(Dir == -1 ? -12 : -18, Dir == -1 ? 12 : -12), Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(topTex.Value, Projectile.Center - offset - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation - (jawRot * Dir), drawOrigin + new Vector2(Dir == -1 ? -6 : -13, Dir == -1 ? -10 : 18), Projectile.scale, effects, 0);
            return false;
        }
    }
}