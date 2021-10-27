using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.Vanity.TBot
{
    [AutoloadEquip(EquipType.Legs)]
    public class TBotVanityLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("T-Bot Legs");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Legs.Sets.OverridesLegs[Mod.GetEquipSlot(Name, EquipType.Legs)] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.vanity = true;
        }
    }
}