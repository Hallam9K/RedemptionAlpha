using Redemption.Buffs.Debuffs;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Xenomite : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("'Infects living things'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
		{
			Item.width = 14;
            Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Pink;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenomiteShard>(), 4)
                .AddTile(ModContent.TileType<XeniumRefineryTile>())
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddCustomShimmerResult(ModContent.ItemType<ToxicBile>())
                .Register().DisableRecipe();
        }
        public override void HoldItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 10);
        }
    }
}
