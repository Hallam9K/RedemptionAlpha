using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Globals.Player;
using Redemption.Projectiles.Ranged;
using Redemption.BaseExtension;
using Redemption.Effects.PrimitiveTrails;

namespace Redemption.Projectiles.Melee
{
    public class MiniSpaceship : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Microship Mk.I");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 800;
            Projectile.Redemption().Unparryable = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(Color.Cyan, Color.White), new RoundCap(), new ArrowGlowPosition(), 6f, 200f);
        }

        private Vector2 move;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            float soundVolume = Projectile.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24.WithVolume(soundVolume), Projectile.position);
                Projectile.soundDelay = 10;
            }

            Player player = Main.player[Projectile.owner];
            RedePlayer modPlayer = player.Redemption();
            if (Projectile.localAI[0] == 0)
            {
                DustHelper.DrawCircle(Projectile.Center, DustID.Frost, 2, 2, 2, 1, 2, nogravity: true);
                Projectile.localAI[0] = 1;
            }
            int getNPC = RedeHelper.GetNearestNPC(Projectile.Center);
            if (getNPC != -1 && Main.myPlayer == Projectile.owner)
                Projectile.rotation = (Main.npc[getNPC].Center - Projectile.Center).ToRotation();
            else
                Projectile.rotation = (player.Center - Projectile.Center).ToRotation() - MathHelper.PiOver2;

            if (Projectile.timeLeft >= 120)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.ai[1] += 4;
                    if (Projectile.ai[0] == 0)
                        move = player.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[1])) * 50;
                    else
                        move = player.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[1] + 180)) * 50;

                    Projectile.Move(move, 20, 10);
                    if (Projectile.ai[0] == 0)
                    {
                        if (modPlayer.hitTarget != -1)
                        {
                            SoundEngine.PlaySound(SoundID.Item12, Projectile.position);
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(8, Projectile.rotation), ModContent.ProjectileType<MiniSpaceship_Laser>(), 1 + (player.HeldItem.damage / 4), Projectile.knockBack, Projectile.owner);
                            modPlayer.hitTarget = -1;
                        }
                    }
                    else
                    {
                        if (modPlayer.hitTarget2 != -1)
                        {
                            SoundEngine.PlaySound(SoundID.Item12, Projectile.position);
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(8, Projectile.rotation), ModContent.ProjectileType<MiniSpaceship_Laser>(), 1 + (player.HeldItem.damage / 4), Projectile.knockBack, Projectile.owner);
                            modPlayer.hitTarget2 = -1;
                        }
                    }
                }
            }
            else
            {
                Projectile.velocity.Y -= 0.4f;
                Projectile.velocity.X *= 0.9f;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 4.4f;
            }
        }
    }
}