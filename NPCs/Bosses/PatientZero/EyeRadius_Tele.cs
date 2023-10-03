using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class EyeRadius_Tele : ModProjectile
    {
        public override string Texture => "Redemption/Textures/RadialTelegraph2";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph");
        }
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 25;
            Projectile.scale = 0.1f;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale += 0.2f;
            Projectile.scale *= 0.9f;
            if (Projectile.localAI[0] < 10)
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.timeLeft / 10f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.Green), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
    public class EyeRadius_Tele2 : ModProjectile
    {
        public override string Texture => "Redemption/Textures/RadialTelegraph2";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph");
        }
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.1f;
            Projectile.localAI[0] = 20;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.ai[0] != 1)
                Projectile.Kill();
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.localAI[1]++ <= (Main.getGoodWorld ? 20 : 40) && Projectile.localAI[1] % 8 == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, Projectile.velocity, ModContent.ProjectileType<EyeRadius_Tele>(), 0, 0, Main.myPlayer);
            }
            if (Projectile.localAI[0]-- >= 0)
            {
                Projectile.scale += 0.2f;
                Projectile.scale *= 0.9f;
                if (Projectile.localAI[0] < 10)
                    Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.localAI[0] / 10f);
            }
            if (npc.ai[1] >= 230 && npc.ai[1] % 3 == 0 && npc.ai[1] <= 380 && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.Item14 with { Volume = .5f }, npc.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, RedeHelper.PolarVector(13, Projectile.rotation + Main.rand.NextFloat(-0.3f, 0.3f)), ModContent.ProjectileType<PZ_Miniblast>(), (int)(npc.damage * 0.85f) / 3, 3, Main.myPlayer);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.Green), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}