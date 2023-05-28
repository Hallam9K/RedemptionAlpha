using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class DurableBowString : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Durable Bow String");
            // Tooltip.SetDefault("20% increased velocity of arrow-based weaponry");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<LeatherSheath>();
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().bowString = true;
        }
    }
}