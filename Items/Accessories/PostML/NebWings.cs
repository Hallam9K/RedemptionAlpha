using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    [AutoloadEquip(EquipType.Wings)]
    public class NebWings : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.CelestialS);
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<NebWings2>();
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(220, 7f, 3f, true, 6, 1.5f);
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 36;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.accessory = true;
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Celestial] += 0.15f;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.95f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.2f;
        }
        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 10;
            acceleration = .8f;
        }
    }
    [AutoloadEquip(EquipType.Wings)]
    public class NebWings2 : NebWings
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<NebWings>();
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(220, 7f, 3f, true, 6, 1.5f);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 70;
            Item.height = 58;
        }
    }
}