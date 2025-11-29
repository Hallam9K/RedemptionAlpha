using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Face)]
    public class CircletOfBrambles : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.NatureS);
        public override void SetStaticDefaults()
        {
            ElementID.ItemPoison[Type] = true;
            ElementID.ItemNature[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().thornCirclet = true;
        }
    }
}
