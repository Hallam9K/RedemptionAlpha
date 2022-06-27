using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class Keycard : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Unlocks Laboratory Chests and Doors"
                + "\nOnly one is needed");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 22;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = 1;
        }
    }
}