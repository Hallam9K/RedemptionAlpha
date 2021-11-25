using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
	public class OblitBrain : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Obliterator Brain");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 32;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 50, 0, 0);
			Item.rare = ItemRarityID.Red;
		}
	}
}
