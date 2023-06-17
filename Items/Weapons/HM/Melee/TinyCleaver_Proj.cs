using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using ReLogic.Content;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class TinyCleaver_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/TinyCleaver_Proj1";
        private static Asset<Texture2D> chainTexture;
        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/TinyCleaver_Chain");
        }
        public override void Unload()
        {
            chainTexture = null;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tiny Cleaver");
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 108;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Length = 62;
            Rot = MathHelper.ToRadians(2);
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 2 && Timer > 9)
                return false;
            if (Projectile.ai[0] == 1 && Timer > 8)
                return false;
            if (Projectile.ai[0] == 3 && Timer > 8)
                return false;
            if (Projectile.ai[0] == 4 && Timer > 8)
                return false;
            return !target.friendly && Projectile.ai[0] != 0 ? null : false;
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
            Projectile.Center = player.MountedCenter + vector;

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
                        speed = MathHelper.ToRadians(6);
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.4f }, Projectile.position);
                        vector = startVector * Length;
                        Projectile.ai[0] = 1;
                        Projectile.netUpdate = true;
                        break;
                    case 1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ < 2 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.35f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.6f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 18 * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;
                            speed = MathHelper.ToRadians(6);
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() - ((MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.spriteDirection));
                            Projectile.ai[0] = 2;
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.4f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ < 3 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.1f;
                            extendoLength += 2f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            extendoLength += 2f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 15 * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;
                            speed = MathHelper.ToRadians(6);

                            Projectile.ai[0] = 3;
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() - ((MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.spriteDirection));
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.4f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 3:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ < 3 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.2f;
                            extendoLength += 3f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.6f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 3 && Timer < 12)
                            extendoLength -= 2f;

                        if (Timer >= 16 * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;
                            speed = MathHelper.ToRadians(6);

                            Projectile.ai[0] = 4;
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() - ((MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.spriteDirection));
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.5f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 4:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ < 3 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.15f;
                            extendoLength += 5f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 8)
                            extendoLength -= 10f;

                        if (Timer >= 12 * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Projectile.alpha = 255;
                            speed = 10;

                            Projectile.ai[0] = 5;

                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.BallCreate with { Volume = 0.3f, Pitch = 0.1f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 5:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Main.MouseWorld.X < player.Center.X)
                            player.direction = -1;
                        else
                            player.direction = 1;
                        startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation());
                        vector = startVector * Length;

                        if (Timer++ < 12 * SwingSpeed)
                        {
                            extendoLength += speed;
                            speed *= 0.96f;
                        }
                        else
                        {
                            extendoLength -= speed;
                            speed *= 1.1f;
                        }
                        if (Timer >= 24 * SwingSpeed)
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
                            extendoLength = 0;
                            Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - player.Center).ToRotation());
                            Projectile.alpha = 255;
                            speed = MathHelper.ToRadians(6);
                            Rot = MathHelper.ToRadians(2);
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() - ((MathHelper.PiOver2 + 0.6f) * player.direction));
                            vector = startVector * Length;
                            Projectile.ai[0] = 1;
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            SoundEngine.PlaySound(CustomSounds.ElectricSlash with { Volume = 0.3f, Pitch = 0.2f }, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;

                }
            }
            extendoLength = MathHelper.Clamp(extendoLength, 10, 80);
            if (Timer > 1)
                Projectile.alpha = 0;

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy((Projectile.Center - player.Center).ToRotation());
            float point = 0f;
            // Run an AABB versus Line check to look for collisions
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 20,
                Projectile.Center + unit * ((extendoLength - 7) * extendo[7] / 1.5f), 32, ref point))
                return true;
            else
                return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 5)
            {
                modifiers.FinalDamage *= 1.5f;
                if (target.knockBackResist > 0)
                    modifiers.Knockback.Flat += 4;
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            Projectile.localNPCImmunity[target.whoAmI] = 11;
            target.immune[Projectile.owner] = 0;
        }

        public float[] extendo = new float[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        public float extendoLength = 10;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D segMid = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/TinyCleaver_Proj2").Value;
            Texture2D segEnd = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/TinyCleaver_Proj3").Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(40, (Projectile.Center - player.Center).ToRotation());

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            for (int i = 0; i < 8; i++)
            {
                Main.EntitySpriteDraw(i < 7 ? segMid : segEnd, Projectile.Center - v + RedeHelper.PolarVector(extendoLength * extendo[i], (Projectile.Center - player.Center).ToRotation()) - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            return false;
        }
        public override bool PreDrawExtras()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 v = RedeHelper.PolarVector(40, (Projectile.Center - player.Center).ToRotation());
            Vector2 handleCenter = Projectile.Center - v + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 center = Projectile.Center - RedeHelper.PolarVector(30, (Projectile.Center - player.Center).ToRotation()) + RedeHelper.PolarVector(extendoLength * extendo[7], (Projectile.Center - player.Center).ToRotation()) + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 directionToHandle = handleCenter - center;
            float chainRotation = directionToHandle.ToRotation() - MathHelper.PiOver2;
            float distanceToHandle = directionToHandle.Length();

            while (distanceToHandle > 20f && !float.IsNaN(distanceToHandle))
            {
                directionToHandle /= distanceToHandle; //get unit vector
                directionToHandle *= chainTexture.Height(); //multiply by chain link length

                center += directionToHandle; //update draw position
                directionToHandle = handleCenter - center; //update distance
                distanceToHandle = directionToHandle.Length();

                //Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, Projectile.GetAlpha(Color.White), chainRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}