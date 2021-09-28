using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using System;

namespace Redemption.Items.Armor.PreHM
{
    [AutoloadEquip(EquipType.Head)]
    public class DragonLeadSkull : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon-Lead Skull");
            Tooltip.SetDefault("7% increased damage");
            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.sellPrice(silver: 60);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DragonLeadRibplate>() && legs.type == ModContent.ItemType<DragonLeadGreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased Ice elemental resistance\n" +
                "33% chance for weapons to inflict Dragonblaze\n" +
                "100% chance for all Dragon-Lead weapons to inflict Dragonblaze";
            player.GetModPlayer<BuffPlayer>().ElementalResistance[3] += 0.2f;
            player.GetModPlayer<BuffPlayer>().dragonLeadBonus = true;
            player.GetModPlayer<BuffPlayer>().MetalSet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                //.AddIngredient(ModContent.ItemType<PureIronBar>(), 10)
                .AddIngredient(ItemID.Bone, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "''")
                {
                    overrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    overrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}