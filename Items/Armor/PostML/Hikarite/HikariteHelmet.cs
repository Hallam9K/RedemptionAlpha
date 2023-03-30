using Terraria;
using Terraria.ModLoader;
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
            // DisplayName.SetDefault("Hikarite Greathelm");
            /* Tooltip.SetDefault("16% increased damage"
                + "\n9% increased critical strike chance"); */

            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 36;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 24;
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
            player.GetDamage<GenericDamageClass>() += .16f;
            player.GetCritChance<GenericDamageClass>() += 9;
            player.RedemptionPlayerBuff().hikariteHead = true;
        }
    }
}