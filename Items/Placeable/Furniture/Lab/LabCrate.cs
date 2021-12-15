using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabCrate : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laboratory Crate");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabCrateTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.maxStack = 999;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            // TODO: Lab crate loot
        }
    }
}
