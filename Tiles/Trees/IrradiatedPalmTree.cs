using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Tiles;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Trees
{
    public class IrradiatedPalmTree : ModPalmTree
    {
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };
        public override void SetStaticDefaults()
        {
            GrowsOnTileId = new int[1] { ModContent.TileType<IrradiatedSandTile>() };
        }
        // This is the primary texture for the trunk. Branches and foliage use different settings.
        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedPalmTree");
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 1;
            return ModContent.TileType<IrradiatedPalmSapling>();
        }
        public override Asset<Texture2D> GetOasisTopTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedPalmTree_Top");
        }
        public override Asset<Texture2D> GetTopTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedPalmTree_Top");
        }
        public override int DropWood()
        {
            return ModContent.ItemType<PetrifiedWood>();
        }
        public override int CreateDust()
        {
            return DustID.Ash;
        }
    }
}
