using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.Items.Armor.PreHM.DragonLead
{
    [AutoloadEquip(EquipType.Body)]
    public class DragonLeadRibplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon-Lead Ribplate");
            /* Tooltip.SetDefault("7% increased damage\n" +
                "Immunity to most ice-related debuffs"); */
            ArmorIDs.Body.Sets.IncludedCapeBack[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = Redemption.dragonLeadCapeID;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = Redemption.dragonLeadCapeID;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 28;
            Item.sellPrice(0, 0, 65);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .07f;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[ModContent.BuffType<PureChillDebuff>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 20)
                .AddIngredient(ItemID.Bone, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.DragonLead.DragonLeadRibplate"))
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
