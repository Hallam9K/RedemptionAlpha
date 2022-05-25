using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class AncientMushroomSoup : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Massive improvements to all stats\n" +
                "'Mushroom, mushroom'");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(203, 185, 151),
                new Color(88, 69, 55),
                new Color(231, 84, 211)
            };
            ItemID.Sets.IsFood[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(30, 22, ModContent.BuffType<WellFed4>(), 54000, true);
            Item.value = Item.sellPrice(0, 1, 85, 0);
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        // TODO: Ancient Mushroom Soup recipe
        /*public override void AddRecipes() 
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Caveshroom>(), 7)
                .AddIngredient(ModContent.ItemType<ToxicAngel2>(), 2)
                .AddTile(TileID.CookingPots)
                .Register();
        }*/
    }
}