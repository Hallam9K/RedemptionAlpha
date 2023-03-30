using Redemption.Items.Materials.HM;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class KSBox : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (King Slayer III)");
            // Tooltip.SetDefault("William 'GoukisanNG' Prevett - Betrayal of Fear");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossSlayer"), ModContent.ItemType<KSBox>(), ModContent.TileType<KSBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<KSBoxTile>(), 0);
			Item.createTile = ModContent.TileType<KSBoxTile>();
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<CyberPlating>(), 2)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
