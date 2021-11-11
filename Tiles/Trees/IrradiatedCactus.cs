using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Redemption.Tiles.Trees
{
    public class IrradiatedCactus : ModCactus
	{
		private static Mod Mod
		{
			get
			{
				return ModLoader.GetMod("Redemption");
			}
		}

		public override Texture2D GetTexture()
		{
			return Mod.Assets.Request<Texture2D>("Tiles/Trees/IrradiatedCactus").Value;
		}
    }
}