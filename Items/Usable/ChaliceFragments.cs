using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Items.Materials.PreHM;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ChaliceFragments : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Holding this in your hand will point to an ancient structure");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CursedGem>();

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Lime;
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<HallPointer>()] < 1)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<HallPointer>(), 0, 0, player.whoAmI);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Lime.ToVector3() * 0.6f * Main.essScale);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(textureGlow, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
    public class HallPointer : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pointer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 HallPos = new((RedeGen.HallOfHeroesVector.X + 36) * 16, (RedeGen.HallOfHeroesVector.Y + 17) * 16);
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<ChaliceFragments>())
                Projectile.timeLeft = 10;

            Projectile.Center = player.Center;
            Projectile.rotation = (HallPos - player.Center).ToRotation();
            Projectile.direction = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.1f, 0.3f, 0.1f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.05f, 0.15f, 0.05f);
            float alpha = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.5f, 1f, 0.5f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * alpha, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale + scale2, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale + scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
