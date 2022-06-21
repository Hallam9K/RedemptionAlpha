using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Redemption.Rarities;
using Terraria.ID;
using Redemption.BaseExtension;

namespace Redemption.Items.Armor.PostML.Hikarite
{
    [AutoloadEquip(EquipType.Head)]
    public class HikariteHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 36;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 26;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HikariteBreastplate>() && legs.type == ModContent.ItemType<HikariteGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.RedemptionPlayerBuff().MetalSet = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.RedemptionPlayerBuff().hikariteHead = true;
        }
    }
}