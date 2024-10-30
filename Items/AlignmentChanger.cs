using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items
{
    public class AlignmentChanger : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Alignment Changer");
            /* Tooltip.SetDefault("Left-click to increase alignment by 1\n" +
                "Right-click to decrease alignment by 1" +
                "\nNon-Consumable"); */
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 44;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.altFunctionUse == 2)
                    RedeWorld.SetAlignment(RedeWorld.Alignment - 1, Color.DarkGoldenrod);
                else
                    RedeWorld.SetAlignment(RedeWorld.Alignment + 1, Color.DarkGoldenrod);
            }
            return true;
        }
    }
}
