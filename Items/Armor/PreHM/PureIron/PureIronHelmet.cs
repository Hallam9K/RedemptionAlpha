using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Base;

namespace Redemption.Items.Armor.PreHM.PureIron
{
    [AutoloadEquip(EquipType.Head)]
    public class PureIronHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure-Iron Helmet");
            Tooltip.SetDefault("7% increased damage");
            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.sellPrice(silver: 60);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PureIronChestplate>() && legs.type == ModContent.ItemType<PureIronGreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased Fire elemental resistance\n" +
                "33% chance for weapons to inflict Pure Chill\n" +
                "100% chance for all Pure-Iron weapons to inflict Pure Chill";
            player.RedemptionPlayerBuff().ElementalResistance[1] += 0.2f;
            player.RedemptionPlayerBuff().pureIronBonus = true;
            player.RedemptionPlayerBuff().MetalSet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 10)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'A visored helmet used by the Warriors of the Iron Realm.\n" +
                    "The metal emits a constant chill mist and is cold to the touch,\n" +
                    "however the Iron Realm's warriors have been trained to resist such harsh temperatures.\n\n" +
                    "The Warriors of the Iron Realm are Gathuram's main military force,\n" +
                    "with units spanning all across the domain.'")
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