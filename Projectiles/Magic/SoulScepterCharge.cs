using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Base;

namespace Redemption.Projectiles.Magic
{
    public class SoulScepterCharge : ModProjectile, ITrailProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Keeper/KeeperSoulCharge";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Charge");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 200;
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
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, Projectile.Opacity, Projectile.Opacity);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            Player player = Main.player[Projectile.owner];

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Main.myPlayer == Projectile.owner && Projectile.localAI[0] == 0)
            {
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
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Scale: 2f);
                Main.dust[dustIndex].velocity *= 0.6f;
                Main.dust[dustIndex].noGravity = true;
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
                    int mana = player.inventory[player.selectedItem].mana;
                    if (Projectile.localAI[1] == 0)
                        SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                    if (Projectile.localAI[1] == 120)
                    {
                        if (BasePlayer.ReduceMana(player, (int)(mana * 1.5f)))
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.One, ModContent.ProjectileType<SoulScepterCharge>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0] + 90, Projectile.ai[1]);
                        }
                    }
                    if (Projectile.localAI[1] == 240)
                    {
                        if (BasePlayer.ReduceMana(player, (int)(mana * 2f)))
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.One, ModContent.ProjectileType<SoulScepterCharge>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0] + 180, Projectile.ai[1]);
                        }
                    }
                    if (Projectile.localAI[1] == 360)
                    {
                        if (BasePlayer.ReduceMana(player, (int)(mana * 2.5f)))
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.One, ModContent.ProjectileType<SoulScepterCharge>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0] + 270, Projectile.ai[1]);
                        }
                    }
                }
            }
            return true;
        }
    }
}