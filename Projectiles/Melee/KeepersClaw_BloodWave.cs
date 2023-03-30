using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Keeper;
using Terraria;
using Terraria.ID;

namespace Redemption.Projectiles.Melee
{
    public class KeepersClaw_BloodWave : KeeperBloodWave
    {
        public override string Texture => "Redemption/NPCs/Bosses/Keeper/KeeperBloodWave";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Wave");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjBlood[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, Projectile.Opacity * 0.2f, Projectile.Opacity * 0.2f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.alpha += 5;
            if (Collision.SolidCollision(Projectile.Center - Vector2.One, 1, 1))
            {
                Projectile.velocity *= .9f;
                Projectile.alpha += 5;
            }
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.alpha < 200 && Projectile.ai[0] < 2 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 40;
            target.immune[Projectile.owner] = 0;

            Projectile.ai[0]++;
            target.AddBuff(BuffID.Bleeding, 260);
            Player player = Main.player[Projectile.owner];
            if (player.statLife < player.statLifeMax2 - 5)
            {
                int steps = (int)player.Distance(target.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(player.Center, target.Center, (float)i / steps), 2, 2, DustID.LifeDrain);
                        dust.velocity = target.DirectionTo(dust.position) * 2;
                        dust.noGravity = true;
                    }
                }
                player.statLife += 5;
                player.HealEffect(5);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) { }
    }
}