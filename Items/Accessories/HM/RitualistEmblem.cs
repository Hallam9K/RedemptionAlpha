using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.DamageClasses;

namespace Redemption.Items.Accessories.HM
{
    public class RitualistEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ritualist Emblem");
            // Tooltip.SetDefault("15% increased ritual damage");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RitualistClass>() *= 1.15f; 
        }
    }
}
