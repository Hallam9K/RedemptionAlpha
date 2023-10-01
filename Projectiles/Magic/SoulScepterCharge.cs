using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
using Redemption.Effects;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Redemption.Projectiles.Magic
{
    public class SoulScepterCharge : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Keeper/KeeperSoulCharge";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soul Charge");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ElementID.ProjArcane[Type] = true;
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

        private readonly int NUMPOINTS = 70;
        public Color baseColor = Color.GhostWhite;
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 5f;

        private Vector2 move;
        public override void AI()
        {
            baseColor = Color.GhostWhite * .5f;
            Projectile.timeLeft = 10;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * .4f, Projectile.Opacity * .4f, Projectile.Opacity * .4f);

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
                    FakeKill();
            }
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, baseColor, baseColor, thickness);
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, Scale: 4f);
                    Main.dust[dustIndex].velocity *= 0.6f;
                    Main.dust[dustIndex].noGravity = true;
                }
            }
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer >= 120)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            return true;
        }
        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, Scale: 4f);
                Main.dust[dustIndex].velocity *= 0.6f;
                Main.dust[dustIndex].noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0) * Projectile.Opacity;
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
                    if (Projectile.localAI[1] % 10 == 0 && !BasePlayer.ReduceMana(player, 3))
                        player.channel = false;

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