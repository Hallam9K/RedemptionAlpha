using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class EpidotraMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Epidotra"), ItemType<EpidotraMusicBox>(), TileType<EpidotraMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<EpidotraMusicBoxTile>(), 0);
            Item.createTile = TileType<EpidotraMusicBoxTile>();
            Item.width = 32;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
    }
}