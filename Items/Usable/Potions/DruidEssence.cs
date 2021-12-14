using Redemption.Buffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
	public class DruidEssence : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Druid Essence");
			Tooltip.SetDefault("5% increased druidic damage and critical strike chance");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }
		public override void SetDefaults()
		{
			Item.UseSound = SoundID.Item3;
			Item.useStyle = 2;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.consumable = true;
			Item.width = 20;
			Item.height = 32;
			Item.maxStack = 30;
			Item.value = Item.sellPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.Blue;
			Item.buffType = ModContent.BuffType<DruidEssenceBuff>();
			Item.buffTime = 10800;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RedeRecipe.PlantRecipeGroup, 2)
            .AddIngredient(ItemID.BottledWater, 1)
            .AddTile(TileID.Bottles)
            .Register();
        }
	}
}
