using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace Redemption.Items.Armor.Vanity.TBot
{
    [AutoloadEquip(EquipType.Body)]
    class AndroidArmour : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Android Armour");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Body.Sets.HidesArms[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 18;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.vanity = true;
        }
    }
}
