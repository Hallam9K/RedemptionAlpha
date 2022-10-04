using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Lore;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabCrate2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reinforced Laboratory Crate");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            ItemID.Sets.IsFishingCrate[Type] = true;
            ItemID.Sets.IsFishingCrateHardmode[Type] = true;
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabCrate2Tile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.maxStack = 9999;
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);
            int[] LabChestLoot = new int[] { ModContent.ItemType<GasMask>(), ModContent.ItemType<Holoshield>(), ModContent.ItemType<PrototypeAtomRifle>(), ModContent.ItemType<MiniWarhead>(), ModContent.ItemType<GravityHammer>(), ModContent.ItemType<TeslaGenerator>(), ModContent.ItemType<LightningRod>() };
            int[] LabChestLoot4 = new int[] { ModContent.ItemType<HazmatSuit>(), ModContent.ItemType<MysteriousXenomiteFragment>(), ModContent.ItemType<EmptyMutagen>(), ModContent.ItemType<Hacksaw>(), ModContent.ItemType<DepletedCrossbow>() };
            int[] FloppyDiskLoot = new int[]
            {
                ModContent.ItemType<FloppyDisk1>(),
                ModContent.ItemType<FloppyDisk3>(),
                ModContent.ItemType<FloppyDisk3_1>(),
                ModContent.ItemType<FloppyDisk5>(),
                ModContent.ItemType<FloppyDisk5_1>(),
                ModContent.ItemType<FloppyDisk5_2>(),
                ModContent.ItemType<FloppyDisk5_3>()
            };
            int[] LabChestLoot2 = new int[]
            {
                ModContent.ItemType<ScrapMetal>(),
                ModContent.ItemType<AIChip>(),
                ModContent.ItemType<Capacitator>(),
                ModContent.ItemType<Plating>(),
                ModContent.ItemType<RawXenium>()
            };
            int[] LabChestLoot3 = new int[]
            {
                ModContent.ItemType<XenomiteShard>(),
                ItemID.LunarOre,
                ModContent.ItemType<Uranium>(),
                ModContent.ItemType<Plutonium>()
            };
            int[] OreLoot = new int[] { ItemID.CobaltOre, ItemID.PalladiumOre, ItemID.MythrilOre, ItemID.OrichalcumOre, ItemID.AdamantiteOre, ItemID.TitaniumOre };
            int[] BarLoot = new int[] { ItemID.CobaltBar, ItemID.PalladiumBar, ItemID.MythrilBar, ItemID.OrichalcumBar, ItemID.AdamantiteBar, ItemID.TitaniumBar };
            int[] PotionLoot = new int[] { ItemID.ObsidianSkinPotion, ItemID.SpelunkerPotion, ItemID.HunterPotion, ItemID.GravitationPotion, ItemID.MiningPotion, ItemID.HeartreachPotion };
            int[] PotionLoot2 = new int[] { ItemID.SuperHealingPotion, ItemID.SuperManaPotion };
            int[] BaitLoot = new int[] { ItemID.JourneymanBait, ItemID.MasterBait };
            if (Main.rand.NextBool(4))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, LabChestLoot));
            else
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, LabChestLoot4));
            player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, LabChestLoot2), Main.rand.Next(1, 3));
            player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, LabChestLoot3), Main.rand.Next(8, 12));

            if (Main.rand.NextBool(4))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, FloppyDiskLoot));

            if (Main.rand.NextBool(14))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, OreLoot), Main.rand.Next(30, 50));
            if (Main.rand.NextBool(6))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, BarLoot), Main.rand.Next(8, 21));
            if (Main.rand.NextBool(4))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, PotionLoot), Main.rand.Next(2, 5));
            if (Main.rand.NextBool(2))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, PotionLoot2), Main.rand.Next(5, 18));
            if (Main.rand.NextBool(2))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, BaitLoot), Main.rand.Next(2, 7));
            if (Main.rand.NextBool(4))
                player.QuickSpawnItem(entitySource, ItemID.GoldCoin, Main.rand.Next(8, 19));
        }
    }
}
