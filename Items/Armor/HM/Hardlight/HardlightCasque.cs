using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Redemption.DamageClasses;
using Redemption.BaseExtension;

namespace Redemption.Items.Armor.HM.Hardlight
{
    [AutoloadEquip(EquipType.Head)]
    public class HardlightCasque : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("13% increased ritual damage\n" +
            "5% increased ritual critical strike chance\n" +
            "+2 max spirit level"); */

            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RitualistClass>() += .13f;
            player.GetCritChance<RitualistClass>() += 5;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HardlightPlate>() && legs.type == ModContent.ItemType<HardlightBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Keybind"); // TODO: Hardlight ritualist bonus
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Press") + key + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Support") +
                    Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Casque");
            }
            player.RedemptionPlayerBuff().hardlightBonus = 1;
            player.RedemptionPlayerBuff().MetalSet = true;
        }
    }
}
