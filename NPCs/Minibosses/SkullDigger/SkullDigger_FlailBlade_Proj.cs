using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.SkullDigger
{
    public class SkullDigger_FlailBlade_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skull Digger's Skull Digger");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.alpha = 180;
        }

        public override void AI()
        {
            Projectile host = Main.projectile[(int)Projectile.ai[0]];
            NPC boss = Main.npc[(int)Projectile.ai[1]];
            if (!host.active || host.type != ModContent.ProjectileType<SkullDigger_FlailBlade>())
                Projectile.Kill();

            if (Projectile.localAI[0]++ == 0)
                Projectile.rotation = host.rotation;

            if (Projectile.localAI[0] == 5)
                Projectile.velocity = RedeHelper.PolarVector(0.08f, (Main.player[boss.target].Center - Projectile.Center).ToRotation());
            if (Projectile.localAI[0] >= 5)
            {
                Projectile.LookByVelocity();
                Projectile.velocity *= 1.06f;
                Projectile.rotation += Projectile.velocity.Length() / 30 * Projectile.spriteDirection;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0, 0, Scale: 2);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var effects = SpriteEffects.None;
            if (Projectile.ai[0] <= 0)
            {
                Projectile host = Main.projectile[(int)Projectile.ai[0]];
                effects = host.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            Texture2D ballTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, ballTexture.Width, ballTexture.Height);
            Vector2 origin2 = new(ballTexture.Width / 2, ballTexture.Height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2f - Main.screenPosition;
                Color color = Projectile.GetAlpha(Color.LightCyan) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ballTexture, drawPos, new Rectangle?(rect), color, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(ballTexture, position, new Rectangle?(rect), Projectile.GetAlpha(new Color(255, 255, 255, 0)), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
    public class SkullDigger_FlailBlade_ProjF : SkullDigger_FlailBlade_Proj
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_FlailBlade_Proj";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Skull Digger's Skull Digger");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }
        public override bool? CanCutTiles() => false;
        public override bool PreAI()
        {
            Projectile host = Main.projectile[(int)Projectile.ai[0]];
            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0]++ == 0)
                Projectile.rotation = host.rotation;
            bool spirit = Projectile.localAI[1] == 1;
            if (!spirit && Projectile.localAI[0] == 5 && player.whoAmI == Main.myPlayer)
            {
                Projectile.velocity = RedeHelper.PolarVector(0.08f, (Main.MouseWorld - Projectile.Center).ToRotation());
            }
            if (Projectile.localAI[0] >= 5)
            {
                Projectile.LookByVelocity();
                Projectile.velocity *= 1.06f;
                Projectile.rotation += Projectile.velocity.Length() / 30 * Projectile.spriteDirection;
            }

            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.timeLeft -= 4;
            return false;
        }
    }
}