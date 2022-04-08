using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class AbyssBloskus : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 0, 82, 0);
            Item.maxStack = 999;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}