using Terraria;
using Terraria.ModLoader;
using Redemption.Rarities;

namespace Redemption.Items.Armor.PostML.Shinkite
{
    [AutoloadEquip(EquipType.Legs)]
    public class ShinkiteLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 24;
        }
    }
}