using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class SwarmGrowth_Proj : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/SeedOfInfection/SeedGrowth";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Swarm Growth");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.penetrate = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
        }
        private NPC target;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (RedeHelper.ClosestNPC(ref target, 1000, Projectile.Center, false, Main.player[Projectile.owner].MinionAttackTargetNPC))
            {
                Projectile.Move(target.Center, 30, 20);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.3f }, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenBlood, Scale: 2f);
                Main.dust[dustIndex].velocity *= 2;
            }
            for (int i = 0; i < 8; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Green);
                Main.dust[dustIndex].velocity *= 0.3f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;

            target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 300);
        }
    }
}