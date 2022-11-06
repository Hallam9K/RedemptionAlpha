using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Armor.PreHM.CommonGuard
{
    [AutoloadEquip(EquipType.Legs)]
	public class CommonGuardGreaves : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("7% increased melee speed");

			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Green;
			Item.defense = 3;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetAttackSpeed(DamageClass.Melee) += .07f;
		}

		public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 15)
                .AddIngredient(ItemID.Silk, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Plate mail greaves of the Common Guard unit of Anglon that were scavenged by skeletons.\n" +
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