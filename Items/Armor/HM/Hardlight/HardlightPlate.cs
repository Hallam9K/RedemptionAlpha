using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.HM.Hardlight
{
    [AutoloadEquip(EquipType.Body)]
    public class HardlightPlate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("10% increased damage");

            Item.ResearchUnlockCount = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .1f;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 26;
            Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CyberPlating>(), 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}