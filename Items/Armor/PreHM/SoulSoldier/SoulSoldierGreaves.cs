using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Armor.PreHM.SoulSoldier
{
    [AutoloadEquip(EquipType.Legs)]
	public class SoulSoldierGreaves : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 14;
            Item.sellPrice(silver: 30);
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GraveSteelAlloy>(16)
                .AddIngredient<LostSoul>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}