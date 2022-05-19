using Microsoft.Xna.Framework.Graphics;
using Redemption.Tiles.Tiles;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Redemption.Tiles.Trees
{
    public class IrradiatedCactus : ModCactus
	{
        public override void SetStaticDefaults()
        {
			GrowsOnTileId = new int[1] { ModContent.TileType<IrradiatedSandTile>() };
		}
		public override Asset<Texture2D> GetTexture()
		{
			return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/IrradiatedCactus");
		}
		// This would be where the Cactus Fruit Texture would go, if we had one.
		public override Asset<Texture2D> GetFruitTexture()
		{
			return null;
		}
    }
}