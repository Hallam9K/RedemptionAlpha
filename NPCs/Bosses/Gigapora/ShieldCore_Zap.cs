using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Redemption.Particles;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class ShieldCore_Zap : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dualcast");
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }
        public override bool CanHitPlayer(Player target) => Projectile.ai[1] >= 60;
        private bool Flare;
        private int FlareTimer;
        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || host.type != ModContent.NPCType<Gigapora_ShieldCore>())
                Projectile.Kill();

            if (Projectile.ai[1]++ < 60)
            {
                for (int k = 0; k < 2; k++)
                {
                    Vector2 vector;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector.X = (float)(Math.Sin(angle) * 60);
                    vector.Y = (float)(Math.Cos(angle) * 60);
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, DustID.Electric)];
                    dust2.shader = GameShaders.Armor.GetSecondaryShader(GameShaders.Armor.GetShaderIdFromItemId(ItemID.RedandBlackDye), Main.LocalPlayer);
                    dust2.noGravity = true;
                    dust2.noLight = true;
                    dust2.velocity = dust2.position.DirectionTo(Projectile.Center) * 6f;
                }
            }
            if (Projectile.ai[1] == 60)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Zap2, Projectile.position);
                DustHelper.DrawParticleElectricity<LightningParticle>(host.Center, Projectile.Center, 2f, 20, 0.1f, 2);
                DustHelper.DrawParticleElectricity<LightningParticle>(host.Center, Projectile.Center, 2f, 20, 0.1f, 2);
                Flare = true;
            }
            if (Flare)
            {
                FlareTimer += 3;
                if (FlareTimer > 60)
                    Projectile.Kill();
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color colour = Color.Lerp(Color.Red, Color.Red, 1f / FlareTimer * 10f) * (1f / FlareTimer * 10f);
            if (Flare)
            {
                Main.spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 2, 0, origin, 2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 1f, 0, origin, 2f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}
