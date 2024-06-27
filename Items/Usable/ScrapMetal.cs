using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.Items.Weapons.HM.Ranged;
using Terraria;
using Terraria.GameContent.ItemDropRules;
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

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Plating>(), 2, 1, 2));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Capacitor>(), 2, 1, 2));
            itemLoot.Add(ItemDropRule.Common(ItemID.EmptyBucket, 75));
            itemLoot.Add(ItemDropRule.Common(ItemID.IllegalGunParts, 75));
            itemLoot.Add(ItemDropRule.Common(ItemID.Handgun, 75));
            itemLoot.Add(ItemDropRule.Common(ItemID.ClockworkAssaultRifle, 75));
            itemLoot.Add(ItemDropRule.Common(ItemID.FlareGun, 75));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FoldedShotgun>(), 7));
            itemLoot.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ItemID.ProximityMineLauncher, 75));
            itemLoot.Add(ItemDropRule.ByCondition(new Conditions.BeatAnyMechBoss(), ItemID.Cog, 1, 4, 18));
        }

        public override void RightClick(Player player)
        {
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