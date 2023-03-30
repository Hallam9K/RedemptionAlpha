using Redemption.Items.Accessories.HM;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Petrified Crate");
            // Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            ItemID.Sets.IsFishingCrate[Type] = true;
            ItemID.Sets.IsFishingCrateHardmode[Type] = true;
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedCrateTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
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
            int[] PrimaryLoot = new int[] { ModContent.ItemType<GasMask>(), ModContent.ItemType<DoubleRifle>(), ModContent.ItemType<DAN>(), ModContent.ItemType<GeigerMuller>() };
            int[] HazmatSuits = new int[] { ModContent.ItemType<HazmatSuit>(), ModContent.ItemType<HazmatSuit3>() };
            int[] OreLoot = new int[] { ItemID.CobaltOre, ItemID.PalladiumOre, ItemID.MythrilOre, ItemID.OrichalcumOre, ItemID.AdamantiteOre, ItemID.TitaniumOre };
            int[] BarLoot = new int[] { ItemID.CobaltBar, ItemID.PalladiumBar, ItemID.MythrilBar, ItemID.OrichalcumBar, ItemID.AdamantiteBar, ItemID.TitaniumBar };
            int[] PotionLoot = new int[] { ItemID.ObsidianSkinPotion, ItemID.SpelunkerPotion, ItemID.HunterPotion, ItemID.GravitationPotion, ItemID.MiningPotion, ItemID.HeartreachPotion, ItemID.StinkPotion };
            int[] PotionLoot2 = new int[] { ItemID.HealingPotion, ItemID.ManaPotion };
            int[] BaitLoot = new int[] { ItemID.JourneymanBait, ItemID.MasterBait };
            int[] MiscLoot = new int[]
            {
                ModContent.ItemType<CrystalSerum>(),
                ModContent.ItemType<ToxicBile>(),
                ModContent.ItemType<XenomiteShard>()
            };
            if (!Main.rand.NextBool(4))
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, PrimaryLoot));
            else
                player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, HazmatSuits));

            player.QuickSpawnItem(entitySource, Utils.Next(Main.rand, MiscLoot), Main.rand.Next(8, 13));

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
            if (Main.rand.NextBool(3))
                player.QuickSpawnItem(entitySource, ModContent.ItemType<Tiles.PetrifiedWood>(), Main.rand.Next(20, 51));
            if (Main.rand.NextBool(4))
                player.QuickSpawnItem(entitySource, ItemID.GoldCoin, Main.rand.Next(5, 13));
        }
    }
}
