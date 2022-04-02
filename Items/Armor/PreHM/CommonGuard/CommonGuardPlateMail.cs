using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Armor.PreHM.CommonGuard
{
    [AutoloadEquip(EquipType.Body)]
    public class CommonGuardPlateMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Reduces damage taken by 4%");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.sellPrice(0, 0, 35);
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.endurance += .04f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 20)
                .AddIngredient(ItemID.Silk, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Plate mail of the Common Guard unit of Anglon that were scavenged by skeletons.\n" +
                    "Originally shining steel, the metal has since dulled with time and coated with layers of dust.\n\n" +
                    "The Common Guard was founded when an Overlord's city was completely obliterated\n" +
                    "by a stray demon that sneaked through an unguarded portal to Demonhollow.\n\n" +
                    "They now guard cities and landmarks of great importance. Despite being stronger than the average\n" +
                    "knight, they don't get involved in wars.'")
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