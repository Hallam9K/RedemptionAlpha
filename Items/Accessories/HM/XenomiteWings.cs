using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
	[AutoloadEquip(EquipType.Wings)]
	public class XenomiteWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Xenomite Wings");
		    Tooltip.SetDefault("Allows flight and slow fall");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(100, 7f, 2.5f);
        }
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = 300000;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
		}
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.85f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 1.7f;
			constantAscend = 0.135f;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<XenomiteItem>(), 20)
			.AddIngredient(ModContent.ItemType<StarliteBar>(), 5)
			.AddIngredient(ItemID.SoulofFlight, 20)
			.AddTile(TileID.MythrilAnvil)
			.Register();
		}
	}
}
