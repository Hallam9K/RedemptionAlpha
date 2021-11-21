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
    public class HardlightRegalia : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("13% increased druidic damage\n" +
            "5% increased druidic critical strike chance");

            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;
            Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<DruidClass>() += .13f;
            player.GetCritChance<DruidClass>() += 5;
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
            player.setBonus = "Select a keybind for [Special Ability Key] in Controls"; // TODO: Hardlight ritualist bonus
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = "Press " + key + " to get support from the Ship of the Slayer\n" +
                    "Summons a drone containing a fungus bio-weapon to poison the enemy";
            }
            player.GetModPlayer<BuffPlayer>().hardlightBonus = 5;
        }
    }
}