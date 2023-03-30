using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.NPCs.Lab.MACE;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Chernobyl_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chernobyl");
            ElementID.ProjFire[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.Redemption().TechnicallyMelee = true;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 310f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 17f;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < 30; i++)
            {
                float distance = Main.rand.Next(10) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = dustPosition - nextDustPosition + Projectile.velocity;
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.GreenTorch, dustVelocity, Scale: 0.5f);
                    dust.scale = distance / 30;
                    dust.scale = MathHelper.Clamp(dust.scale, 0.2f, 3);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || (player.dontHurtCritters && npc.lifeMax <= 5))
                    continue;

                if (Projectile.DistanceSQ(npc.Center) > 70 * 70)
                    continue;

                npc.AddBuff(ModContent.BuffType<BileDebuff>(), 180);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life < 0 && target.lifeMax > 5)
            {
                Rectangle boom = new((int)Projectile.Center.X - 48, (int)Projectile.Center.Y - 48, 96, 96);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || !npc.CanBeChasedBy())
                        continue;

                    if (npc.immune[Projectile.whoAmI] > 0 || !npc.Hitbox.Intersects(boom))
                        continue;

                    npc.immune[Projectile.whoAmI] = 20;
                    int hitDirection = target.RightOfDir(Projectile);
                    BaseAI.DamageNPC(npc, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                }

                SoundEngine.PlaySound(SoundID.Item14, target.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.rand.Next(3, 6); i++)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-9, -5)), ModContent.ProjectileType<MACE_Miniblast>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 1);
                        Main.projectile[proj].timeLeft = 300;
                        Main.projectile[proj].hostile = false;
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].netUpdate = true;
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex3 = Dust.NewDust(target.position, target.width, target.height, DustID.Smoke, Scale: 2f);
                    Main.dust[dustIndex3].velocity *= 2f;
                }
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex4 = Dust.NewDust(target.position, target.width, target.height, DustID.Torch, Scale: 3f);
                    Main.dust[dustIndex4].noGravity = true;
                    Main.dust[dustIndex4].velocity *= 5f;
                    dustIndex4 = Dust.NewDust(target.position, target.width, target.height, DustID.Torch, Scale: 2f);
                    Main.dust[dustIndex4].velocity *= 3f;
                }
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int g = 0; g < 3; g++)
                {
                    int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), target.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                }
            }
        }
    }
}