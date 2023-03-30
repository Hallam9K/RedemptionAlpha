using Terraria;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
using Terraria.GameContent;

namespace Redemption.Projectiles.Minions
{
    public class Hardlight_Magnet : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/KSIII/KS3_Magnet";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Magnet Drone Mk.I");
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 46;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 800;
        }

        Vector2 vector;
        private int damageStored;
        public override void AI()
        {
            if (Projectile.frameCounter++ >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 11)
                    Projectile.frame = 8;
            }
            Player player = Main.player[Projectile.owner];
            Projectile.LookAtEntity(player);

            float soundVolume = Projectile.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (Projectile.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, Projectile.position);
                Projectile.soundDelay = 10;
            }

            if (++Projectile.ai[1] % 80 == 0)
            {
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 200);
                vector.Y = (float)(Math.Cos(angle) * 200);
                Projectile.ai[1] = 0;
            }
            if (Projectile.localAI[0]++ == 0)
            {
                for (int m = 0; m < 16; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.Frost, 0f, 0f, 100, Color.White, 2f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)16 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
            }
            if (Projectile.localAI[0] < 180)
            {
                for (int k = 0; k < 3; k++)
                {
                    Vector2 vector2;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector2.X = (float)(Math.Sin(angle) * 200);
                    vector2.Y = (float)(Math.Cos(angle) * 200);
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector2, 2, 2, DustID.Frost, 0f, 0f, 100, default, 1f)];
                    dust2.noGravity = true;
                    dust2.velocity *= 0f;
                }
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile target = Main.projectile[i];
                    if (!target.active || target.width >= 40 || target.height >= 40 || Projectile.DistanceSQ(target.Center) >= 200 * 200 || !target.hostile || target.damage <= 0 || target.ProjBlockBlacklist())
                        continue;

                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<Hardlight_MagnetPulse>(), 0, 0, player.whoAmI, Projectile.whoAmI);
                    damageStored += target.damage * 2;
                    target.Kill();
                }
            }
            if (Projectile.localAI[0] >= 180 && Projectile.localAI[0] < 240)
            {
                for (int k = 0; k < 4; k++)
                {
                    Vector2 vector2;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector2.X = (float)(Math.Sin(angle) * 90);
                    vector2.Y = (float)(Math.Cos(angle) * 90);
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector2, 2, 2, DustID.Frost, 0f, 0f, 100, default, 2f)];
                    dust2.noGravity = true;
                    dust2.velocity = -Projectile.DirectionTo(dust2.position) * 8f;
                }
            }
            int getNPC = RedeHelper.GetNearestNPC(Projectile.Center);
            if (Projectile.localAI[0] == 240 && damageStored > 10 && getNPC != -1 && Projectile.owner == Main.myPlayer)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.BallFire, Projectile.position);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(10, (Main.npc[getNPC].Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<Hardlight_MagnetBeam>(), (int)MathHelper.Clamp(damageStored, 10, 800), 4, player.whoAmI, Projectile.whoAmI);
            }
            if (Projectile.localAI[0] >= 400)
            {
                Projectile.velocity.Y -= 0.5f;
                if (Projectile.DistanceSQ(player.Center) > 1500 * 1500)
                    Projectile.active = false;
            }
            else if (Projectile.localAI[0] <= 200)
                Projectile.Move(vector, 11, 15, true);
            else
                Projectile.velocity *= 0.96f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 12;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowMask, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
    public class Hardlight_MagnetPulse : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Surge");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            int DustID2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
            Main.dust[DustID2].noGravity = true;

            Projectile projAim = Main.projectile[(int)Projectile.ai[0]];
            if (!projAim.active)
                Projectile.Kill();

            Projectile.Move(projAim.Center, 12, 1);

            if (Projectile.Hitbox.Intersects(projAim.Hitbox))
                Projectile.Kill();
        }
    }
}