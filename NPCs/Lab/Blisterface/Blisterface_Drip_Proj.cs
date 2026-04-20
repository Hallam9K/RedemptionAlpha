using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.NPCs.Lab.Blisterface
{
    public class Blisterface_Drip_Proj : ModRedeProjectile
    {
        public override string Texture => "Redemption/Gores/Misc/XenoDroplet";
        public Vector2[] oldPos = new Vector2[6];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 15;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
            ElementID.ProjWater[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.hide = true;
            Projectile.frame = 7;
            Projectile.scale = Main.rand.NextFloat(1, 1.4f);
            Projectile.Redemption().friendlyHostile = true;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 6;
            return true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 9)
                    Projectile.frame = 8;
            }
            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.localAI[1] = Projectile.ai[1];

                    Projectile.ai[0] = 1;
                    Projectile.netUpdate = true;
                    break;
                case 1:
                    Projectile.localAI[0]++;
                    Projectile.ai[1] -= 1f;
                    BeamOpacity = MathHelper.Lerp(1, 0, Projectile.ai[1] / Projectile.localAI[1]);

                    if (Projectile.ai[1] <= 0)
                    {
                        SoundEngine.PlaySound(SoundID.Drip with { MaxInstances = 16 }, Projectile.position);

                        Projectile.timeLeft = 300;
                        Projectile.alpha = 0;
                        Projectile.ai[0] = 2;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2:
                    if (BeamOpacity > 0)
                        BeamOpacity -= .05f;
                    if (Projectile.timeLeft < 250)
                        Projectile.tileCollide = true;
                    Projectile.velocity.Y += 0.2f;
                    break;
            }
            for (int k = oldPos.Length - 1; k > 0; k--)
                oldPos[k] = oldPos[k - 1];
            oldPos[0] = Projectile.Center;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<WastelandWaterSplash>(), Scale: 2);
                Main.dust[dustIndex].velocity *= 2f;
            }
            SoundEngine.PlaySound(SoundID.DripSplash with { MaxInstances = 16 }, Projectile.position);

            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<WastelandWaterSplash>(),
                    Projectile.velocity.X * .6f, -Projectile.velocity.Y, Scale: 1);
                Main.dust[d].fadeIn = 300;
            }
        }
        private float BeamSize;
        private float BeamStrength;
        private float BeamOpacity;
        public void DrawTether(Vector2 End, Vector2 screenPos, Color color1, Color color2, float Size, float Strength)
        {
            Effect effect = Request<Effect>("Redemption/Effects/Beam").Value;
            effect.Parameters["uTexture"].SetValue(Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly / 3);
            effect.Parameters["uColor"].SetValue(color1.ToVector4());
            effect.Parameters["uSecondaryColor"].SetValue(color2.ToVector4());

            Vector2 dist = End - Projectile.Center;

            TrianglePrimitive tri = new()
            {
                TipPosition = Projectile.Center - screenPos,
                Rotation = dist.ToRotation(),
                Height = Size + 20 + dist.Length() * 1.5f,
                Color = Color.DarkGreen * Strength,
                Width = Size + 30
            };

            PrimitiveRenderer.DrawPrimitiveShape(tri, effect);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            int maxSize = 10;
            BeamSize = Math.Min(BeamSize + 1, maxSize);
            BeamStrength = BeamSize / maxSize;
            DrawTether(Projectile.Center + Vector2.UnitY * (BeamOpacity * 300), Main.screenPosition, Color.Green, Color.LightGreen, BeamSize, BeamStrength * BeamOpacity);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = texture.Frame(1, 15, 0, Projectile.frame);
            Vector2 drawOrigin = rect.Size() / 2;
            for (int k = oldPos.Length - 1; k >= 0; k -= 1)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                float alpha = 1f - (k + 1) / (float)(oldPos.Length + 2);
                Main.EntitySpriteDraw(texture, drawPos, rect, lightColor * alpha * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale * alpha, 0, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            return false;
        }
    }
}