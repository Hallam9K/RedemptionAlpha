using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class GraveSteelShards : ModItem
    {
        public override void SetStaticDefaults()
        {            
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 0, 2);
            Item.rare = ItemRarityID.Gray;
        }
    }
}
