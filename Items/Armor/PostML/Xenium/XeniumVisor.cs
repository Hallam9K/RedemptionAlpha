using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
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
            Tooltip.SetDefault("10% increased damage"
                + "\n15% increased critical strike chance");

            SacrificeTotal = 1;
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
            player.setBonus = "Increased Energy regeneration if an Energy Pack is in your inventory\n" +
                "Select a keybind for [Special Ability Key] in Controls";
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = "Increased Energy regeneration if an Energy Pack is in your inventory\n" +
                    "Press " + key + " to fire a virulent grenade from your shoulder launcher";
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