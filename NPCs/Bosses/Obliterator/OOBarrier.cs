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

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OOBarrier : ModProjectile
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
            Projectile.hide = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        private Vector2 originPos;
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

            Player player = Main.LocalPlayer;
            float distX = player.Center.X - Projectile.Center.X;
            distX = Math.Abs(distX);
            if (distX > 500) distX = 500;
            visibility = 0.5f * (500 - distX) / 500;

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
            drawPos.Y += projHeight * 200;
            Vector2 origin = new(texture.Width - 8, texture.Height - 8);
            if (Projectile.spriteDirection == -1)
                origin.X = 8;

            for (int i = 0; i < 400; i++)
            {
                Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(RedeColor.RedPulse) * visibility, 0, origin, Projectile.scale, effects, 0);
                drawPos.Y -= projHeight;
            }
            return false;
        }
    }
    public class OOBarrierH : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Obliterator/OOBarrier";
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
            Projectile.hide = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        private Vector2 originPos;
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

            Player player = Main.LocalPlayer;
            float distY = player.Center.Y - Projectile.Center.Y;
            distY = Math.Abs(distY);
            if (distY > 500) distY = 500;
            visibility = 0.5f * (500 - distY) / 500;

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
            drawPos.X += projHeight * 200;
            Vector2 origin = new(texture.Width - 8, texture.Height - 8);
            if (Projectile.ai[1] >= 0)
                origin.Y = 8;

            for (int i = 0; i < 400; i++)
            {
                Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(RedeColor.RedPulse) * visibility, MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
                drawPos.X -= projHeight;
            }
            return false;
        }
    }
}