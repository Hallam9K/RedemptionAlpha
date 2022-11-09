using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class XeniumBubble_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenium Bubble");
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 10;
        }
        public override void PostAI()
        {

        }
        public override void Kill(int timeLeft)
        {
            Projectile.friendly = true;
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 120 * 120)
                    continue;

                int hitDirection = Projectile.Center.X > target.Center.X ? -1 : 1;
                BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile);
            }
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, Scale: 2);
                Main.dust[dustIndex].velocity *= 5f;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 300);
            else if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 150);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }
    }
}