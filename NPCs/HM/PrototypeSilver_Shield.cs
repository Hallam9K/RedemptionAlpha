using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;

namespace Redemption.NPCs.HM
{
    public class PrototypeSilver_Shield : ModProjectile
    {
        public override string Texture => "Redemption/Textures/BubbleShield";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bubble Shield");
        }
        public override void SetDefaults()
        {
            Projectile.width = 175;
            Projectile.height = 175;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<PrototypeSilver>())
                Projectile.Kill();

            Projectile.Center = npc.Center;
            npc.ai[3] = 1;
            Projectile.timeLeft = 10;

            Projectile.alpha += 2;
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);

            Projectile.scale -= 0.02f;
            Projectile.scale = (int)MathHelper.Clamp(Projectile.scale, 1, 2);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile target = Main.projectile[i];
                if (Projectile == target || !target.active || target.damage <= 0 || !target.friendly || target.hostile || target.ProjBlockBlacklist())
                    continue;

                if (!Projectile.Hitbox.Intersects(target.Hitbox))
                    continue;

                target.Kill();
                Projectile.localAI[0] += target.damage;
                CombatText.NewText(Projectile.getRect(), Color.Orange, target.damage, true, true);
                SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
                Projectile.alpha -= 40;
                Projectile.scale += 0.04f;

                if (Projectile.localAI[0] < 800)
                    continue;

                SoundEngine.PlaySound(SoundID.NPCDeath56, Projectile.position);
                for (int k = 0; k < 20; k++)
                {
                    Vector2 vector;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector.X = (float)(Math.Sin(angle) * 66);
                    vector.Y = (float)(Math.Cos(angle) * 66);
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, DustID.Frost, 0f, 0f, 100, default, 3f)];
                    dust2.noGravity = true;
                    dust2.velocity = Projectile.DirectionTo(dust2.position) * 4f;
                }
                Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            npc.ai[3] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D overlay = ModContent.Request<Texture2D>("Redemption/Textures/BubbleShield_Overlay").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(overlay, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.FadeColour1 with { A = 0 }), Projectile.rotation, drawOrigin, Projectile.scale * 0.75f, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale * 0.75f, effects, 0);
            return false;
        }
    }
}