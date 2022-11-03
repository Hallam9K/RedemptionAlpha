using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;

namespace Redemption.Items.Armor.PreHM.DragonLead
{
    [AutoloadEquip(EquipType.Head)]
    public class DragonLeadSkull : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon-Lead Skull");
            Tooltip.SetDefault("7% increased damage");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            SacrificeTotal = 1;
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
            player.RedemptionPlayerBuff().ElementalResistance[3] += 0.2f;
            player.RedemptionPlayerBuff().dragonLeadBonus = true;
            player.RedemptionPlayerBuff().MetalSet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 10)
                .AddIngredient(ItemID.Bone, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'A horned helmet forged from melted dragon bone and metal, made to look like a dragon's skull. It is said\n" +
                    "to be used by the ancient warlords of Dragonrest.\n" +
                    "The warlords were famous dragon slayers who used the bones of their victims for weaponry and armour,\n" +
                    "nearly bringing the dragons to extinction. That was until every single one was wiped out by Goliathon, the Dragon God.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}