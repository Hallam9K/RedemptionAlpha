using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.ElderWood
{
    [AutoloadEquip(EquipType.Body)]
    public class ElderWoodBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;
            Item.value = Item.sellPrice(copper: 35);
            Item.rare = ItemRarityID.White;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic).Flat += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Placeable.Tiles.ElderWood>(30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}