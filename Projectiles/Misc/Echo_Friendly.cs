using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class Echo_Friendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Echo");
            Main.projFrames[Projectile.type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 200;
        }
        private NPC target;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 10)
                    Projectile.frame = 0;
            }
            Projectile.LookByVelocity();
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Projectile.timeLeft > 120)
                Projectile.velocity *= 0.98f;
            else
            {
                Projectile.friendly = true;
                if (RedeHelper.ClosestNPC(ref target, 900, Projectile.Center, true, player.MinionAttackTargetNPC))
                    Projectile.Move(target.Center, Projectile.timeLeft > 60 ? 15 : 20, 20);
                else
                    Projectile.velocity *= 0.98f;
            }
        }
        public override Color? GetAlpha(Color lightColor) => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.GhostWhite, Color.Black, Color.GhostWhite);
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Scale: 2f);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
    }
}