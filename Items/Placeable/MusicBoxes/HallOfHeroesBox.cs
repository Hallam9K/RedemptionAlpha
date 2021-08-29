using Redemption.Tiles.MusicBoxes;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class HallOfHeroesBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Hall of Heroes)");

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/HallofHeroes"), ModContent.ItemType<HallOfHeroesBox>(), ModContent.TileType<HallOfHeroesBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<HallOfHeroesBoxTile>();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.LightRed;
            Item.value = 1;
            Item.accessory = true;
        }
    }
}