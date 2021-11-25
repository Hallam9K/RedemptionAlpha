using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
	public class VlitchScale : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vlitch Scale");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 30;
			Item.maxStack = 99;
            Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Red;
		}
	}
}
