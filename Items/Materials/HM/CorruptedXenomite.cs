using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class CorruptedXenomite : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Infects mechanical things'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Red;
        }
        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 5)
                .AddIngredient(ModContent.ItemType<OmegaBattery>())
                .AddTile(ModContent.TileType<GirusCorruptorTile>())
                .Register();
        }
    }
}
