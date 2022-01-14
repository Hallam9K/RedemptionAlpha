using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.DamageClasses;
using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Armor.PostML.Xenium
{
    [AutoloadEquip(EquipType.Head)]
    public class XeniumVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased damage"
                + "\n15% increased critical strike chance");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            player.setBonus = "The belch cannon on your shoulder will fire a poison gas that infects your enemies.";
            player.GetModPlayer<BuffPlayer>().xeniumBonus = true;
            ModContent.GetInstance<XeniumArmorDraw>().xeniumBonus = true;
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