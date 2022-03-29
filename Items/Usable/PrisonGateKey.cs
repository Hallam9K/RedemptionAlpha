using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class PrisonGateKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Unlocks gates in the Soulless Prison");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 30;
        }
    }
}