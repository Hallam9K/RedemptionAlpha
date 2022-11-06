using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Tiles.Furniture.Misc;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class WastelandTorch : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 100;
		}

		public override void SetDefaults()
		{
			Item.flame = true;
			Item.noWet = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.holdStyle = ItemHoldStyleID.HoldFront;
			Item.autoReuse = true;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<WastelandTorchTile>();
			Item.width = 10;
			Item.height = 12;
			Item.value = 50;
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches;
		}

		public override void HoldItem(Player player)
		{
			if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80))
			{
				Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<WastelandTorchDust>());
			}

			Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

			Lighting.AddLight(position, 0.85f, 0.7f, 0.7f);
		}

		public override void PostUpdate()
		{
			if (!Item.wet)
				Lighting.AddLight(Item.Center, 0.85f, 0.7f, 0.7f);
		}

		public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
		{
			dryTorch = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe(3)
				.AddIngredient(ItemID.Torch, 3)
				.AddIngredient(ModContent.ItemType<IrradiatedStone>())
				.Register();
			CreateRecipe(3)
				.AddIngredient(ItemID.Torch, 3)
				.AddIngredient(ModContent.ItemType<IrradiatedHardenedSand>())
				.Register();
			CreateRecipe(3)
				.AddIngredient(ItemID.Torch, 3)
				.AddIngredient(ModContent.ItemType<IrradiatedIce>())
				.Register();
		}
	}
}