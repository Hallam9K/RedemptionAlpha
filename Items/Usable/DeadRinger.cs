using Redemption.BaseExtension;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class DeadRinger : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateInventory(Player player)
        {
            if (!RedeWorld.deadRingerGiven && player.whoAmI == Main.myPlayer)
            {
                RedeWorld.deadRingerGiven = true;
                RedeWorld.SyncData();
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            if (!player.RedemptionAbility().Spiritwalker)
                return;
            string text = Language.GetTextValue("Mods.Redemption.Items.DeadRinger.SpiritWalkerTooltip");
            TooltipLine line = new(Mod, "DeadRingerLine", text);
            tooltips.Insert(Item.favorited ? 4 : 2, line);
        }
    }
}
