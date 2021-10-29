using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Bioweapon : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Bio-Weapon");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 22;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 0, 8, 0);
			Item.rare = ItemRarityID.Lime;
		}
    }
}
