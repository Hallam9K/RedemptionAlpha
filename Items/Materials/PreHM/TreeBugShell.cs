using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class TreeBugShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tree Bug Shell");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 50);
            Item.rare = ItemRarityID.White;
        }
    }
}