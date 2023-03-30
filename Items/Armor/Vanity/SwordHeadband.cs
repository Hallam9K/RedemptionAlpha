using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class SwordHeadband : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fake Knife Headband");
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 16;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }
    }
}