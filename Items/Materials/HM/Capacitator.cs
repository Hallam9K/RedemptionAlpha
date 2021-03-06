using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Capacitator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Capacitator");
            Tooltip.SetDefault("'Holds a high amount of energy'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.maxStack = 30;
            Item.value = 650000;
            Item.rare = ItemRarityID.Lime;
        }
    }
}