using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Armor.Vanity.Joe
{
    [AutoloadEquip(EquipType.Body)]
	class JoesShirt : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Joe's Bardic Shirt");
            ArmorIDs.Body.Sets.HidesHands[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = false;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.vanity = true;
		}
	}
}
