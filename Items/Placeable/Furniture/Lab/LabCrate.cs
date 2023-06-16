using Redemption.Items.Accessories.HM;
using Redemption.Items.Lore;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Crate");
            // Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            ItemID.Sets.IsFishingCrate[Type] = true;
            ItemID.Sets.IsFishingCrateHardmode[Type] = true;
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabCrateTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);
            int[] LabChestLoot = new int[]
{
                ModContent.ItemType<GasMask>(), ModContent.ItemType<Holoshield>(), ModContent.ItemType<PrototypeAtomRifle>(), ModContent.ItemType<MiniWarhead>(), ModContent.ItemType<GravityHammer>(), ModContent.ItemType<TeslaGenerator>(), ModContent.ItemType<LightningRod>()
};
            int[] FloppyDiskLoot = new int[]
            {
                ModContent.ItemType<FloppyDisk1>(),
                ModContent.ItemType<FloppyDisk3>(),
                ModContent.ItemType<FloppyDisk3_1>()
            };
            int[] LabChestLoot2 = new int[]
            {
                ModContent.ItemType<ScrapMetal>(),
                ModContent.ItemType<AIChip>(),
                ModContent.ItemType<Capacitor>(),
                ModContent.ItemType<Plating>()
            };
            int[] LabChestLoot3 = new int[]
            {
                ModContent.ItemType<CrystalSerum>(),
                ModContent.ItemType<CarbonMyofibre>(),
                ModContent.ItemType<XenomiteShard>()
            };
            int[] OreLoot = new int[] { ItemID.CobaltOre, ItemID.PalladiumOre, ItemID.MythrilOre, ItemID.OrichalcumOre, ItemID.AdamantiteOre, ItemID.TitaniumOre };
            int[] BarLoot = new int[] { ItemID.CobaltBar, ItemID.PalladiumBar, ItemID.MythrilBar, ItemID.OrichalcumBar, ItemID.AdamantiteBar, ItemID.TitaniumBar };
            int[] PotionLoot = new int[] { ItemID.ObsidianSkinPotion, ItemID.SpelunkerPotion, ItemID.HunterPotion, ItemID.GravitationPotion, ItemID.MiningPotion, ItemID.HeartreachPotion };
            int[] PotionLoot2 = new int[] { ItemID.HealingPotion, ItemID.ManaPotion };
            int[] BaitLoot = new int[] { ItemID.JourneymanBait, ItemID.MasterBait };
            player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, LabChestLoot));
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
                player.QuickSpawnItem(entitySource, ItemID.GoldCoin, Main.rand.Next(5, 13));
        }
    }
}
