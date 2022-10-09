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

            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(copper: 50);
            Item.rare = ItemRarityID.White;
        }
    }
}