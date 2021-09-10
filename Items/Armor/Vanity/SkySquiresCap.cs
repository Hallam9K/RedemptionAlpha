using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class SkySquiresCap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Squire's Cap");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.sellPrice(0, 0, 24, 0);
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawAltHair = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient(ItemID.Cloud, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}