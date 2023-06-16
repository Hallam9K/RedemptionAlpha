using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class Bible_SeedSpear : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Erhan/HolySpear_Proj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Spear");
            ElementID.ProjHoly[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 84;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.rotation = Main.rand.NextFloat(-0.3f, 0.3f);
            Projectile.scale = 0.01f;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override void AI()
        {
            Projectile.width = (int)(18 * Projectile.scale);
            Projectile.height = (int)(84 * Projectile.scale);
            if (Projectile.timeLeft <= 40)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.scale += 0.2f;
            }
            Projectile.scale = MathHelper.Min(Projectile.ai[0], Projectile.scale);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 + 38);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(new Color(255, 255, 255, 50)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
