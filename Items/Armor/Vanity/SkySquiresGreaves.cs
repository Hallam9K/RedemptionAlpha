using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Legs)]
    public class SkySquiresGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Squire's Greaves");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 59, 0);
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 12)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient(ItemID.Cloud, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}