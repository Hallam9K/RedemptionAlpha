using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Single
{
    [AutoloadEquip(EquipType.Head)]
    public class JollyHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunset Helm");
            Tooltip.SetDefault("'Comes from an ashen world...'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.value = 7500;
            Item.rare = -1;
            Item.defense = 4;
        }
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawHair = drawAltHair = false;
        }
    }
}