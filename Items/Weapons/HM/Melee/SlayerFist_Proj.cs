using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.BaseExtension;
using Terraria.Audio;
using System.Collections.Generic;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class SlayerFist_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slayer's Rocket Fist");
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Length = 12;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        public bool rotRight;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.Center = player.MountedCenter + vector;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver2;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver2;

            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Timer++ == 0)
                        {
                            player.velocity = RedeHelper.PolarVector(15, (Main.MouseWorld - player.Center).ToRotation());
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation());
                        }
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        vector = startVector * Length;

                        if (Timer < 16)
                        {
                            player.Redemption().contactImmune = true;

                            Vector2 position = player.Center + (Vector2.Normalize(player.velocity) * 30f);
                            Dust dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.Frost)];
                            dust.position = position;
                            dust.velocity = (player.velocity.RotatedBy(1.57) * 0.33f) + (player.velocity / 4f);
                            dust.position += player.velocity.RotatedBy(1.57);
                            dust.fadeIn = 0.5f;
                            dust.noGravity = true;
                            dust = Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.Frost)];
                            dust.position = position;
                            dust.velocity = (player.velocity.RotatedBy(-1.57) * 0.33f) + (player.velocity / 4f);
                            dust.position += player.velocity.RotatedBy(-1.57);
                            dust.fadeIn = 0.5f;
                            dust.noGravity = true;
                        }
                        if (Timer >= 24)
                            Projectile.Kill();
                        break;
                    case 1:
                        if (Timer++ < 40 && player.velocity.Y != 0)
                        {
                            player.Redemption().contactImmune = true;
                            player.fullRotation += rotRight ? 0.25f : -0.25f;
                            player.fullRotationOrigin = new Vector2(10, 20);
                        }
                        else
                            Projectile.Kill();

                        if (Timer < 20)
                        {
                            player.Redemption().contactImmune = true;
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation());
                            vector = startVector * Length;
                        }
                        if (Timer == 20)
                        {
                            SoundEngine.PlaySound(CustomSounds.MissileFire1, Projectile.Center);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(15, (Main.MouseWorld - player.Center).ToRotation()), ModContent.ProjectileType<KS3_FistF>(), (int)(Projectile.damage * 1.1f), Projectile.knockBack, player.whoAmI);
                        }
                        if (Timer <= 5 && !player.channel)
                            Projectile.Kill();
                        break;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.fullRotation = 0f;
        }
        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] == 0 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
            {
                player.velocity.X = -player.velocity.X / 2;
                player.velocity.Y -= 12;
                player.velocity.Y = MathHelper.Max(player.velocity.Y, -12);
            }
            if (player.channel)
            {
                player.velocity.Y = -12;
                Projectile.alpha = 255;
                if (target.Center.X < player.Center.X)
                    rotRight = true;
                Timer = 0;
                Projectile.ai[0] = 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(2, (Projectile.Center - player.Center).ToRotation());

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}