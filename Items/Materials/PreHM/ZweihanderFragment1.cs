using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class ZweihanderFragment1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Zweihander Hilt");
            Tooltip.SetDefault("'A piece of a strange weapon...'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.maxStack = 30;
            Item.value = 0;
            Item.rare = -1;
        }
    }
}
