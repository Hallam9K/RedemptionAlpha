using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ScrapMetal : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Scrap Metal");
            /* Tooltip.SetDefault("'Surely I can get something useful from this scrap...'"
                + "\n{$CommonItemTooltip.RightClickToOpen}"); */

            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 52;
            Item.height = 40;
            Item.rare = -1;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            if (Main.rand.NextBool(2))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<Plating>(), Main.rand.Next(1, 2));

            if (Main.rand.NextBool(2))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ModContent.ItemType<Capacitor>(), Main.rand.Next(1, 2));

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.EmptyBucket);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.IllegalGunParts);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.Handgun);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.ClockworkAssaultRifle);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.BreakerBlade);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.ChainGuillotines);

            if (Main.rand.NextBool(100))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.FlareGun);

            if (NPC.downedPlantBoss && Main.rand.NextBool(75))
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.ProximityMineLauncher);

            if (NPC.downedMechBossAny)
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.Cog, Main.rand.Next(4, 19));
                switch (Main.rand.Next(2))
                {
                    case 0:
                        player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.AdamantiteBar, Main.rand.Next(2, 7));
                        break;
                    case 1:
                        player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.TitaniumBar, Main.rand.Next(2, 7));
                        break;
                }
            }
            switch (Main.rand.Next(2))
            {
                case 0:
                    player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.IronBar, Main.rand.Next(2, 7));
                    break;
                case 1:
                    player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemID.LeadBar, Main.rand.Next(2, 7));
                    break;
            }
        }
    }
}