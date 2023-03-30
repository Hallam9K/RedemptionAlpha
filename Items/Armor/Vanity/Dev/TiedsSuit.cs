using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Armor.Vanity.Dev
{
    [AutoloadEquip(EquipType.Body)]
	class TiedsSuit : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tied's Marvelous Suit");
            // Tooltip.SetDefault("'Great for impersonating devs!'");
            ArmorIDs.Body.Sets.HidesHands[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
			Item.width = 34;
            Item.height = 22;
            Item.rare = ItemRarityID.Cyan;
            Item.vanity = true;
		}
	}
}
