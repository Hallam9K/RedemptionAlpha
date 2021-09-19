using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;

namespace Redemption.Items
{
    public class UndownerRede : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Redemption Undowner");
            Tooltip.SetDefault("Undowns all Redemption bosses" +
                "\nNon-Consumable");
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
            RedeBossDowned.downedThorn = false;
            RedeBossDowned.downedKeeper = false;
            RedeBossDowned.downedSkullDigger = false;
            RedeBossDowned.downedSeed = false;
            RedeBossDowned.keeperSaved = false;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            return true;
        }
    }
}
