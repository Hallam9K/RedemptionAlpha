using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ThornMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Looks painful...'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
        }

        public override bool DrawHead() => false;
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair) => drawHair = drawAltHair = false;
    }
}