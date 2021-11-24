using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Materials.HM;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
	public class AntiCrystallizer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Anti-Crystallizer Band");
			Tooltip.SetDefault("Makes you immune to the Xenomite infection");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 30;
			Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<CrystalSerum>(), 8)
			.AddIngredient(ModContent.ItemType<XenomiteItem>(), 4)
			.AddIngredient(ModContent.ItemType<StarliteBar>(), 6)
			.AddTile(TileID.MythrilAnvil)
			.Register();
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.buffImmune[ModContent.BuffType<GreenRashesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<GlowingPustulesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<FleshCrystalsDebuff>()] = true;
        }
	}
}
