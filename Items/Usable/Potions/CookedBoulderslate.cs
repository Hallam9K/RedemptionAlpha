using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class CookedBoulderslate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'Tastes like slate... and boulders'");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(251, 151, 108),
                new Color(219, 109, 68),
                new Color(160, 83, 63)
            };
            ItemID.Sets.IsFood[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
        public override void SetDefaults()
        {
            Item.DefaultToFood(34, 18, ModContent.BuffType<WellFed4>(), 7200);
            Item.value = 65;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Boulderslate>())
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}