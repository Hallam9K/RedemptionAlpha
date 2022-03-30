using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class ShadeTreble_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Song of the Abyss");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 50;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 20;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.Redemption().Unparryable = true;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Projectile.localAI[0] == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            int degrees = 0;
                            for (int i = 0; i < 2; i++)
                            {
                                degrees += 180;
                                int p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SongDust>(), Projectile.damage, 0, Main.myPlayer);
                                Main.projectile[p].ai[0] = Projectile.whoAmI;
                                Main.projectile[p].ai[1] = degrees;
                            }
                            break;
                        case 1:
                            int degrees2 = 0;
                            for (int i = 0; i < 4; i++)
                            {
                                degrees2 += 90;
                                int p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SongDust>(), Projectile.damage, 0, Main.myPlayer);
                                Main.projectile[p].ai[0] = Projectile.whoAmI;
                                Main.projectile[p].ai[1] = degrees2;
                            }
                            break;
                        case 2:
                            int degrees3 = 0;
                            for (int i = 0; i < 6; i++)
                            {
                                degrees3 += 60;
                                int p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SongDust>(), Projectile.damage, 0, Main.myPlayer);
                                Main.projectile[p].ai[0] = Projectile.whoAmI;
                                Main.projectile[p].ai[1] = degrees3;
                            }
                            break;
                    }
                }
                Projectile.localAI[0] = 1;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.GhostWhite, Color.Black, Color.GhostWhite, Color.DarkSlateBlue, Color.GhostWhite, Color.Indigo, Color.GhostWhite);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Scale: 2f);
                Main.dust[dustIndex].noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            switch (Projectile.ai[0])
            {
                case 0:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < 4; i++)
                            Projectile.Shoot(Projectile.Center, ModContent.ProjectileType<ShadeNote_Proj>(), Projectile.damage / 2, RedeHelper.PolarVector(14, MathHelper.PiOver2 * i), false, SoundID.Item1.WithVolume(0), "", target.whoAmI);
                    }
                    break;
                case 1:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < 4; i++)
                            Projectile.Shoot(Projectile.Center, ModContent.ProjectileType<ShadeNote_Proj>(), Projectile.damage / 2, RedeHelper.PolarVector(14, (MathHelper.PiOver2 * i) + MathHelper.PiOver4), false, SoundID.Item1.WithVolume(0), "", target.whoAmI, 1);
                    }
                    break;
            }
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
        }
    }
    public class SongDust : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Song of the Abyss");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.Redemption().Unparryable = true;
        }

        readonly double dist = 30;
        public override void AI()
        {
            Projectile host = Main.projectile[(int)Projectile.ai[0]];
            if (!host.active || host.type != ModContent.ProjectileType<ShadeTreble_Proj>())
                Projectile.Kill();

            Dust dust = Dust.NewDustDirect(Projectile.position, 2, 2, DustID.AncientLight, 0, 0, 100);
            dust.velocity *= 0f;
            dust.noGravity = true;
            double deg = Projectile.ai[1];
            double rad = deg * (Math.PI / 180);
            Projectile.position.X = host.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = host.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            Projectile.ai[1] += 4f;
        }
    }
    public class ShadeNote_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Song of the Abyss");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.Redemption().Unparryable = true;
        }
        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (!target.active)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Projectile.timeLeft > 150)
                Projectile.velocity *= 0.94f;
            else
            {
                Projectile.friendly = true;
                Projectile.Move(target.Center, Projectile.timeLeft > 50 ? 30 : 50, 20);
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[1] == 1)
                return BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.Indigo, Color.GhostWhite, Color.Indigo);

            return BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.GhostWhite, Color.DarkSlateBlue, Color.GhostWhite);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Scale: 2f);
                Main.dust[dustIndex].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            if (Projectile.ai[1] == 1)
                texture = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "2").Value;

            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
        }
    }
}
