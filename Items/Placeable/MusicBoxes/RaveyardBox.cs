using Redemption.Items.Usable;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class RaveyardBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Raveyard"), ModContent.ItemType<RaveyardBox>(), ModContent.TileType<RaveyardBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RaveyardBoxTile>(), 0);
            Item.createTile = ModContent.TileType<RaveyardBoxTile>();
            Item.width = 32;
            Item.height = 16;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient(ModContent.ItemType<Trumpet>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
