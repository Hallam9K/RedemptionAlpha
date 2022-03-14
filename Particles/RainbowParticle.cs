using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Particles
{
    public class RainbowParticle : Particle
    {
        int frame;
        int frameTick;
        public override void SetDefaults()
        {
            width = 34;
            height = 34;
            scale = 1f;
            timeLeft = 120;
            oldPos = new Vector2[10];
            oldRot = new float[10];
            SpawnAction = Spawn;
            DeathAction = Death;
        }
        public override void AI()
        {
            for (int i = oldPos.Length - 1; i > 0; i--)
            {
                oldPos[i] = oldPos[i - 1];
            }
            oldPos[0] = position;
            for (int i = oldRot.Length - 1; i > 0; i--)
            {
                oldRot[i] = oldRot[i - 1];
            }
            oldRot[0] = rotation;
            if (ai[0] == 0)
            {
                ai[1] = Main.rand.NextFloat(2f, 8f) / 10f;
                ai[2] = Main.rand.Next(0, 2);
                ai[3] = Main.rand.NextFloat(0f, 360f);
                timeLeft = (int)ai[4] > 0 ? (int)ai[4] : timeLeft;
            }
            ai[0]++;
            rotation += Utils.Clamp(velocity.X * 0.025f, -ai[1], ai[1]);
            velocity *= 0.98f;
            color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), (360f - timeLeft) / 360f);

            if (scale <= 0f)
                active = false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Redemption/Particles/RainbowParticle1").Value;
            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            Color color = new((int)(hsl.R * alpha), (int)(hsl.G * alpha), (int)(hsl.B * alpha), 0);
            spriteBatch.Draw(Request<Texture2D>("Redemption/Particles/RainbowParticle2").Value, position - Main.screenPosition, new Rectangle(0, 0, 142, 42), color, ai[3].InRadians().AngleLerp((ai[3] + 90f).InRadians(), (120f - timeLeft) / 120f), new Vector2(71, 21), 0.75f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Request<Texture2D>("Redemption/Particles/RainbowParticle3").Value, position - Main.screenPosition, new Rectangle(0, 0, 512, 512), color, ai[3].InRadians().AngleLerp((ai[3] + 90f).InRadians(), (120f - timeLeft) / 120f), new Vector2(256, 256), 0.25f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, position - screenPos, tex.AnimationFrame(ref frame, ref frameTick, 4, 7, true), color, 0f, new Vector2(width / 2, height / 2), scale + 0.5f, SpriteEffects.None, 0f);
            return false;
        }
        public void Spawn()
        {

        }
        public void Death()
        {

        }
    }
}
