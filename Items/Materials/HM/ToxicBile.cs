using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class ToxicBile : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Xenomite>();
            Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(0, 0, 9, 0);
			Item.rare = ItemRarityID.LightRed;
		}
    }
}
