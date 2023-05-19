using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class KeepGateKeys : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Keep Key");
            //Tooltip.SetDefault("Used to open a gate to the Keep");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 36;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}