using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PostML.Xenium
{
    [AutoloadEquip(EquipType.Body)]
    public class XeniumBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("10% increased damage");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 28;
            Item.sellPrice(7, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.defense = 32;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += .10f;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<XeniumAlloy>(), 16)
            .AddIngredient(ModContent.ItemType<Capacitor>(), 1)
            .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
            .AddTile(ModContent.TileType<XeniumRefineryTile>())
            .Register();
        }
    }
}