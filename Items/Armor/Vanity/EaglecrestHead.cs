using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class EaglecrestHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eaglecrest Cowl");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 32;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }
    }
}