using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Effects;

namespace Redemption.Projectiles.Magic
{
    public class SoulScepterCharge : ModProjectile, ITrailProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Keeper/KeeperSoulCharge";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Charge");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 200;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new StandardColorTrail(Color.GhostWhite), new RoundCap(), new ArrowGlowPosition(), 32f, 250f);
        }

        private Vector2 move;
        public override void AI()
        {
            Projectile.timeLeft = 10;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 1f / 255f, (255 - Projectile.alpha) * 1f / 255f, (255 - Projectile.alpha) * 1f / 255f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            if (Main.myPlayer == Projectile.owner && Projectile.localAI[0] == 0)
            {
                Player player = Main.player[Projectile.owner];
                if (player.channel)
                {
                    Projectile.ai[0] += 4;
                    Projectile.ai[1] += 0.2f;
                    Projectile.ai[1] = MathHelper.Clamp(Projectile.ai[1], 0, 40);
                    move = Main.MouseWorld + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[0])) * (Projectile.ai[1] + 10);
                    Projectile.Move(move, 16, 20);

                    Projectile.localAI[1]++;
                }
                else if (Projectile.localAI[0] == 0)
                    Projectile.Kill();
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 4.4f;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);
    }
    public class SoulScepterChargeS : SoulScepterCharge
    {
        public override string Texture => "Redemption/NPCs/Bosses/Keeper/KeeperSoulCharge";

        public override bool PreAI()
        {
            if (Main.myPlayer == Projectile.owner && Projectile.localAI[0] == 0)
            {
                Player player = Main.player[Projectile.owner];
                if (player.channel)
                {
                    if (Projectile.localAI[1] == 0)
                        SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                    if (Projectile.localAI[1] == 120)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center, Vector2.One, ModContent.ProjectileType<SoulScepterCharge>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0] + 90, Projectile.ai[1]);
                    }
                    if (Projectile.localAI[1] == 240)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center, Vector2.One, ModContent.ProjectileType<SoulScepterCharge>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0] + 180, Projectile.ai[1]);
                    }
                    if (Projectile.localAI[1] == 360)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.Center, Vector2.One, ModContent.ProjectileType<SoulScepterCharge>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0] + 270, Projectile.ai[1]);
                    }
                }
            }
            return true;
        }
    }
}