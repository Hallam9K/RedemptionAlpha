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
	[AutoloadEquip(EquipType.Back)]
	public class ArchePatreonVanityCape : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iridescent Cape");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 22;
			Item.sellPrice(0, 0, 5, 0);
			Item.accessory = true;
			Item.vanity = true;
            Item.rare = ModContent.RarityType<DonatorRarity>();
        }
		public override void AddRecipes()
		{
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Archcloth>(), 5)
            .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 3)
            .AddTile(TileID.Loom)
            .Register();
		}
	}
}
