using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Armor.PostML.Hikarite
{
    [AutoloadEquip(EquipType.Legs)]
    public class HikariteGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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