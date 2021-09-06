using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class KeepersVeil : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Keeper's Veil");
            Tooltip.SetDefault("'For a wedding that never came to be...'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override bool DrawHead() => false;
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair) => drawHair = drawAltHair = false;
    }
}