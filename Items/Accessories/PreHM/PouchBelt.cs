using Redemption.DamageClasses;
using Terraria;
using Terraria.GameContent.Creative;
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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            player.GetDamage<DruidClass>().Flat += 2;
            player.GetCritChance<DruidClass>() += 2;
		}
	}
}
