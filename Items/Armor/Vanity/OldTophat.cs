using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class OldTophat : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 14;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }
}