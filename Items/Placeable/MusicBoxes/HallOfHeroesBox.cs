using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class HallOfHeroesBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Hall of Heroes)");

			SacrificeTotal = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/HallofHeroes"), ModContent.ItemType<HallOfHeroesBox>(), ModContent.TileType<HallOfHeroesBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<HallOfHeroesBoxTile>(), 0);
			Item.createTile = ModContent.TileType<HallOfHeroesBoxTile>();
			Item.width = 32;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}
    }
}
