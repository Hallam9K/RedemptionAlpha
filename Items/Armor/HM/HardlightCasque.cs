using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using System;
using Redemption.Items.Materials.HM;
using Redemption.DamageClasses;

namespace Redemption.Items.Armor.HM
{
    [AutoloadEquip(EquipType.Head)]
    public class HardlightCasque : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("13% increased ritual damage\n" +
            "5% increased ritual critical strike chance\n" +
            "+2 max spirit level");

            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.Cyan;
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CyberPlating>(), 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "TBD"; // TODO: Hardlight set bonus
        }
    }
}