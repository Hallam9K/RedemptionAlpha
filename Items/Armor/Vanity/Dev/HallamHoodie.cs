using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace Redemption.Items.Armor.Vanity.Dev
{
    [AutoloadEquip(EquipType.Body)]
	class HallamHoodie : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Halm's Casual Hoodie");
            // Tooltip.SetDefault("'Great for impersonating devs!'");
            ArmorIDs.Body.Sets.HidesHands[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = false;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
			Item.width = 30;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.expert = true;
            Item.vanity = true;
		}
	}
}
