using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Textures
{
    public class Ring_Visual : ModProjectile
    {
        public override string Texture => "Redemption/Textures/Ring1";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ring");
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 20;
            Projectile.scale = 0.1f;
        }

        public ref float FlatScale => ref Projectile.ai[0];
        public ref float MultiScale => ref Projectile.ai[1];

        public Color Color;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Color.PackedValue);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Color.PackedValue = reader.ReadUInt32();
        }

        public override void AI()
        {
            Projectile.scale += FlatScale; // 0.13f
            Projectile.scale *= MultiScale; // 0.9f
            if (Projectile.timeLeft < 10)
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.timeLeft / 10f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
