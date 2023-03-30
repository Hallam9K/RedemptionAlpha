using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PostML;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Armor.PostML.Xenium
{
    [AutoloadEquip(EquipType.Legs)]
    public class XeniumLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("10% increased damage\n" +
                "13% increased critical strike chance\n" +
                "30% increased movement speed"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.defense = 22;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed *= 1.3f;
            player.GetDamage<GenericDamageClass>() += .1f;
            player.GetCritChance<GenericDamageClass>() += 13;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<XeniumAlloy>(), 12)
            .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 4)
            .AddTile(ModContent.TileType<XeniumRefineryTile>())
            .Register();
        }
    }
}