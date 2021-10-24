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
    [AutoloadEquip(EquipType.Body)]
    internal class ArchePatreonVanityBody : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iridescent Outfit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 0, 5, 0);
			Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Archcloth>(), 7)
			.AddIngredient(ModContent.ItemType<MoonflareFragment>(), 4)
			.AddTile(TileID.Loom)
			.Register();
		}
	}
}
