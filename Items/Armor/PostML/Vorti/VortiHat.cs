using Terraria;
using Terraria.ModLoader;
using Redemption.Rarities;
using Terraria.ID;
using Redemption.BaseExtension;

namespace Redemption.Items.Armor.PostML.Vorti
{
    [AutoloadEquip(EquipType.Head)]
    public class VortiHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("18% increased magic damage"
                + "\n12% increased magic critical strike chance"
                + "\n+150 max mana"); */

            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
            ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 22;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 28;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += .18f;
            player.GetCritChance<MagicDamageClass>() += 12;
            player.statManaMax2 += 150;
            player.RedemptionPlayerBuff().vortiHead = true;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VortiRobes>() && legs.type == ModContent.ItemType<VortiPants>();
        }
    }
}