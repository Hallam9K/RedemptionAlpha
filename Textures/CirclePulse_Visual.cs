using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Textures
{
    public class CirclePulse_Visual : ModProjectile
    {
        public override string Texture => "Redemption/Textures/DreamsongLight_Visual";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pulse");
        }

        public Entity EntityTarget;

        public ref float Scale => ref Projectile.ai[0];
        public Color Color;
 
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Color.PackedValue);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Color.PackedValue = reader.ReadUInt32();
        }
 
        public override void SetDefaults()
        {
            Projectile.width = 600;
            Projectile.height = 600;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            // This is only valid on the owner, requires sync
            if (EntityTarget != null)
            {
                if (EntityTarget.active)
                {
                    Projectile.Center = EntityTarget.Center;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.timeLeft = 10;
            Projectile.velocity *= 0;
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 60)
            {
                if (Projectile.localAI[0] < 30)
                    Projectile.alpha -= 5;
                else
                    Projectile.alpha += 5;
                Projectile.scale += 0.003f;
            }
            else
            {
                Projectile.alpha = 255;
                Projectile.scale = 1;
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color), Projectile.rotation, origin, Projectile.scale * Scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}