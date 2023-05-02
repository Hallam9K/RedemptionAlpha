using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    public class Chickadier_Bomb : Rooster_EggBomb
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Egg Bomb");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = true;
            Projectile.hide = false;
            Projectile.width = 12;
            Projectile.height = 12;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {}

        public override bool PreAI()
        {
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            Projectile.velocity.Y += 0.2f;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/FowlEmperor/Chickadier_Bomb").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 + 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}