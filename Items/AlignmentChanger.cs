using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Microsoft.Xna.Framework;

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
            if (player.altFunctionUse == 2)
                RedeWorld.alignment -= 1;
            else
                RedeWorld.alignment += 1;

            CombatText.NewText(player.Hitbox, Color.DarkGoldenrod, RedeWorld.alignment, true, false);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }
}
