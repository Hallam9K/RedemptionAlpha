using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Tiles;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria;

namespace Redemption.Tiles.Trees
{
    public class IrradiatedBorealTree : ModTree
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
			GrowsOnTileId = new int[1] { ModContent.TileType<IrradiatedSnowTile>() };
		}
		public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
		{
		}
		public override Asset<Texture2D> GetTexture()
		{
			return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedBorealTree");
		}
		public override bool Shake(int x, int y, ref bool createLeaves) => false;
		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<ElderSapling>();
		}
		public override Asset<Texture2D> GetBranchTextures()
		{
			return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedBorealTree_Branch");
		}
		public override Asset<Texture2D> GetTopTextures()
		{
			return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedBorealTree_Top");
		}
		public override int DropWood() => ModContent.ItemType<PetrifiedWood>();
		public override int CreateDust() => DustID.Ash;
	}
}