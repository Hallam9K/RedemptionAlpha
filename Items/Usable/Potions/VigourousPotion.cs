using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs;
using Redemption.Globals;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Plants;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class VigourousPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Greatly increases life regeneration for a short time");
            SacrificeTotal = 20;
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 14;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = ModContent.RarityType<CosmicRarity>();
            Item.buffType = ModContent.BuffType<VigourousBuff>();
            Item.buffTime = 2400;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LifeFragment>(), 2)
                .AddIngredient(ItemID.RegenerationPotion)
                .AddTile(TileID.Bottles)
                .Register();
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            float opac = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 1f, 0f);

            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), drawColor, 0, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, position, new Rectangle(0, 0, texture.Width, texture.Height), new Color(255, 255, 255, 150) * opac, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            float opac = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 1f, 0f);
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, frame, new Color(255, 255, 255, 150) * opac, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}