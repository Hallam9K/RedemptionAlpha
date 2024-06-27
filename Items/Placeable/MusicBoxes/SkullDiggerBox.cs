using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class SkullDiggerBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SilentCaverns"), ModContent.ItemType<SkullDiggerBox>(), ModContent.TileType<SkullDiggerBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SkullDiggerBoxTile>(), 0);
            Item.createTile = ModContent.TileType<SkullDiggerBoxTile>();
            Item.width = 32;
            Item.height = 22;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient(ModContent.ItemType<LostSoul>(), 3)
                .AddIngredient(ModContent.ItemType<GrimShard>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
