using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Wings)]
    public class TerrestrialWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terrestrial Wings");
			Tooltip.SetDefault("Allows flight and slow fall");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);
        }
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 28;
			Item.value = Item.sellPrice(0, 8, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.canBePlacedInVanityRegardlessOfConditions = true;
			Item.accessory = true;
		}
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.85f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.LunarBar, 10)
			.AddIngredient(ModContent.ItemType<TerrestrialFragment>(), 14)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
		}
	}
}
