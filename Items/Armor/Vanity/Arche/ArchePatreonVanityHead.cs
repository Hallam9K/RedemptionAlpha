using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Redemption.Items.Materials.PreHM;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Arche
{
	[AutoloadEquip(EquipType.Head)]
	public class ArchePatreonVanityHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iridescent Hat");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Mod.GetEquipSlot(Name, EquipType.Head)] = false;
        }
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 16;
			Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Archcloth>(), 5)
			.AddIngredient(ModContent.ItemType<MoonflareFragment>(), 2)
			.AddTile(TileID.Loom)
			.Register();
		}
	}
}
