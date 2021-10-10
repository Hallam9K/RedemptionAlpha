using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Single
{
    [AutoloadEquip(EquipType.Head)]
    public class JollyHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunset Helm");
            Tooltip.SetDefault("'Comes from an ashen world'");
            ArmorIDs.Head.Sets.DrawFullHair[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.value = 7500;
            Item.rare = ItemRarityID.Gray;
            Item.defense = 4;
        }
    }
}