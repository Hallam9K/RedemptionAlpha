using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class EpidotrianSkull : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 20;
            Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
        }
    }
}