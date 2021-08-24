using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Materials.PreHM
{
    public class CursedGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'A gem... or an eye?'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Lime;
        }
        public override void HoldItem(Player player)
        {
            player.AddBuff(BuffID.Weak, 10);
        }
    }
}
