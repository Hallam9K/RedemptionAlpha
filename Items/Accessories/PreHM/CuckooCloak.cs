using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class CuckooCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 0, 70, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.vanity = true;
        }
    }
}
