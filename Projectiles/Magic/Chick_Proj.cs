using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Chick_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chick");
        }
        public override void SetDefaults()
        {
            AIType = ProjectileID.BabySpider;
            Projectile.aiStyle = ProjAIStyleID.BabySpider;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
        }
        private bool rotRight;
        public override bool PreAI()
        {
            if (rotRight)
                Projectile.rotation += Projectile.velocity.Y / 40;
            else
                Projectile.rotation -= Projectile.velocity.Y / 40;
            Projectile.velocity.Y += 0.2f;
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .5f, Pitch = 0.5f }, Projectile.position);
            for (int i = 0; i < 12; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f);
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Vector2 drawOrigin = new(glow.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                rotRight = !rotRight;
                Projectile.velocity.Y = -Main.rand.NextFloat(3, 6);
            }
            return false;
        }
    }
}