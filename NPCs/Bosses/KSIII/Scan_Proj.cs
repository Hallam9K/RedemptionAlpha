using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.HM;
using Terraria.GameContent;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class Scan_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/Ray";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Scan");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != ModContent.NPCType<KS3_ScannerDrone>() && npc.type != ModContent.NPCType<Android>()))
                Projectile.Kill();

            if (npc.type == ModContent.NPCType<Android>() && npc.ai[0] != 2)
                Projectile.Kill();

            Vector2 Pos = new(npc.Center.X + 5 * npc.spriteDirection, npc.Center.Y - 3);
            if (npc.type == ModContent.NPCType<Android>())
                Pos = new(npc.Center.X + 19 * npc.spriteDirection, npc.Center.Y - 1);

            Projectile.Center = Pos;
            switch (Projectile.localAI[1])
            {
                case 0:
                    Projectile.localAI[0] -= 0.03f;
                    if (Projectile.localAI[0] <= -0.6f)
                    {
                        Projectile.localAI[0] = -0.6f;
                        Projectile.localAI[1] = 1;
                    }
                    break;
                case 1:
                    Projectile.localAI[0] += 0.03f;
                    if (Projectile.localAI[0] >= 0.6f)
                    {
                        Projectile.localAI[0] = 0.6f;
                        Projectile.localAI[1] = 0;
                    }
                    break;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (1.57f * -npc.spriteDirection) + Projectile.localAI[0];
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightBlue), Projectile.rotation, drawOrigin, new Vector2(Projectile.scale - (npc.type == ModContent.NPCType<Android>() ? 0.4f : 0), Projectile.scale + 1), effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}