using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Armor.Vanity.TBot
{
    [AutoloadEquip(EquipType.Body)]
    class JanitorOutfit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Janitor Outfit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Body.Sets.HidesArms[Mod.GetEquipSlot(Name, EquipType.Body)] = true;
            ArmorIDs.Body.Sets.HidesBottomSkin[Mod.GetEquipSlot(Name, EquipType.Body)] = true;
            ArmorIDs.Body.Sets.HidesTopSkin[Mod.GetEquipSlot(Name, EquipType.Body)] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.vanity = true;
        }
    }
}
