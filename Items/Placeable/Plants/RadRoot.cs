using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Placeable.Plants
{
    public class RadRoot : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Grows in the Wasteland");
            SacrificeTotal = 25;
        }
        public override void SetDefaults()
		{
			Item.maxStack = 9999;
            Item.width = 16;
            Item.height = 20;
            Item.value = 150;
            Item.rare = ItemRarityID.Blue;
		}
	}
}