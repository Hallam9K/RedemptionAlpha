using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    public class SkySquiresTabard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Squire's Tabard");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 0, 74, 0);
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 16)
                .AddIngredient(ItemID.Silk, 8)
                .AddIngredient(ItemID.Cloud, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}