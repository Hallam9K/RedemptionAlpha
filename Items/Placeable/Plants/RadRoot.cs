using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Plants
{
    public class RadRoot : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Grows in the Corrupted or Crimson Wasteland");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
		{
			Item.maxStack = 99;
            Item.width = 16;
            Item.height = 20;
            Item.value = 150;
            Item.rare = ItemRarityID.Blue;
		}
	}
}