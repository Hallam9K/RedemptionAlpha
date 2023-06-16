using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Armor.Single;

namespace Redemption.Items.Armor.PreHM.PureIron
{
    [AutoloadEquip(EquipType.Head)]
    public class PureIronHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Helmet");
            // Tooltip.SetDefault("7% increased damage");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AntiquePureIronHelmet>();
            Item.ResearchUnlockCount = 1;
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
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.PureIron.20Increased") + ElementID.FireS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance") +
                Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.PureIron.Bonus");
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Fire] += 0.2f;
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
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.PureIron.PureIronHelmet"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}
