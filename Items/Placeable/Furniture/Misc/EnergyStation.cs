using Terraria.ID;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Redemption.Items.Materials.HM;
using Redemption.Items.Usable;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class EnergyStation : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("Place down and right-click to recharge your Energy if an Energy Pack is in your inventory");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<EnergyStationTile>(), 0);
			Item.width = 42;
			Item.height = 40;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(0, 12, 0, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Xenomite>(), 20)
                .AddIngredient(ModContent.ItemType<Plating>(), 8)
                .AddIngredient(ModContent.ItemType<Capacitor>(), 2)
                .AddIngredient(ModContent.ItemType<EnergyCell>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            spriteBatch.Draw(texture, position, null, drawColor, 0, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, position, null, RedeColor.EnergyPulse, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, null, lightColor, rotation, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, null, RedeColor.EnergyPulse, rotation, origin, scale, 0, 0f);
            return false;
        }
    }
}