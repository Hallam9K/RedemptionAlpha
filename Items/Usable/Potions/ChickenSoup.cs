using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class ChickenSoup : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'Waiter, there's a beak in my soup'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(246, 200, 71),
                new Color(216, 129, 35),
                new Color(174, 194, 14)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(38, 26, BuffID.WellFed, 54000, true);
            Item.value = 2500;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RedeRecipe.ChickenRecipeGroup)
                .AddIngredient(ItemID.Mushroom)
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}