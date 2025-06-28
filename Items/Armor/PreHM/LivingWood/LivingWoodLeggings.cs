using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.value = Item.sellPrice(copper: 40);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LivingTwig>(30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}