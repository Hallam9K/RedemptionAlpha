using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class MythrilsBane_Proj : TrueMeleeProjectile
    {
        public float[] oldrot = new float[6];
        public override string Texture => "Redemption/Items/Weapons/PostML/Melee/MythrilsBane";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mythril's Bane");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 84;
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
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit, 50);

            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;

            if (NPCLists.Armed.Contains(target.type))
                target.AddBuff(ModContent.BuffType<DisarmedDebuff>(), 1800);
            target.AddBuff(ModContent.BuffType<BrokenArmorDebuff>(), 600);
        }
        private Vector2 startVector;
        private Vector2 vector;
        private Vector2 mouseOrig;
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
                        if (Timer == 0)
                        {
                            mouseOrig = Main.MouseWorld;
                            SoundEngine.PlaySound(SoundID.Item71, player.position);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                            speed = MathHelper.ToRadians(Main.rand.Next(6, 15));
                        }
                        if (Timer++ == (int)(5 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(Main.rand.Next(45, 66), (mouseOrig - player.Center).ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<MythrilsBaneSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        if (Timer < 5 * SwingSpeed)
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
                        if (Timer >= Main.rand.Next(28, 33) * SwingSpeed)
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
                            mouseOrig = Main.MouseWorld;
                            Projectile.alpha = 255;
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == (int)(5 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(Main.rand.Next(45, 66), (mouseOrig - player.Center).ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<MythrilsBaneSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                        }
                        if (Timer < 5 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.23f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= Main.rand.Next(28, 33) * SwingSpeed)
                            Projectile.Kill();
                        break;
                }
            }
            if (Timer < 8)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile target = Main.projectile[i];
                    if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile)
                        continue;

                    if (target.damage > 100 / 4 || Projectile.alpha > 0 || target.width + target.height > Projectile.width + Projectile.height)
                        continue;

                    if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || target.alpha > 0 || target.ProjBlockBlacklist(true))
                        continue;

                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
                    DustHelper.DrawCircle(target.Center, DustID.SilverCoin, 1, 4, 4, nogravity: true);
                    target.Kill();
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
            return Timer <= 8 && Projectile.ai[0] < 2 ? null : false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(20, (Projectile.Center - player.Center).ToRotation());

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos - v, null, color * (Timer / 10) * 0.5f, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}