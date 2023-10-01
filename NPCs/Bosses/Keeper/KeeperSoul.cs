using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class KeeperSoul : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Keeper");
        }
        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 140;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 400;
        }

        public Vector2 vector;
        public override void AI()
        {
            if (Projectile.ai[0] < 8)
                Projectile.ai[0] = 8;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.ai[0]++;
                if (Projectile.ai[0] > 9)
                    Projectile.ai[0] = 8;
            }

            if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
            {
                spiritOpacity -= .1f;
                RedeSystem.Silence = true;
            }
            else
                spiritOpacity += .1f;
            spiritOpacity = MathHelper.Clamp(spiritOpacity, 0, 1);
            Player player = Main.player[Projectile.owner];
            if (Projectile.timeLeft < 180)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 3;
                Projectile.rotation += .02f;
                Projectile.scale = MathHelper.Min(Projectile.scale, 1);
                Projectile.alpha = (int)MathHelper.Max(Projectile.alpha, 0);
                if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        vector.X = (float)(Math.Sin(angle) * 100);
                        vector.Y = (float)(Math.Cos(angle) * 100);
                        Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, ModContent.DustType<VoidFlame>(), 0f, 0f, 100, default, 3f)];
                        dust2.noGravity = true;
                        dust2.velocity = -Projectile.DirectionTo(dust2.position) * 10f;
                    }
                }
                for (int k = 0; k < 2; k++)
                {
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector.X = (float)(Math.Sin(angle) * 150);
                    vector.Y = (float)(Math.Cos(angle) * 150);
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, DustID.AncientLight, 0f, 0f, 100, default, 3f)];
                    dust2.noGravity = true;
                    dust2.velocity = -Projectile.DirectionTo(dust2.position) * 10f;
                }
            }
            if (Projectile.timeLeft < 50)
            {
                Projectile.alpha += 7;
            }
            if (Projectile.timeLeft == 180)
            {
                Projectile.scale = 0.1f;
                player.RedemptionScreen().Rumble(180, 3);
                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Octavia...", 120, 30, 0.6f, null, 2, Color.DarkGray);
            }
        }
        private float spiritOpacity;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive && spiritOpacity <= 0)
                return false;

            Texture2D texture = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/SoullessPortal").Value;
            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Texture2D keeper = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Keeper/Keeper_Closure").Value;
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity * spiritOpacity, -Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale * 1.2f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity * spiritOpacity, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(flare, Projectile.Center - new Vector2(10, 20) - Main.screenPosition, null, Color.White * Projectile.Opacity * spiritOpacity, 0, new Vector2(flare.Width / 2, flare.Height / 2), Projectile.scale * .5f, 0, 0);
            Main.EntitySpriteDraw(flare, Projectile.Center - new Vector2(-10, 20) - Main.screenPosition, null, Color.White * Projectile.Opacity * spiritOpacity, 0, new Vector2(flare.Width / 2, flare.Height / 2), Projectile.scale * .5f, 0, 0);

            int height = keeper.Height / 10;
            int y = height * (int)Projectile.ai[0];
            Rectangle rect = new(0, y, keeper.Width, height);
            Vector2 origin = new(keeper.Width / 2f, height / 2f);
            Main.EntitySpriteDraw(keeper, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * Projectile.Opacity * spiritOpacity, 0, origin, 2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            int dustType = DustID.AncientLight;
            int pieCut = 40;
            for (int m = 0; m < pieCut; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType, 0f, 0f, 100, Color.White, 2f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)pieCut * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
        }
    }
}