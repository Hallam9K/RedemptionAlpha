using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Materials.PreHM
{
    public class CoastScarabShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 75);
            Item.rare = ItemRarityID.White;
        }
    }
}
