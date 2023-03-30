using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Armor.PreHM.LivingWood
{
    [AutoloadEquip(EquipType.Legs)]
    public class LivingWoodLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Living Wood Leggings");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.sellPrice(copper: 40);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LivingTwig>(30)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}