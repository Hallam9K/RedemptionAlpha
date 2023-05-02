using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    public class FowlEmperor_Crown_Proj : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Minibosses/FowlEmperor/FowlEmperor_Crown";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fowl Crown");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<FowlEmperor>() || npc.ai[2] > 4)
                Projectile.Kill();
            if (npc.ai[2] == 4 && npc.Hitbox.Intersects(Projectile.Hitbox))
            {
                npc.ai[1] = 120;
                npc.netUpdate = true;
                Projectile.active = false;
            }
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            Projectile.velocity.Y += 0.2f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 + 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            Projectile.velocity.Y *= 0.5f;
            Projectile.velocity.X *= 0.8f;
            return false;
        }
    }
}