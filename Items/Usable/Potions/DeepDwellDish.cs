using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class DeepDwellDish : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Deep Dwell Dish");
            /* Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'Tastes like [REDACTED]'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(229, 124, 206),
                new Color(103, 84, 119),
                new Color(120, 146, 19)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.DefaultToFood(36, 22, ModContent.BuffType<WellFed4>(), 6000);
            Item.value = 55;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<SoullessRarity>();
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