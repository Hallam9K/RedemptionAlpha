using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class ToxicBile : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
		}

		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 24;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 0, 9, 0);
			Item.rare = ItemRarityID.Lime;
		}
    }
}
