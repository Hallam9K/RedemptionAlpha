using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Tiles.Bars;

namespace Redemption.Items.Materials.PostML
{
    public class XeniumAlloy : ModItem
    {
        public override void SetStaticDefaults()
        { 
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<XeniumAlloyTile>(), 0);
            Item.width = 28;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Purple;
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(5))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.GreenTorch, 0, 0, 10);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<MoltenScrap>())
                .AddIngredient(ModContent.ItemType<RawXenium>(), 3)
                .AddTile(ModContent.TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}
