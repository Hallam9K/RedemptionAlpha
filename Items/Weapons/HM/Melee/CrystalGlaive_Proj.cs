using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using ReLogic.Content;
using Redemption.BaseExtension;
using Redemption.Effects.PrimitiveTrails;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class CrystalGlaive_Proj : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Glaive");
            ElementID.ProjHoly[Type] = true;
        }
        private Vector2 startVector;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.alpha = 255;
            Length = 60;
            Rot = MathHelper.ToRadians(3);
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new RainbowTrail(saturation: 0.4f), new NoCap(), new DefaultTrailPosition(), 80f, 250f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_3", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float speed;
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.Center = player.MountedCenter + vector;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            player.SetCompositeArmFront(true, Length >= 100 ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            switch (Projectile.ai[0])
            {
                case 0:
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                        speed = MathHelper.ToRadians(3);
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
                    if (Timer >= 18)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 120);
                    break;
                case 1:
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() + (MathHelper.PiOver2 * Projectile.spriteDirection));
                        speed = MathHelper.ToRadians(3);
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
                    if (Timer >= 18)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 120);
                    break;
                case 2:
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation());
                        speed = 1.2f;
                    }
                    speed -= 0.02f;
                    Length *= speed;
                    vector = startVector * Length;
                    if (Timer >= 18)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 180);
                    break;
                case 3:
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation());
                        speed = 1.2f;
                    }
                    if (Timer == 5 && Projectile.owner == Main.myPlayer)
                    {
                        SoundEngine.PlaySound(SoundID.Item101, Projectile.position);
                        if (Projectile.ai[1] == 1)
                        {
                            for (int i = 0; i < Main.rand.Next(5, 8); i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(Main.rand.Next(7, 11), Projectile.velocity.ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f)), ProjectileID.CrystalStorm, Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Main.rand.Next(1, 3); i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(Main.rand.Next(7, 11), Projectile.velocity.ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f)), ProjectileID.CrystalStorm, Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                            }
                        }
                    }
                    speed -= 0.02f;
                    Length *= speed;
                    vector = startVector * Length;
                    if (Timer >= 18)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 60, 180);
                    break;
            }
            if (Timer > 1)
                Projectile.alpha = 0;

            Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight)];
            dust.velocity *= 0;
            dust.noGravity = true;
            dust.shader = GameShaders.Armor.GetSecondaryShader(77, Main.player[Projectile.owner]);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 2 && player.Redemption().crystalGlaiveShotCount <= 0)
            {
                if (player.Redemption().crystalGlaiveShotCount <= 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, player.position);
                    DustHelper.DrawCircle(player.Center, DustID.CrystalPulse, 4, 1, 1, 1, 2, nogravity: true);
                }
                player.Redemption().crystalGlaiveLevel = 0;
                player.Redemption().crystalGlaiveShotCount = 5;
            }
            else if (Projectile.ai[0] < 2)
            {
                player.Redemption().crystalGlaiveLevel = (int)Projectile.ai[0] + 1;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 v = Projectile.Center - RedeHelper.PolarVector(64, (Projectile.Center - player.Center).ToRotation());
            if (Projectile.ai[1] == 1)
            {
                int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

                Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(Color.Pink) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale + 0.2f, effects, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}