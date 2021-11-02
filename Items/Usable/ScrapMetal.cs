using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ScrapMetal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scrap Metal");
            Tooltip.SetDefault("'Surely I can get something useful from this scrap...'"
                + "\n{$CommonItemTooltip.RightClickToOpen}");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 42;
            Item.height = 28;
            Item.rare = -1;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            if (Main.rand.NextBool(2))
                player.QuickSpawnItem(ModContent.ItemType<Plating>(), Main.rand.Next(1, 2));

            if (Main.rand.NextBool(2))
                player.QuickSpawnItem(ModContent.ItemType<Capacitator>(), Main.rand.Next(1, 2));

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(ItemID.EmptyBucket);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(ItemID.IllegalGunParts);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(ItemID.Handgun);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(ItemID.ClockworkAssaultRifle);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(ItemID.BreakerBlade);

            if (Main.rand.NextBool(75))
                player.QuickSpawnItem(ItemID.ChainGuillotines);

            if (Main.rand.NextBool(100))
                player.QuickSpawnItem(ItemID.FlareGun);

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && Main.rand.NextBool(75))
                player.QuickSpawnItem(ItemID.ProximityMineLauncher);

            if (NPC.downedMechBossAny)
            {
                player.QuickSpawnItem(ItemID.Cog, Main.rand.Next(4, 19));
                switch (Main.rand.Next(2))
                {
                    case 0:
                        player.QuickSpawnItem(ItemID.AdamantiteBar, Main.rand.Next(2, 7));
                        break;
                    case 1:
                        player.QuickSpawnItem(ItemID.TitaniumBar, Main.rand.Next(2, 7));
                        break;
                }
            }
            switch (Main.rand.Next(2))
            {
                case 0:
                    player.QuickSpawnItem(ItemID.IronBar, Main.rand.Next(2, 7));
                    break;
                case 1:
                    player.QuickSpawnItem(ItemID.LeadBar, Main.rand.Next(2, 7));
                    break;
            }
        }
    }
}