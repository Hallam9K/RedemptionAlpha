using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class PouchBelt : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Pouch Belt");
            Tooltip.SetDefault("+2 druidic damage"
                + "\n2% increased druidic critical strike chance");
        }

		public override void SetDefaults()
		{
            Item.width = 30;
            Item.height = 24;
            Item.value = Item.sellPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.GetModPlayer<BuffPlayer>().DruidDamageFlat += 2;
            player.GetCritChance<DruidClass>() += 2;
		}
	}
}
