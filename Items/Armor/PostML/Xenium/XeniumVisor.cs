using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Lab;
using Redemption.BaseExtension;
using Redemption.Globals.Player;

namespace Redemption.Items.Armor.PostML.Xenium
{
    [AutoloadEquip(EquipType.Head)]
    public class XeniumVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("10% increased damage"
                + "\n15% increased critical strike chance"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.defense = 22;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<XeniumBreastplate>() && legs.type == ModContent.ItemType<XeniumLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += .10f;
            player.GetCritChance<GenericDamageClass>() += 15;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenium.Keybind");
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenium.Bonus1") +
                    Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenium.Press") + key + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenium.Bonus2");
            }
            player.RedemptionPlayerBuff().xeniumBonus = true;
            player.GetModPlayer<EnergyPlayer>().energyRegen += 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<XeniumAlloy>(), 12)
            .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 4)
            .AddTile(ModContent.TileType<XeniumRefineryTile>())
            .Register();
        }
    }
}
