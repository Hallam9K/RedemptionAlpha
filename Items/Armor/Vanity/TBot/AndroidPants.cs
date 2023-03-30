using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.TBot
{
    [AutoloadEquip(EquipType.Legs)]
    public class AndroidPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Legs.Sets.OverridesLegs[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.vanity = true;
        }
    }
}