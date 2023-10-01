using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Redemption.Effects;
using System;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolySpear_Tele : ModProjectile, IDrawAdditive
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 80;
            Projectile.alpha = 255;
        }
        public float LaserLength = 0;
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft >= 70)
            {
                Projectile.alpha -= 8;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 20, 255);
            }
            else if (Projectile.timeLeft <= 30)
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;

                Projectile.alpha += 10;
            }
            EndpointTileCollision();
        }
        public virtual void EndpointTileCollision()
        {
            for (LaserLength = 0; LaserLength < 1000; LaserLength += 16)
            {
                Vector2 start = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength;
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, start, 1, 1))
                {
                    LaserLength -= 16;
                    break;
                }
            }
        }
        private float BeamSize;
        private float BeamStrength;
        public void AdditiveCall(SpriteBatch sB, Vector2 screenPos)
        {
            int maxSize = 20;
            BeamSize = Math.Min(BeamSize + 1, maxSize);
            BeamStrength = BeamSize / maxSize;
            DrawTether(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (LaserLength + 100), screenPos, Color.LightGoldenrodYellow, Color.Yellow, BeamSize, BeamStrength * Projectile.Opacity);
        }
        public void DrawTether(Vector2 End, Vector2 screenPos, Color color1, Color color2, float Size, float Strength)
        {
            Effect effect = ModContent.Request<Effect>("Redemption/Effects/Beam", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly / 3);
            effect.Parameters["uColor"].SetValue(color1.ToVector4());
            effect.Parameters["uSecondaryColor"].SetValue(color2.ToVector4());

            Vector2 dist = End - Projectile.Center;

            TrianglePrimitive tri = new()
            {
                TipPosition = Projectile.Center - screenPos,
                Rotation = dist.ToRotation(),
                Height = Size + 20 + dist.Length() * 1.5f,
                Color = Color.White * Strength,
                Width = Size + 10
            };

            PrimitiveRenderer.DrawPrimitiveShape(tri, effect);
        }
    }
}