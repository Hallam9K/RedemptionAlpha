using Microsoft.Xna.Framework;
using Redemption.Items.Critters;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class RoastLarva : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Roast Larva");
            /* Tooltip.SetDefault("Medium improvements to all stats" +
                               "\n'The forbidden croissant'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(230, 164, 100),
                new Color(187, 133, 81),
                new Color(142, 86, 30)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(30, 18, BuffID.WellFed2, 21600);
            Item.value = Item.sellPrice(silver: 5);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GrandLarvaBait>())
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}