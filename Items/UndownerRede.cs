using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Redemption.BaseExtension;

namespace Redemption.Items
{
    public class UndownerRede : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Redemption Undowner");
            /* Tooltip.SetDefault("Undowns all Redemption bosses" +
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
            RedeBossDowned.downedOmega1 = false;
            RedeBossDowned.downedOmega2 = false;
            RedeBossDowned.downedOmega3 = false;
            RedeBossDowned.voltBegin = false;
            RedeBossDowned.downedPZ = false;
            RedeBossDowned.downedNebuleus = false;
            RedeBossDowned.downedADD = false;
            RedeBossDowned.nebDeath = 0;
            RedeBossDowned.oblitDeath = 0;
            RedeBossDowned.ADDDeath = 0;
            RedeBossDowned.downedCalavia = false;
            player.Redemption().slayerStarRating = 0;
            player.RedemptionAbility().Spiritwalker = false;
            RedeQuest.wayfarerVars[0] = 1;
            RedeQuest.forestNymphVar = 0;
            RedeBossDowned.downedGGBossFirst = 0;
            RedeQuest.calaviaVar = 0;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            return true;
        }
    }
}
