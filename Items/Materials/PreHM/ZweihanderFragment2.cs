using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class ZweihanderFragment2 : ModItem
    {
        public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Rusty Zweihander Blade");
            Tooltip.SetDefault("'A piece of a strange weapon...'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 42;
            Item.height = 42;
            Item.maxStack = 30;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
        }
    }
}
