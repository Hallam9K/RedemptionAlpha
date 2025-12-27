using Redemption.Items.Materials.HM;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class BeyondSteelBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BeyondSteel"), ItemType<BeyondSteelBox>(), TileType<BeyondSteelBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<BeyondSteelBoxTile>(), 0);
            Item.createTile = TileType<BeyondSteelBoxTile>();
            Item.width = 32;
            Item.height = 22;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient(ItemType<CyberPlating>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
