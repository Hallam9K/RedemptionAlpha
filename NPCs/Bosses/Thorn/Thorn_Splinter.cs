using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class Thorn_Splinter : ModProjectile
    {
        public Vector2[] oldPos = new Vector2[5];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            ElementID.ProjNature[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.4f, Projectile.Opacity * 0.3f, Projectile.Opacity * 0.2f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.timeLeft < 120)
                Projectile.velocity.Y += .01f;

            for (int k = oldPos.Length - 1; k > 0; k--)
                oldPos[k] = oldPos[k - 1];
            oldPos[0] = Projectile.Center;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            int chance = Main.expertMode ? 2 : 4;
            if (Main.rand.NextBool(chance))
                target.AddBuff(BuffID.Poisoned, 80);
            Projectile.timeLeft = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = Main.expertMode ? 2 : 4;
            if (Main.rand.NextBool(chance))
                target.AddBuff(BuffID.Poisoned, 80);
            Projectile.timeLeft = 2;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.JungleGrass);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                float oldScale = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;
                Color color = Projectile.GetAlpha(Color.Orange with { A = 0 }) * oldScale;
                Main.EntitySpriteDraw(texture.Value, drawPos, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale * oldScale, 0, 0);
            }
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
    }
}