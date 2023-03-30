using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Redemption.Projectiles.Pets
{
    public class Xenoemia_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenomite Crystal");
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.scale = 0.01f;
            Projectile.hide = true;
            Projectile.frame = Main.rand.Next(4);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.Y = 0;
            return false;
        }
        public override void AI()
        {
            Projectile.velocity.Y = 1;
            if (Projectile.timeLeft <= 40)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.scale += 0.02f;
            }
            Projectile.scale = MathHelper.Min(1, Projectile.scale);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f + 12);

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 4) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
