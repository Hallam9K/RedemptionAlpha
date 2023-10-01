using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolySpear_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 8;
            return true;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (Projectile.timeLeft <= 540)
                    Projectile.velocity *= 1.05f;
            }
            else
            {
                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Projectile.localAI[0] == 0)
            {
                Projectile.position += Projectile.velocity * (Projectile.ai[0] == 1 ? 2000 : 1200);
                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120));
                RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.13f, 0.83f, 0);
                Projectile.alpha = 0;
                Projectile.localAI[0] = 1;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.localAI[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = .5f }, Projectile.position);
                Projectile.position += oldVelocity * 2;
                Projectile.localAI[1] = 1;
                for (int i = 0; i < 20; i++)
                {
                    int num5 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X * 0.5f,
                        Projectile.velocity.Y * 0.5f);
                    Color dustColor = new(255, 255, 209) { A = 0 };
                    Main.dust[num5].fadeIn = 0.1f;
                    Main.dust[num5].noGravity = true;
                    Main.dust[num5].velocity.Y = -7;
                    Main.dust[num5].color = dustColor * Projectile.Opacity;

                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f,
                        Projectile.velocity.Y * 0.5f, Scale: 3);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y = -7;
                }
            }
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = new Color(255, 255, 120, 0) * 0.5f * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale + 0.2f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(new Color(255, 255, 255, 50)), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
