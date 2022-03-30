using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Base;

namespace Redemption.Items.Usable
{
    public class ManiacsLantern_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Maniac's Lantern");
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public float moveY;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.timeLeft = 10;
            if (player.HeldItem.type != ModContent.ItemType<ManiacsLantern>() || !player.active || player.dead)
                Projectile.Kill();

            if (player.direction == 1)
                Projectile.spriteDirection = 1;
            else
                Projectile.spriteDirection = -1;

            Lighting.AddLight(Projectile.Center, 1, 1, 1);
            if (Projectile.localAI[0] == 0)
            {
                moveY = player.Center.Y - 40;
                Projectile.localAI[0] = 1;
            }
            Projectile.MoveToVector2(new Vector2(player.Center.X + (40 * player.direction), moveY), 20);
            MoveClamp();
            if (Projectile.DistanceSQ(player.Center) > 2000 * 2000)
                Projectile.Kill();
            for (int p = 0; p < Main.maxNPCs; p++)
            {
                NPC target = Main.npc[p];
                if (!target.active || target.boss || target.friendly || !NPCLists.Soulless.Contains(target.type))
                    continue;

                if (Projectile.DistanceSQ(target.Center) >= 140 * 140)
                    continue;

                target.velocity -= RedeHelper.PolarVector(0.3f, (Projectile.Center - target.Center).ToRotation());
            }
        }
        public void MoveClamp()
        {
            Player player = Main.player[Projectile.owner];
            if (moveY < player.Center.Y - 40)
                moveY = player.Center.Y - 40;
            else if (moveY > player.Center.Y + 10)
                moveY = player.Center.Y + 10;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 0.8f, 1f);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.White, Color.White * 0.8f, Color.White);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), color * 0.5f, Projectile.rotation, origin, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}