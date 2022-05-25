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
    public class EvergoldSoup : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Massive improvements to all stats\n" +
                "'Tastes like royalty'");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(231, 84, 211),
                new Color(229, 129, 82),
                new Color(118, 51, 51)
            };
            ItemID.Sets.IsFood[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(30, 28, ModContent.BuffType<WellFed4>(), 216000, true);
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        // TODO: Evergold Soup recipe
        /*public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<EvergoldNautilus>())
                .AddIngredient(ModContent.ItemType<SlateShell>(), 2)
                .AddIngredient(ModContent.ItemType<Caveshroom>(), 3)
                .AddIngredient(ModContent.ItemType<ToxicAngel2>())
                .AddTile(TileID.CookingPots)
                .Register();
        }*/
    }
}