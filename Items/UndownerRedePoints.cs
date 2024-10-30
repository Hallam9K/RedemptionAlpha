using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items
{
    public class UndownerRedePoints : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Alignment Resetter");
            /* Tooltip.SetDefault("Sets alignment to 0" +
                "\nNon-Consumable"); */
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                 RedeWorld.Alignment = 0;

            return true;
        }
    }
}
