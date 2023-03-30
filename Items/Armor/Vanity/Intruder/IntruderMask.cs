using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Intruder
{
    [AutoloadEquip(EquipType.Head)]
    public class IntruderMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Intruder's Mask");
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.rare = ItemRarityID.LightRed;
            Item.vanity = true;
        }
    }
}