using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Tiles.Bars;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Materials.PreHM
{
    public class PureIronAlloy : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Alloy");
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 70;
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<PureIronAlloyTile>();
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(10))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.SilverCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .AddIngredient(ModContent.ItemType<GathicCryoCrystal>())
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
