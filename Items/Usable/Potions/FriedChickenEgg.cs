using Microsoft.Xna.Framework;
using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class FriedChickenEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'Because eggs are tasty.'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(247, 226, 199),
                new Color(242, 183, 111),
                new Color(235, 161, 31)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(28, 18, BuffID.WellFed, 3600);
            Item.value = 100;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
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