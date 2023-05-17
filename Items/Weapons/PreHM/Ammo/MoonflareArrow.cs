using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Items.Weapons.PreHM.Ammo
{
    public class MoonflareArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Moonflare Arrow");
            /* Tooltip.SetDefault("Burns targets while the moon is out" +
				"\nFlame intensity is based on moon phase"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerArrow;
            Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 14;
			Item.height = 34;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 2.5f;
			Item.value = 2;
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<MoonflareArrow_Proj>();
			Item.shootSpeed = 7f;
			Item.ammo = AmmoID.Arrow;
			if (!Main.dedServ)
				Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string text = "There is no moonlight to reflect...";
			if (Main.dayTime || Main.moonPhase == 4)
			{
				TooltipLine line = new(Mod, "text", text)
				{
					OverrideColor = Color.LightGray
				};
				tooltips.Insert(2, line);
			}
		}
		public override void AddRecipes()
		{
			CreateRecipe(20)
				.AddIngredient(ItemID.WoodenArrow, 20)
				.AddIngredient(ModContent.ItemType<MoonflareFragment>())
				.AddTile(TileID.WorkBenches)
				.AddCondition(RedeConditions.InMoonlight)
				.Register();
		}
	}
}
