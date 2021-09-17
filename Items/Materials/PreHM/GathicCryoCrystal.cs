using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class GathicCryoCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            Tooltip.SetDefault("'A freezing cold crystal'"
                + "\nMakes the player Chilled when held");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
        }

        public override void HoldItem(Player player)
        {
            player.AddBuff(BuffID.Chilled, 60);
        }
    }
}
