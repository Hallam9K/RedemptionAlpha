using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
	[AutoloadEquip(EquipType.Head)]
	public class GirusMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Girus Mask");
			Tooltip.SetDefault("'The mechanical corruption's source...'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.rare = ItemRarityID.Red;
			Item.vanity = true;
		}
	}
}
