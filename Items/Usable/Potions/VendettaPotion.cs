using Redemption.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class VendettaPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Attackers also take damage, and get inflicted by poison");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void SetDefaults()
		{
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 32;
            Item.maxStack = 30;
            Item.value = Item.sellPrice(0, 0, 3, 0);
            Item.rare = ItemRarityID.Blue;
            Item.buffType = ModContent.BuffType<VendettaPotionBuff>();
            Item.buffTime = 10800;
        }
        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod)
            {
                alchemy = true
            };
            recipe.AddIngredient(null, "Nightshade", 2);
            recipe.AddIngredient(ItemID.Stinger);
            recipe.AddIngredient(ItemID.WormTooth);
            recipe.AddIngredient(ItemID.Cactus);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Nightshade", 2);
            recipe.AddIngredient(ItemID.ThornsPotion);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
	}
}
