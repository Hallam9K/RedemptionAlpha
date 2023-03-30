using Microsoft.Xna.Framework;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class CorruptedXenomite : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Infects mechanical things'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.6f * Main.essScale);
        }
        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ModContent.ItemType<Xenomite>(), 5)
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>())
                .AddTile(ModContent.TileType<GirusCorruptorTile>())
                .Register();
        }
    }
}
