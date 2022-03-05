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
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Crate");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabCrateTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.maxStack = 999;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
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
                ModContent.ItemType<Capacitator>(),
                ModContent.ItemType<Plating>()
            };
            int[] LabChestLoot3 = new int[]
            {
                ModContent.ItemType<CrystalSerum>(),
                ModContent.ItemType<CarbonMyofibre>(),
                ModContent.ItemType<XenomiteShard>()
            };
            player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), Utils.Next(Main.rand, LabChestLoot));
            player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), Utils.Next(Main.rand, LabChestLoot2), Main.rand.Next(1, 3));
            player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), Utils.Next(Main.rand, LabChestLoot3), Main.rand.Next(8, 12));

            if (Main.rand.NextBool(4))
                player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), Utils.Next(Main.rand, FloppyDiskLoot));

            if (Main.rand.NextBool(4))
                player.QuickSpawnItem(player.GetItemSource_OpenItem(Type), ItemID.GoldCoin, Main.rand.Next(2, 5));
        }
    }
}
