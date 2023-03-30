using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.PreHM
{
    public class SkeletonWanderer_SpearProj : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 20f;
        protected virtual float HoldoutRangeMax => 48f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spear");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Redemption().TechnicallyMelee = true;
            Projectile.Redemption().friendlyHostile = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            return target == host.Redemption().attacker ? null : false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override bool CanHitPlayer(Player target)
        {
            return !target.RedemptionPlayerBuff().skeletonFriendly;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), Main.rand.Next(400, 1200));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<DirtyWoundDebuff>(), Main.rand.Next(400, 1200));
        }
        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (host.ai[0] != 3 || !host.active)
                Projectile.Kill();

            if (Projectile.ai[1] == 1)
                Projectile.frame = 1;
            else
                Projectile.frame = 0;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.velocity.X > 0)
                    Projectile.spriteDirection = 1;
                else if (Projectile.velocity.X < 0)
                    Projectile.spriteDirection = -1;

                if (Projectile.spriteDirection == -1)
                    Projectile.rotation += MathHelper.ToRadians(135f);
                else
                    Projectile.rotation += MathHelper.ToRadians(45f);
                Projectile.localAI[0] = 1;
            }

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            int duration = 42;

            if (Projectile.timeLeft > duration)
                Projectile.timeLeft = duration;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            float halfDuration = duration * 0.5f;
            float progress;

            if (Projectile.timeLeft < halfDuration)
                progress = Projectile.timeLeft / halfDuration;
            else
                progress = (duration - Projectile.timeLeft) / halfDuration;

            Projectile.Center = npc.Center + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(0, 8), new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}