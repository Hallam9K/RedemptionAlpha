using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ThornMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<ThornMask2>();
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class ThornMask2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<ThornMask>();
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }
    }
}