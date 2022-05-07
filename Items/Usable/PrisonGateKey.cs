using Redemption.Globals;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class PrisonGateKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Unlocks gates in the Soulless Prison");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 30;
        }
        public override bool OnPickup(Player player)
        {
            if (SoullessArea.soullessBools[0])
                return true;

            SoullessArea.soullessBools[0] = true;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }
}