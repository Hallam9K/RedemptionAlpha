using Redemption.Items.Usable.Potions;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class RadiationPillPlaceable : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 4;
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadiationPillTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RadiationPill>()
                .AddCondition(Condition.InGraveyard)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}