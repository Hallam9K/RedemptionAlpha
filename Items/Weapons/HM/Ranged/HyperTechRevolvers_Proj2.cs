using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class HyperTechRevolvers_Proj2 : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Ranged/HyperTechRevolvers_Proj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hyper-Tech Revolver");
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.3f * Projectile.ai[0];
            Projectile.velocity.Y += 0.2f;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.localAI[0]++ >= 30)
                {
                    if (player.Hitbox.Intersects(Projectile.Hitbox))
                    {
                        SoundEngine.PlaySound(CustomSounds.ShootChange, Projectile.Center);
                        if (player.HasBuff<RevolverTossBuff2>())
                            player.AddBuff(ModContent.BuffType<RevolverTossBuff2>(), 420);
                        else if (player.HasBuff<RevolverTossBuff3>())
                            player.AddBuff(ModContent.BuffType<RevolverTossBuff3>(), 420);
                        else
                            player.AddBuff(ModContent.BuffType<RevolverTossBuff>(), 420);
                        Projectile.localAI[0] = 1;
                        Projectile.Kill();
                    }
                }
            }
            if (Projectile.timeLeft <= 200)
                Projectile.tileCollide = true;
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] != 1)
            {
                player.AddBuff(ModContent.BuffType<RevolverTossDebuff>(), 300);
                player.ClearBuff(ModContent.BuffType<RevolverTossBuff3>());
                player.ClearBuff(ModContent.BuffType<RevolverTossBuff2>());
                player.ClearBuff(ModContent.BuffType<RevolverTossBuff>());
            }
            else
            {
                for (int g = 0; g < Main.maxProjectiles; ++g)
                {
                    if (Main.projectile[g].active && Main.projectile[g].owner == player.whoAmI && Main.projectile[g].type == ModContent.ProjectileType<HyperTechRevolvers_Proj>())
                    {
                        Main.projectile[g].Kill();
                        break;
                    }
                }
            }
        }
    }
}
