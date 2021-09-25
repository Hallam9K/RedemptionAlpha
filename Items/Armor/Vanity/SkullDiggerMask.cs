using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class SkullDiggerMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull Digger's Mask");
            Tooltip.SetDefault("'Made of bone'");
            ArmorIDs.Head.Sets.DrawFullHair[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 18;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
    }
}