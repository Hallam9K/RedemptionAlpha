using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    [AutoloadEquip(EquipType.Wings)]
	public class NebWings : ModItem
	{
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus Wings");
            /* Tooltip.SetDefault("Allows flight and slow fall"
                + "\nUse dyes to make it look fabulous!"); */
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(220, 7f, 2.5f, true, 6, 1.5f);
        }

        public override void SetDefaults()
		{
			Item.width = 38;
            Item.height = 36;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.accessory = true;
            Item.expert = true;
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Expert;
        }
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.95f;
			ascentWhenRising = 0.35f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 4f;
			constantAscend = 0.2f;
		}
        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 6;
            acceleration = 1f;
        }
    }
}