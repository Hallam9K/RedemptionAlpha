using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class CorpseWalkerSkullVanity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpse-Walker Skull");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool DrawHead() => false;
        public override void DrawHair(ref bool drawHair, ref bool drawAltHair) => drawHair = drawAltHair = false;
    }
}