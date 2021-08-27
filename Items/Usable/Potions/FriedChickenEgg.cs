using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class FriedChickenEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'Because eggs are tasty.'");
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.width = 12;
            Item.height = 38;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 3600;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ChickenEgg>())
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}