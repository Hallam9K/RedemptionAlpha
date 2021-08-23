using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Materials.PreHM
{
    public class Archcloth : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Expensive, purple cloth only used by the Nobles of Anglon'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;
        }
    }
}
