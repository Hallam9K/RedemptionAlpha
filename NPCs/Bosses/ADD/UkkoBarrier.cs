using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
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
            DisplayName.SetDefault("Barrier");
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
            float distX = player.Center.X - Projectile.Center.X;
            distX = Math.Abs(distX);
            if (distX > 700) distX = 700;
            visibility = 1f * (700 - distX) / 700;

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
        float visibility = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int projHeight = texture.Height;
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawPos = originPos;
            drawPos.Y += projHeight * 20;
            Vector2 origin = new(texture.Width - 65, texture.Height - 512);
            if (Projectile.spriteDirection == -1)
                origin.X = 65;

            for (int i = 0; i < 40; i++)
            {
                Main.EntitySpriteDraw(texture, drawPos + originPos2 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor) * visibility, 0, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture, drawPos - originPos2 + new Vector2(50 * Projectile.spriteDirection, 0) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor) * 0.6f * visibility, 0, origin, Projectile.scale, effects, 0);
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
            DisplayName.SetDefault("Barrier");
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
            float distY = player.Center.Y - Projectile.Center.Y;
            distY = Math.Abs(distY);
            if (distY > 700) distY = 700;
            visibility = 1f * (700 - distY) / 700;

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
        float visibility = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int projHeight = texture.Height;
            var effects = Projectile.ai[1] >= 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 drawPos = originPos;
            drawPos.X += projHeight * 20;
            Vector2 origin = new(texture.Width - 65, texture.Height - 512);
            if (Projectile.ai[1] >= 0)
                origin.Y = 65;

            for (int i = 0; i < 40; i++)
            {
                Main.EntitySpriteDraw(texture, drawPos + originPos2 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor) * visibility, MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture, drawPos - originPos2 + new Vector2(0, 50 * Projectile.ai[1] >= 0 ? 1 : -1) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor) * 0.6f * visibility, MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
                drawPos.X -= projHeight;
            }
            return false;
        }
    }
}