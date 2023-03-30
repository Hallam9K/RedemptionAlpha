using Terraria;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.GameContent;

namespace Redemption.NPCs.Bosses.ADD
{
    public class EarthBarrier : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Floating Island");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 224;
            Projectile.height = 98;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 340;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            int akka = NPC.FindFirstNPC(ModContent.NPCType<Akka>());

            Projectile.velocity *= 0;
            Projectile.position = new Vector2(player.Center.X - 112, player.Center.Y - 250);

            Projectile.frame = (int)Projectile.ai[0];
            if (Projectile.timeLeft < 40 || akka <= -1 || !Main.npc[akka].active || Main.npc[akka].type != ModContent.NPCType<Akka>())
            {
                if (Projectile.timeLeft > 40)
                    Projectile.timeLeft = 40;
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Vector2 handPos1 = Main.npc[akka].Center + new Vector2(39 * Main.npc[akka].spriteDirection, -21);
                Vector2 handPos2 = Main.npc[akka].Center + new Vector2(-11 * Main.npc[akka].spriteDirection, -17);

                Dust dust = Dust.NewDustDirect(handPos1 - new Vector2(8, 8), 16, 16, DustID.PoisonStaff, Scale: 2);
                dust.velocity = -Projectile.DirectionTo(dust.position) * 30;
                dust.noGravity = true;
                dust = Dust.NewDustDirect(handPos2 - new Vector2(8, 8), 16, 16, DustID.PoisonStaff, Scale: 2);
                dust.velocity = -Projectile.DirectionTo(dust.position) * 30;
                dust.noGravity = true;
            }
            if (Projectile.alpha > 0 && Projectile.timeLeft >= 60)
            {
                Projectile.alpha -= 10;
            }

            var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
            foreach (var proj in list)
            {
                if (!proj.active || Projectile == proj || !proj.friendly || proj.ProjBlockBlacklist())
                    continue;

                proj.Kill();
            }
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.LightGreen * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}