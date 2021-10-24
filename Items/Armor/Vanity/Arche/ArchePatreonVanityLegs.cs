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
	[AutoloadEquip(EquipType.Legs)]
	public class ArchePatreonVanityLegs : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iridescent Leggings");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.rare = 1;
			Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Archcloth>(), 6)
			.AddIngredient(ModContent.ItemType<MoonflareFragment>(), 2)
			.AddTile(TileID.Loom)
			.Register();
		}
	}
}
