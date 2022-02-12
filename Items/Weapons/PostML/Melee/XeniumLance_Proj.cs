using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using ReLogic.Content;
using Redemption.BaseExtension;
using Redemption.Effects.PrimitiveTrails;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class XeniumLance_Proj : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenium Lance");
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Length = 40;
            Rot = MathHelper.ToRadians(3);
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.Redemption().Unparryable = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(136, 255, 255), new Color(105, 255, 255)), new NoCap(), new DefaultTrailPosition(), 150f, 250f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_1", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        private Vector2 vector;
        private Vector2 startVector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public int Timer;
        private float speed;
        private int directionLock;


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

            switch (Projectile.ai[0])
            {
                case 0:
                    player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                        speed = MathHelper.ToRadians(6);
                    }
                    if (Timer < 10)
                    {
                        Length *= 1.1f;
                        Rot += speed * Projectile.spriteDirection;
                        speed *= 1.2f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    else
                    {
                        Length *= 0.98f;
                        Rot += speed * Projectile.spriteDirection;
                        speed *= 0.8f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    if (Timer >= 22)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 120);
                    break;

                case 1:
                    player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() + (MathHelper.PiOver2 * Projectile.spriteDirection));
                        speed = MathHelper.ToRadians(6);
                    }
                    if (Timer < 10)
                    {
                        Length *= 1.1f;
                        Rot -= speed * Projectile.spriteDirection;
                        speed *= 1.2f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    else
                    {
                        Length *= 0.98f;
                        Rot -= speed * Projectile.spriteDirection;
                        speed *= 0.8f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    if (Timer >= 22)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 120);
                    break;
                case 2:
                    player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                        speed = MathHelper.ToRadians(6);
                    }
                    if (Timer < 10)
                    {
                        Length *= 1.1f;
                        Rot += speed * Projectile.spriteDirection;
                        speed *= 1.27f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    else
                    {
                        Length *= 0.98f;
                        Rot += speed * Projectile.spriteDirection;
                        speed *= 0.85f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    if (Timer >= 26)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 120);
                    break;
                case 3:
                    directionLock = player.direction;
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation());
                        speed = 1.2f;
                    }

                    if (Timer == 5)
                    {
                        player.velocity = RedeHelper.PolarVector(35, Projectile.velocity.ToRotation());
                    }

                    if (Timer >= 10)
                    {
                        player.immune = true;
                        player.immuneTime = 60;
                        Projectile.damage += Timer * 100;
                    }
                    Length *= speed;
                    vector = startVector * Length;
                    speed -= 0.015f;
                    if (Timer >= 26)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 120);
                    break;
            }
            Projectile.Center = player.MountedCenter + vector;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 v = Projectile.Center - RedeHelper.PolarVector(48, (Projectile.Center - player.Center).ToRotation());

            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}