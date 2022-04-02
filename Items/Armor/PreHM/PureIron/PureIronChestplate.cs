using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.PureIron
{
    [AutoloadEquip(EquipType.Body)]
    public class PureIronChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure-Iron Chestplate");
            Tooltip.SetDefault("7% increased damage\n" +
                "Immunity to most fire-related debuffs");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 30;
            Item.sellPrice(0, 0, 65);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 9;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .07f;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 20)
                .AddIngredient(ItemID.Leather, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'A chestplate made of metal and leather used by the Warriors of the Iron Realm.\n" +
                    "The metal emits a constant chill mist and is cold to the touch,\n" +
                    "however the Iron Realm's warriors have been trained to resist such harsh temperatures.\n\n" +
                    "The Warriors of the Iron Realm are Gathuram's main military force,\n" +
                    "with units spanning all across the domain.'")
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