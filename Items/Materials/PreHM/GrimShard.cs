using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Materials.PreHM
{
    public class GrimShard : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("'Gives the Undead power'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = Item.sellPrice(0, 0, 0, 25);
			Item.rare = ItemRarityID.Blue;
		}
	}
}
