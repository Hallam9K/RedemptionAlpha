using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ammo
{
    public class BileArrow_Proj : ModProjectile
	{
        public override string Texture => "Redemption/Items/Weapons/HM/Ammo/BileArrow";
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bile Arrow");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
		{
			Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
			AIType = ProjectileID.CursedArrow;
            Projectile.arrow = true;
		}
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        public override void AI()
        {
            int d = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenFairy, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Main.dust[d].noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.velocity.Y += 0.034f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BileDebuff>(), 600);
        }
    }
}
