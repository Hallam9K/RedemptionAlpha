using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.BaseExtension;
using Redemption.Projectiles.Melee;
using Redemption.Buffs.Debuffs;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class DaggerOfOathkeeper_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dagger of the Oathkeeper");
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Length = 28;
            Rot = MathHelper.ToRadians(3);
        }
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float speed;
        private float SwingSpeed;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item1, player.position);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                            speed = MathHelper.ToRadians(3);
                        }
                        if (Timer < 5 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.15f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 13 * SwingSpeed)
                        {
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ < 5 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.15f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.5f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 15 * SwingSpeed)
                        {
                            Projectile.Kill();
                            if (player.channel)
                                Projectile.Shoot(player.Center, ModContent.ProjectileType<DaggerOfOathkeeper_Proj2>(), Projectile.damage * 4, Projectile.velocity, false, SoundID.Item1.WithVolume(0));
                        }
                        break;
                }
            }
            if (Timer > 1)
                Projectile.alpha = 0;

            Projectile.Center = player.MountedCenter + vector;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.ai[0] < 3)
                RedeProjectile.Decapitation(target, ref damage, ref crit);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(0, (Projectile.Center - player.Center).ToRotation());

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
    public class DaggerOfOathkeeper_Proj2 : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Melee/DaggerOfOathkeeper_Proj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dagger of the Oathkeeper");
        }
        private Vector2 startVector;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.alpha = 255;
            Projectile.scale = 1;
            Projectile.Redemption().Unparryable = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }

        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public float Rot;
        public int HitCount;
        public ref float Timer => ref Projectile.localAI[1];
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            player.SetCompositeArmFront(true, Length >= 30 ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            if (Timer++ == 0 && Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation());
                Rot = MathHelper.ToRadians(Main.rand.NextFloat(-10, 10));
                Length = 10;
                player.direction = Main.MouseWorld.X > player.Center.X ? 1 : -1;
            }

            if (Timer == 3)
                Projectile.Shoot(Projectile.Center, ModContent.ProjectileType<ShadeStab_Proj>(), Projectile.damage * 2, RedeHelper.PolarVector(26, vector.ToRotation()), false, SoundID.Item1.WithVolume(0), "", Main.rand.Next(2));

            if (Timer < 4)
                Length *= 1.7f;
            else
                Length *= 0.7f;

            vector = startVector.RotatedBy(Rot) * Length;
            if (Timer >= 7)
            {
                if (player.channel)
                    Timer = 0;
                else
                    Projectile.Kill();
            }

            Length = MathHelper.Clamp(Length, 6, 40);

            if (Timer > 1)
                Projectile.alpha = 0;

            Projectile.Center = player.MountedCenter + vector;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 v = Projectile.Center - RedeHelper.PolarVector(0, (Projectile.Center - player.Center).ToRotation());

            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}