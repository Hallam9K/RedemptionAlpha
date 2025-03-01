using Redemption.Items.Usable;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class HallOfHeroesBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Music Box (Hall of Heroes)");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/HallofHeroes"), ItemType<HallOfHeroesBox>(), TileType<HallOfHeroesBoxTile>());
            MusicID.Sets.SkipsVolumeRemap[MusicLoader.GetMusicSlot(Mod, "Sounds/Music/HallofHeroes")] = false;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<HallOfHeroesBoxTile>(), 0);
            Item.createTile = TileType<HallOfHeroesBoxTile>();
            Item.width = 32;
            Item.height = 24;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient(ItemType<ChaliceFragments>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
