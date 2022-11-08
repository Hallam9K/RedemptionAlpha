using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class LeatherSheath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leather Sheath");
            Tooltip.SetDefault("The first hit landing on an enemy using a physical melee attack has a guaranteed critical strike");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().leatherSheath = true;
        }
    }
}