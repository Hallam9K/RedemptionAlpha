using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;

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
            RedeWorld.alignment = 0;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            return true;
        }
    }
}
