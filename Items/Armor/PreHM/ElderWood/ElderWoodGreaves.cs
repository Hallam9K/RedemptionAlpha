using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.ElderWood
{
    [AutoloadEquip(EquipType.Legs)]
    public class ElderWoodGreaves : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(copper: 40);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic).Flat += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Placeable.Tiles.ElderWood>(25)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}