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
            ItemID.Sets.Torches[Type] = true;
            ItemID.Sets.SingleUseInGamepad[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
            Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
        {
            Item.DefaultToTorch(ModContent.TileType<WastelandTorchTile>(), 0, false);
            Item.value = 50;
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
