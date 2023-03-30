using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.Intruder
{
    [AutoloadEquip(EquipType.Legs)]
    public class IntruderPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Intruder's Armored Pants");
            ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 22;
            Item.rare = ItemRarityID.LightRed;
            Item.vanity = true;
        }
    }
}