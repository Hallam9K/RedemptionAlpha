using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class ForestCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forest Core");
            Tooltip.SetDefault("Increased chance of Nature Boons to drop from the " + ElementID.NatureS + " elemental bonus\n" +
                "Nature Boons additionally increase critical strike chance for " + ElementID.NatureS + " weapons by 10%\n" +
                "Increased duration of Nature Boons' effect");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.value = Item.buyPrice(0, 6, 0, 0);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.forestCore = true;
        }
    }
}