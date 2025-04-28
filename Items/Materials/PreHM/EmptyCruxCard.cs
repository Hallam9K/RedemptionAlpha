using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.WorldGeneration;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class EmptyCruxCard : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 5;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateInventory(Player player)
        {
            if (player.whoAmI == Main.myPlayer && !player.RedemptionAbility().Spiritwalker && Main.rand.NextBool(5000) && player.ZoneRockLayerHeight && player.ownedProjectileCounts[ProjectileType<GuidingStranger_Proj>()] < 1)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ProjectileType<GuidingStranger_Proj>(), 0, 0, player.whoAmI);
        }
        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && !player.RedemptionAbility().Spiritwalker && Main.rand.NextBool(100) && player.ZoneRockLayerHeight && player.ownedProjectileCounts[ProjectileType<GuidingStranger_Proj>()] < 1)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ProjectileType<GuidingStranger_Proj>(), 0, 0, player.whoAmI);
        }
    }
    public class GuidingStranger_Proj : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Friendly/SpiritWalkerMan";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 44;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }
        float opacity;
        public override void AI()
        {
            Vector2 corpsePos = (RedeGen.gathicPortalVector + new Vector2(67, 21)) * 16;
            if (Projectile.timeLeft > 100 && Projectile.DistanceSQ(corpsePos) < 400 * 400)
                Projectile.timeLeft = 100;

            Projectile.velocity = RedeHelper.PolarVector(2, (corpsePos - Projectile.Center).ToRotation());

            Projectile.LookByVelocity();

            if (Projectile.timeLeft < 500 && Projectile.timeLeft >= 400)
                opacity += .01f;
            else if (Projectile.timeLeft <= 100)
                opacity -= .01f;

            opacity = MathHelper.Clamp(opacity, 0, 1);

            Projectile.rotation = Projectile.velocity.X * 0.07f;

            Projectile.alpha += Main.rand.Next(-10, 11);
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 230, 240);
            Projectile.position.Y += (float)Math.Sin(Projectile.localAI[0]++ / 15) / 2;
            Projectile.position.X += (float)Math.Sin(Projectile.localAI[0]++ / 15) / 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rect = texture.Frame(1, 4, 0, Projectile.frame);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.White with { A = 0 }) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, rect, color * opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }
            return false;
        }
    }
}