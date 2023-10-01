using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Redemption.Globals;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Buffs.Debuffs;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZ_Miniblast : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
		{
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 160;
            Projectile.alpha = 255;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BileDebuff>(), 120);
        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust1].noGravity = true;
            }
            Projectile.alpha -= 10;
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            Lighting.AddLight(Projectile.Center, 0, Projectile.Opacity * 0.2f, 0);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.localAI[0]++ > 10)
                Projectile.tileCollide = true;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = .2f }, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, Scale: 2);
                Main.dust[dustIndex].velocity *= 2;
            }
        }
    }
}
