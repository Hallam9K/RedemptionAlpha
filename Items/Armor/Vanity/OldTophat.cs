using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class OldTophat : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 14;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override bool DrawHead() => true;
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair) => drawAltHair = true;
    }
}