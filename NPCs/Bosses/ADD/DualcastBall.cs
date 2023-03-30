using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class DualcastBall : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Static Dualcast");
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 255, 255), new Color(241, 215, 108)), new RoundCap(), new ZigZagTrailPosition(6), 80f, 250f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.1f, 1f, 1f));
        }

        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 1f, Projectile.Opacity * 0.8f, Projectile.Opacity * 0.6f);

            if (originalVelocity == Vector2.Zero)
                originalVelocity = Projectile.velocity;
            if (Projectile.ai[0] == 0)
            {
                if (offsetLeft)
                {
                    vectorOffset -= 0.06f;
                    if (vectorOffset <= -2.4f)
                    {
                        vectorOffset = -2.4f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset += 0.06f;
                    if (vectorOffset >= 2.4f)
                    {
                        vectorOffset = 2.4f;
                        offsetLeft = true;
                    }
                }
            }
            else
            {
                if (offsetLeft)
                {
                    vectorOffset += 0.06f;
                    if (vectorOffset >= 2.4f)
                    {
                        vectorOffset = 2.4f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset -= 0.06f;
                    if (vectorOffset <= -2.4f)
                    {
                        vectorOffset = -2.4f;
                        offsetLeft = true;
                    }
                }
            }
            float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
            Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));

            Projectile.rotation = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + Projectile.velocity) + 1.57f - MathHelper.PiOver4;
            Projectile.spriteDirection = 1;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
            }
            target.AddBuff(ModContent.BuffType<StaticStunDebuff>(), 60);
        }
    }
}