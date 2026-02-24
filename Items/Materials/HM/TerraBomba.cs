using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class TerraBombaTail : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(1);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class TerraBombaCore : TerraBombaTail
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 14;
            Item.height = 14;
        }
    }
    public class TerraBombaNose : TerraBombaTail
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 18;
            Item.height = 18;
        }
    }
}