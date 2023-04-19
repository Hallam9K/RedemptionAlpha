using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.BaseExtension;
using Redemption.Base;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara_Sword : TrueMeleeProjectile
    {
        public float[] oldrot = new float[6];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukonvasara");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 92;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Length = 80;
            Rot = MathHelper.ToRadians(3);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            Projectile.localNPCImmunity[target.whoAmI] = 8;
            target.immune[Projectile.owner] = 0;
        }
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float speed;
        private float SwingSpeed;
        private Vector2 mouseOrig;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.Center = player.MountedCenter + vector;

            Projectile.spriteDirection = player.direction;
            Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver2;

            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 0)
                        {
                            mouseOrig = Main.MouseWorld;
                            SoundEngine.PlaySound(SoundID.Item71, player.position);
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash, player.position);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                            speed = MathHelper.ToRadians(Main.rand.Next(2, 4));
                        }
                        if (Timer == (int)(4 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(17, (mouseOrig - player.Center).ToRotation()),
                                ModContent.ProjectileType<UkonvasaraSword_Wave>(), 0, 0, Projectile.owner);
                        }
                        if (Timer < 5 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.1f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= Main.rand.Next(29, 32) * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (Main.MouseWorld.X < player.Center.X)
                                player.direction = -1;
                            else
                                player.direction = 1;
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() - ((MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.spriteDirection));
                            speed = MathHelper.ToRadians(Main.rand.Next(2, 4));
                            Projectile.alpha = 255;
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash, player.position);
                            mouseOrig = Main.MouseWorld;
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer == (int)(4 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(17, (mouseOrig - player.Center).ToRotation()),
                                ModContent.ProjectileType<UkonvasaraSword_Wave>(), 0, 0, Projectile.owner, 1);
                        }
                        if (Timer++ < 5 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.1f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= Main.rand.Next(29, 32) * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (Main.MouseWorld.X < player.Center.X)
                                player.direction = -1;
                            else
                                player.direction = 1;
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                            speed = MathHelper.ToRadians(Main.rand.Next(2, 4));
                            Projectile.alpha = 255;
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash, player.position);
                            mouseOrig = Main.MouseWorld;
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer == (int)(3 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(17, (mouseOrig - player.Center).ToRotation()),
                                ModContent.ProjectileType<UkonvasaraSword_Wave>(), 0, 0, Projectile.owner);
                        }
                        if (Timer++ < 4 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.14f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= Main.rand.Next(40, 45) * SwingSpeed)
                        {
                            if (player.channel)
                            {
                                if (Main.MouseWorld.X < player.Center.X)
                                    player.direction = -1;
                                else
                                    player.direction = 1;

                                SoundEngine.PlaySound(CustomSounds.ElectricSlash2, player.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Main.MouseWorld.DirectionFrom(player.Center) * 30, ModContent.ProjectileType<Ukonvasara_Sword_Proj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                            }
                            Projectile.Kill();
                        }
                        break;
                }
            }
            if (Timer > 1)
                Projectile.alpha = 0;
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer <= 8 || (Timer <= 14 && Projectile.ai[0] == 2) ? null : false;
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(30, (Projectile.Center - player.Center).ToRotation());

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(28, 8 + Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos - v, null, color * (Timer / 10) * 0.5f, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(Color.LightYellow), Projectile.rotation, origin, Projectile.scale, spriteEffects);
            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
    public class Ukonvasara_Sword_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Melee/Ukonvasara_Sword";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukonvasara");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public float[] oldrot = new float[8];
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.aiStyle = 3;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        private bool boomed;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 7;
            target.immune[Projectile.owner] = 0;

            Boom();
        }
        public override void AI()
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            if (Projectile.localAI[1]++ > 10)
                Projectile.tileCollide = true;
            if (boomed)
                Projectile.width = Projectile.height = 36;
            Player p = Main.player[Projectile.owner];
            p.itemTime = 2;
            p.itemAnimation = 2;
            BaseAI.AIBoomerang(Projectile, ref Projectile.ai, p.position, p.width, p.height, true, 40f, 300, 1f, 0.6f, false);
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            if (boomed)
                texture = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PostML/Melee/Ukonvasara").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(28, 8 + Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * 0.5f, oldrot[k], drawOrigin, Projectile.scale, 0, 0);
            }

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(Color.LightYellow), Projectile.rotation, drawOrigin, Projectile.scale, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void Boom()
        {
            if (boomed)
                return;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 4;

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.rand.Next(5, 11); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.SpreadUp(13), ModContent.ProjectileType<Ukonvasara_Fragments>(), Projectile.damage, 1, Main.myPlayer);
                }
            }
            SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = .4f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.position);
            for (int i = 0; i < 20; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone,
                    -Projectile.velocity.X * 0.3f, -Projectile.velocity.Y * 0.3f);

            boomed = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.15f;
        }
        public override bool OnTileCollide(Vector2 velocityChange)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Boom();
            BaseAI.TileCollideBoomerang(Projectile, ref velocityChange, true);
            return false;
        }

    }
}