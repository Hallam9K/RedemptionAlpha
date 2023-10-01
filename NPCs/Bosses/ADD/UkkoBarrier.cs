using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.DataStructures;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoBarrier : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Barrier");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        private Vector2 originPos;
        private Vector2 originPos2;
        public override void OnSpawn(IEntitySource source)
        {
            originPos = Projectile.Center;
        }
        public override void AI()
        {
            if (ArenaWorld.arenaActive)
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();

            Projectile.spriteDirection = Projectile.ai[1] >= 0 ? 1 : -1;
            originPos2.Y += 25.6f;
            if (originPos2.Y >= 1024)
                originPos2.Y = 0;
            Player player = Main.LocalPlayer;

            if (Projectile.ai[1] > 0)
            {
                float newX = ArenaWorld.arenaTopLeft.X - 16;

                Projectile.Center = new Vector2(newX, player.Center.Y);
            }
            else
            {
                float newX = ArenaWorld.arenaTopLeft.X + ArenaWorld.arenaSize.X + 16;

                Projectile.Center = new Vector2(newX, player.Center.Y);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D fog = ModContent.Request<Texture2D>(Texture + "_Fog").Value;
            int projHeight = texture.Height;
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawPos = originPos;
            drawPos.Y += projHeight * 10;
            Vector2 origin = new(texture.Width - 65, texture.Height - 512);
            if (Projectile.spriteDirection == -1)
                origin.X = 65;

            for (int i = 0; i < 20; i++)
            {
                Main.EntitySpriteDraw(fog, drawPos - originPos2 + new Vector2(Projectile.spriteDirection == -1 ? 50 : -1000, 0) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, 1000, projHeight), Projectile.GetAlpha(Color.Gray), 0, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture, drawPos + originPos2 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(Color.Gray), 0, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture, drawPos - originPos2 + new Vector2(50 * Projectile.spriteDirection, 0) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(Color.Gray) * 0.6f, 0, origin, Projectile.scale, effects, 0);
                drawPos.Y -= projHeight;
            }
            return false;
        }
    }
    public class UkkoBarrierH : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/ADD/UkkoBarrier";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Barrier");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        private Vector2 originPos;
        private Vector2 originPos2;
        public override void OnSpawn(IEntitySource source)
        {
            originPos = Projectile.Center;
        }
        public override void AI()
        {
            if (ArenaWorld.arenaActive)
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();
            originPos2.X += 25.6f;
            if (originPos2.X >= 1024)
                originPos2.X = 0;
            Player player = Main.LocalPlayer;

            if (Projectile.ai[1] > 0)
            {
                float newY = ArenaWorld.arenaTopLeft.Y - 16;

                Projectile.Center = new Vector2(player.Center.X, newY);
            }
            else
            {
                float newY = ArenaWorld.arenaTopLeft.Y + ArenaWorld.arenaSize.Y + 16;

                Projectile.Center = new Vector2(player.Center.X, newY);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D fog = ModContent.Request<Texture2D>(Texture + "_Fog").Value;
            int projHeight = texture.Height;
            var effects = Projectile.ai[1] >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawPos = originPos;
            drawPos.X += projHeight * 10;
            Vector2 origin = new(texture.Width - 65, texture.Height - 512);
            if (Projectile.ai[1] >= 0)
                origin.Y = 65;

            for (int i = 0; i < 20; i++)
            {
                Main.EntitySpriteDraw(fog, drawPos - originPos2 - new Vector2(0, Projectile.ai[1] >= 0 ? 1000 : -100) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle(0, 0, 1000, projHeight), Projectile.GetAlpha(Color.Gray), MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture, drawPos + originPos2 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(Color.Gray), MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture, drawPos - originPos2 + new Vector2(0, 50 * (Projectile.ai[1] >= 0 ? 1 : -1)) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(Color.Gray) * 0.6f, MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
                drawPos.X -= projHeight;
            }
            return false;
        }
    }
}