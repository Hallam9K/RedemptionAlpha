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
    public class IrradiatedMahoganyTree : ModTree
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
            GrowsOnTileId = new int[1] { TileType<IrradiatedJungleGrassTile>() };
        }
        public override bool Shake(int x, int y, ref bool createLeaves) => false;
        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            xoffset = 48;
            topTextureFrameWidth = 114;
            topTextureFrameHeight = 96;
        }
        public override Asset<Texture2D> GetTexture()
        {
            return Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedMahoganyTree");
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return TileType<IrradiatedPuritySapling>();
        }
        public override Asset<Texture2D> GetBranchTextures()
        {
            return Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedMahoganyTree_Branch");
        }
        public override Asset<Texture2D> GetTopTextures()
        {
            return Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedMahoganyTree_Top");
        }
        public override int DropWood() => ItemType<PetrifiedWood>();
        public override int CreateDust() => DustID.Ash;
        public override int TreeLeaf() => Find<ModGore>("Redemption/DeadTreeFX").Type;
    }
}