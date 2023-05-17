using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class PrisonGateKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Unlocks gates in the Soulless Prison");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
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
    public class PrisonGateKey2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Reinforced Prison Gate Key");
            // Tooltip.SetDefault("Unlocks reinforced gates in the Soulless Prison");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override bool OnPickup(Player player)
        {
            if (SoullessArea.soullessInts[1] is 5)
            {
                SoullessArea.soullessInts[1] = 6;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
                return true;
            }
            if (SoullessArea.soullessInts[1] > 1)
                return true;

            SoullessArea.soullessInts[1] = 2;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }
}