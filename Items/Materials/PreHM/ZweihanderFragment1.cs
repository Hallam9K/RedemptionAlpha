using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class ZweihanderFragment1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Zweihander Hilt");
            Tooltip.SetDefault("'A piece of a strange weapon...'");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.maxStack = 9999;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
        }
    }
}
