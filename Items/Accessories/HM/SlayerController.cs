using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.HM
{
    public class SlayerController : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slayer's Gamer Controller");
            // Tooltip.SetDefault("Changes your cursor to a cyan crosshair");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.width = 30;
            Item.height = 26;
            Item.hasVanityEffects = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Main.playerInventory && !Main.inFancyUI && !Main.mapFullscreen)
                player.Redemption().slayerCursor = true;
        }
        public override void UpdateVanity(Player player)
        {
            if (!Main.playerInventory && !Main.inFancyUI && !Main.mapFullscreen)
                player.Redemption().slayerCursor = true;
        }
    }
}

