using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class InfectedMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Become infected... Cosmetically'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 12;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override bool DrawHead() => true;
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair) => drawHair = false;
    }
}