using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Redemption.Rarities;
using Terraria.ID;

namespace Redemption.Items.Armor.PostML.Shinkite
{
    [AutoloadEquip(EquipType.Head)]
    public class ShinkiteHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 26;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ShinkiteChestplate>() && legs.type == ModContent.ItemType<ShinkiteLeggings>();
        }
        public override void UpdateEquip(Player player)
        {
            ModContent.GetInstance<ShinkiteArmorDraw>().shinkiteHead = true;
        }
    }
}