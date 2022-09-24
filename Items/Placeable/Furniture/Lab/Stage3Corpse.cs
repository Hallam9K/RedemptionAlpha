using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class Stage3Corpse : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystallized Corpse");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Stage3CorpseTile>(), 0);
            Item.width = 32;
            Item.height = 34;
            Item.maxStack = 99;
            Item.value = 500;
            Item.rare = ItemRarityID.Green;
        }
    }
}