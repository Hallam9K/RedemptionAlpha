using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class DeepDwellDish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Dwell Dish");
            Tooltip.SetDefault("'Tastes like [REDACTED]'"
                + "\nMinor improvements to all stats");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 0, 25, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 14400;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LurkingKetred>())
                .AddTile(TileID.CookingPots)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ChakrogAngler>())
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}