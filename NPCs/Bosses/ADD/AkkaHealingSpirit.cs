using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Redemption.Helpers;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaHealingSpirit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Healing Spirit");
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
            Projectile.alpha = 255;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            NPC akka = Main.npc[(int)Projectile.ai[0]];
            if (akka.active && akka.type == ModContent.NPCType<Akka>())
                Projectile.Center = akka.Center;
            if (Projectile.localAI[0] == 1f)
            {
                Projectile.alpha += 6;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha <= 0)
                    Projectile.localAI[0] = 1f;
            }
            for (int p = 0; p < Main.maxNPCs; p++)
            {
                NPC npc = Main.npc[p];
                if (npc.active && !npc.immortal && !npc.dontTakeDamage && Projectile.alpha < 200 && Projectile.Hitbox.Intersects(npc.Hitbox))
                {
                    int healAmt = 1;
                    if (npc.type == ModContent.NPCType<Ukko>())
                        healAmt = 20;
                    if (npc.life <= npc.lifeMax - healAmt)
                    {
                        npc.life += healAmt;
                        npc.HealEffect(healAmt);
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Water_Jungle);
                        dust.velocity = -npc.DirectionTo(dust.position);
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, position, null, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}