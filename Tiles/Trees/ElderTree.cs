using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Terraria.GameContent;
using Terraria;
using Redemption.Tiles.Tiles;
using ReLogic.Content;

namespace Redemption.Tiles.Trees
{
	public class ElderTree : ModTree
	{
		public override TreePaintingSettings TreeShaderSettings => new()
        {
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};
		public override void SetStaticDefaults()
		{
			GrowsOnTileId = new int[1] { ModContent.TileType<AncientDirtTile>() };
		}
		public override Asset<Texture2D> GetTexture()
		{
			return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/ElderTree");
		}

		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<ElderSapling>();
		}
		public override void SetTreeFoliageSettings(Tile tile, int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
		{
			// This is where fancy code could go, but let's save that for an advanced example
		}
		public override Asset<Texture2D> GetBranchTextures()
		{
			return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/ElderTree_Branches");
		}
		public override Asset<Texture2D> GetTopTextures()
		{
			return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/ElderTree_Top");
		}
		public override int DropWood() => ModContent.ItemType<ElderWood>();
		public override int CreateDust() => DustID.t_BorealWood;
		public override int GrowthFXGore()
		{
			return ModContent.Find<ModGore>("Redemption/ElderTreeFX").Type;
		}
	}
}