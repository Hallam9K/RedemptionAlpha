using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.NPCs.Lab.MACE;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Furniture.Lab
{
    public class BigMaceTurret_NPC : ModProjectile
    {
        public Tile Parent;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory MACE Turret");
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (!Parent.IsActive || Parent.type != ModContent.TileType<BigMaceTurretTile>() || !LabArea.Active)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;

            if (Parent.frameX == 18)
                Projectile.spriteDirection = -1;
            else
                Projectile.spriteDirection = 1;

            Projectile.velocity *= 0;
            int Mace = NPC.FindFirstNPC(ModContent.NPCType<MACEProject>());
            if (Mace >= 0)
            {
                Player target = Main.player[Main.npc[Mace].target];
                Projectile.rotation.SlowRotation(Projectile.DirectionTo(target.Center).ToRotation() - MathHelper.Pi, (float)Math.PI / 120f);
            }
            else
            {
                Projectile.rotation = -MathHelper.PiOver2;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2 - 8);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation + (Projectile.spriteDirection == -1 ? (float)Math.PI : 0), drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}