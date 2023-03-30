using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class DeadRinger : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Calls upon the spirits of corpses");
            Item.ResearchUnlockCount = 1;
        }

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
            if (!RedeWorld.deadRingerGiven)
            {
                RedeWorld.deadRingerGiven = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
}
