using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Redemption.BaseExtension;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Buffs.Debuffs;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Terraria.Audio;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class InfectiousGlaive_Proj : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Infectious Glaive");
            ElementID.ProjPoison[Type] = true;
        }
        private Vector2 startVector;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.alpha = 255;
            Length = 40;
            Rot = MathHelper.ToRadians(3);
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(187, 241, 96), new Color(0, 98, 94)), new RoundCap(), new DefaultTrailPosition(), 100f, 250f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
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

            player.SetCompositeArmFront(true, Length >= 60 ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            switch (Projectile.ai[0])
            {
                case 0:
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                        speed = MathHelper.ToRadians(3);
                    }
                    if (Timer > 8 && Timer < 14 && Main.myPlayer == Projectile.owner)
                    {
                        SoundEngine.PlaySound(SoundID.Item42 with { Volume = 0.3f }, Projectile.position);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(12, vector.ToRotation()), ModContent.ProjectileType<InfectiousGlaive_Shard>(), (int)(Projectile.damage * .5f), Projectile.knockBack, Projectile.owner);
                    }
                    if (Timer < 10)
                    {
                        Length *= 1.2f;
                        Rot += speed * Projectile.spriteDirection;
                        speed *= 1.22f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    else
                    {
                        Length *= 0.97f;
                        Rot += speed * Projectile.spriteDirection;
                        speed *= 0.8f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    if (Timer >= 16)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 40, 130);
                    break;
                case 1:
                    if (Timer++ == 0)
                    {
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() + (MathHelper.PiOver2 * Projectile.spriteDirection));
                        speed = MathHelper.ToRadians(3);
                    }
                    if (Timer < 10)
                    {
                        Length *= 1.2f;
                        Rot -= speed * Projectile.spriteDirection;
                        speed *= 1.22f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    else
                    {
                        Length *= 0.97f;
                        Rot -= speed * Projectile.spriteDirection;
                        speed *= 0.8f;
                        vector = startVector.RotatedBy(Rot) * Length;
                    }
                    if (Timer >= 16)
                        Projectile.Kill();

                    Length = MathHelper.Clamp(Length, 40, 130);
                    break;
            }
            if (Timer > 1)
                Projectile.alpha = 0;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 300);
            else if (Main.rand.NextBool(6))
                target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 150);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 v = Projectile.Center - RedeHelper.PolarVector(64, (Projectile.Center - player.Center).ToRotation());
            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}
