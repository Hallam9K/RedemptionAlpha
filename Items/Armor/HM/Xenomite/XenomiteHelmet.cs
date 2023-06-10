using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Redemption.BaseExtension;
using Redemption.Globals.Player;

namespace Redemption.Items.Armor.HM.Xenomite
{
    [AutoloadEquip(EquipType.Head)]
    public class XenomiteHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("7% increased damage\n" +
            "10% increased critical strike chance"); */

            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .07f;
            player.GetCritChance(DamageClass.Generic) += 10;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<XenomitePlate>() && legs.type == ModContent.ItemType<XenomiteLeggings>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 5)
                .AddIngredient(ModContent.ItemType<Materials.HM.Xenomite>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 5)
                .AddIngredient(ModContent.ItemType<Materials.HM.Xenomite>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenomite.Keybind");
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenomite.Bonus1") +
                    Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenomite.Press") + key + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenomite.Bonus2");
            }
            player.GetDamage<GenericDamageClass>() += .06f;
            player.GetModPlayer<EnergyPlayer>().energyRegen += 10;
            player.RedemptionPlayerBuff().xenomiteBonus = true;
        }
    }
}
