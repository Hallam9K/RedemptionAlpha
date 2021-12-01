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
            RedeBossDowned.skullDiggerSaved = false;
            RedeBossDowned.downedEaglecrestGolem = false;
            RedeBossDowned.downedErhan = false;
            RedeBossDowned.erhanDeath = 0;
            RedeBossDowned.downedSlayer = false;
            RedeBossDowned.slayerDeath = 0;
            RedeBossDowned.downedJanitor = false;
            RedeBossDowned.downedBehemoth = false;
            RedeBossDowned.downedBlisterface = false;
            RedeBossDowned.downedVolt = false;
            RedeBossDowned.downedMACE = false;
            RedeBossDowned.downedVlitch1 = false;
            RedeBossDowned.downedVlitch2 = false;
            RedeBossDowned.downedVlitch3 = false;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            return true;
        }
    }
}
