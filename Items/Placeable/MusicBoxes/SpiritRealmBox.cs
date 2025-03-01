using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class SpiritRealmBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SpiritRealm"), ItemType<SpiritRealmBox>(), TileType<SpiritRealmBoxTile>());
            MusicID.Sets.SkipsVolumeRemap[MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SpiritRealm")] = false;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<SpiritRealmBoxTile>(), 0);
            Item.width = 26;
            Item.height = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient<LostSoul>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
