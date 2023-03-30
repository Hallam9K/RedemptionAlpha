using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class AcornBomb_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn Bomb");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ElementID.ProjNature[Type] = true;
            ElementID.ProjExplosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.scale = 0.2f;
        }
        public override void AI()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            if (Projectile.scale < 1)
                Projectile.scale += 0.1f;

            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            if (Projectile.localAI[0] < 180)
                Projectile.velocity.Y += 0.2f;

            if (Projectile.localAI[0]++ == 180)
            {
                Projectile.velocity *= 0;
                Projectile.alpha = 255;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: 2f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                for (int i = 0; i < 2; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].velocity *= 3f;
                    dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.SpreadUp(8), ModContent.ProjectileType<AcornBomb_Shard>(), Projectile.damage / 3, 1, Main.myPlayer);
                    }
                }
                Rectangle boom = new((int)Projectile.Center.X - 40, (int)Projectile.Center.Y - 40, 80, 80);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                        continue;

                    target.immune[Projectile.whoAmI] = 20;
                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                }
            }
            if (Projectile.localAI[0] == 182)
                Projectile.friendly = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.whoAmI] = 20;
            if (Projectile.localAI[0] < 180)
                Projectile.localAI[0] = 180;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity.Y *= 0.6f;
            Projectile.velocity.X *= 0.7f;
            return false;
        }
    }
}