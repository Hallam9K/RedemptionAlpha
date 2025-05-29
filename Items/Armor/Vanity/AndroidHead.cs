using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AndroidHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<AndroidHead2>();
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.vanity = true;
            Item.rare = ItemRarityID.LightRed;
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class AndroidHead2 : AndroidHead
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<AndroidHead3>();
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class AndroidHead3 : AndroidHead
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<AndroidHead>();
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }
    }
}