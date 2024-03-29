using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class FowlEmperorBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Music Box (Fowl Emperor)");
            // Tooltip.SetDefault("Sc0p3r - Fowl Play");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossFowl"), ModContent.ItemType<FowlEmperorBox>(), ModContent.TileType<FowlEmperorBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<FowlEmperorBoxTile>(), 0);
            Item.createTile = ModContent.TileType<FowlEmperorBoxTile>();
            Item.width = 32;
            Item.height = 24;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient(ModContent.ItemType<EggBomb>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}