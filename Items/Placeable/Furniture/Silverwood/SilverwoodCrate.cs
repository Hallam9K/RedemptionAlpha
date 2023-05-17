using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            ItemID.Sets.IsFishingCrate[Type] = true;
            ItemID.Sets.IsFishingCrateHardmode[Type] = true;
            SacrificeTotal = 10;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodCrateTile>());
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
        }
    }
}
