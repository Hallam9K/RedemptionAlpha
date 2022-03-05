using Redemption.Items.Materials.HM;
using Redemption.Tiles.MusicBoxes;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class OmegaBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Omega Prototypes)");
			Tooltip.SetDefault("musicman - Armageddon Interface");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega1"), ModContent.ItemType<OmegaBox>(), ModContent.TileType<OmegaBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<OmegaBoxTile>(), 0);
			Item.createTile = ModContent.TileType<OmegaBoxTile>();
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<OmegaBattery>(), 2)
                .AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}

	public class OmegaBox2 : ModItem
	{
		public override string Texture => "Redemption/Items/Placeable/MusicBoxes/OmegaBox";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Omega Obliterator)");
			Tooltip.SetDefault("Nick Nuwe - Chaos Attack");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega2"), ModContent.ItemType<OmegaBox2>(), ModContent.TileType<OmegaBoxTile2>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<OmegaBoxTile2>(), 0);
			Item.createTile = ModContent.TileType<OmegaBoxTile2>();
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
