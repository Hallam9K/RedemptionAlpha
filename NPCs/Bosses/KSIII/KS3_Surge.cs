using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Surge : ModProjectile
    {
        public override string Texture => "Redemption/Textures/StaticBall";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Core Surge");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != ModContent.NPCType<KS3>() && npc.type != ModContent.NPCType<KS3_Clone>()))
                Projectile.Kill();

            Projectile.Center = npc.Center;
            Projectile.scale += 0.2f;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            if (Projectile.alpha < 200 && Main.rand.NextBool(3))
            {
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(280, RedeHelper.RandomRotation()), 2f, 20, 0.1f);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(280, RedeHelper.RandomRotation()), 2f, 20, 0.1f);
            }

            Projectile.alpha += 10;
            if (Projectile.alpha >= 255)
                Projectile.Kill();

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (!player.active || player.dead || Projectile.DistanceSQ(player.Center) >= 280 * 280)
                    continue;

                player.AddBuff(BuffID.Electrified, 320);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.LightCyan), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
    public class KS3_Surge2 : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Core Surge");
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 20;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (!player.active || player.noKnockback || !Projectile.Hitbox.Intersects(player.Hitbox))
                    continue;

                player.velocity = Projectile.velocity;
            }
        }
    }
}