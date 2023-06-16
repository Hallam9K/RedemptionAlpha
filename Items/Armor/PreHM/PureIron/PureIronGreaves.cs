using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using System.Collections.Generic;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Armor.PreHM.PureIron
{
    [AutoloadEquip(EquipType.Legs)]
    public class PureIronGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Greaves");
            // Tooltip.SetDefault("8% increased critical strike chance");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 15)
                .AddIngredient(ItemID.Leather, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.PureIron.PureIronGreaves"))
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
