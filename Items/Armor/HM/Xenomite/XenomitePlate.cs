using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.HM.Xenomite
{
    [AutoloadEquip(EquipType.Body)]
    public class XenomitePlate : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("5% increased damage\n" +
            "7% increased critical strike chance"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .05f;
            player.GetCritChance(DamageClass.Generic) += 7;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 24;
            Item.sellPrice(silver:50);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 8)
                .AddIngredient(ModContent.ItemType<Materials.HM.Xenomite>(), 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 8)
                .AddIngredient(ModContent.ItemType<Materials.HM.Xenomite>(), 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
