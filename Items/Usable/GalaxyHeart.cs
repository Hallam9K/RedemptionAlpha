using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class GalaxyHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Permanently increases maximum life by 50"
                + "\nCan only be used if the max amount of life fruit has been consumed");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<CosmicRarity>();
        }
        public override bool CanUseItem(Player player)
        {
            RedePlayer modPlayer = player.GetModPlayer<RedePlayer>();
            if (modPlayer.galaxyHeart || player.statLifeMax < 500)
                return false;
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                if (Main.myPlayer == player.whoAmI)
                    player.HealEffect(50);
                RedePlayer modPlayer = player.GetModPlayer<RedePlayer>();
                modPlayer.galaxyHeart = true;
            }
            return true;
        }
    }
}