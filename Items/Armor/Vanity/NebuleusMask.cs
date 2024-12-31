using Redemption.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class NebuleusMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<NebuleusMask2>();
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.vanity = true;
            Item.rare = RarityType<CosmicRarity>();
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class NebuleusMask2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<NebuleusMask>();
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.vanity = true;
            Item.rare = RarityType<CosmicRarity>();
        }
    }
}