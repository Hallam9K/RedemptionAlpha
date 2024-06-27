using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Tiles;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Trees
{
    public class IrradiatedPurityTree : ModTree
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
            GrowsOnTileId = new int[3] { ModContent.TileType<IrradiatedGrassTile>(), ModContent.TileType<IrradiatedCorruptGrassTile>(), ModContent.TileType<IrradiatedCrimsonGrassTile>() };
        }
        public override bool Shake(int x, int y, ref bool createLeaves) => false;
        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
        }
        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedPurityTree");
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<IrradiatedPuritySapling>();
        }
        public override Asset<Texture2D> GetBranchTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedPurityTree_Branch");
        }
        public override Asset<Texture2D> GetTopTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedPurityTree_Top");
        }
        public override int DropWood() => ModContent.ItemType<PetrifiedWood>();
        public override int CreateDust() => DustID.Ash;
        public override int TreeLeaf() => ModContent.Find<ModGore>("Redemption/DeadTreeFX").Type;
    }
}