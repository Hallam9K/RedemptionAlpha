using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.SoulSoldier
{
    [AutoloadEquip(EquipType.Body)]
    public class SoulSoldierChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.sellPrice(0, 0, 35);
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GraveSteelAlloy>(20)
                .AddIngredient<LostSoul>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}