using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoDancingLights : ModProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteOrb";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dancing Light");
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 254;
            Projectile.timeLeft = 180;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
                Projectile.alpha -= 8;
            else
                Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 60)
                Projectile.alpha += 6;
            if (Projectile.alpha < 100)
                Projectile.localAI[0] = 1;

            Lighting.AddLight(Projectile.Center, Projectile.Opacity, Projectile.Opacity * 0.9f, Projectile.Opacity * 0.6f);
            for (int p = 0; p < 255; p++)
            {
                Player player = Main.player[p];
                if (player.active && !player.dead && Projectile.alpha < 110 && Projectile.Hitbox.Intersects(player.Hitbox))
                    player.AddBuff(ModContent.BuffType<HolyFireDebuff>(), 30);
            }
            if (Projectile.alpha >= 255 && Projectile.localAI[0] != 0)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), new Color(255, 243, 162) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}