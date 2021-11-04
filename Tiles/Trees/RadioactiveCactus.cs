using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Redemption.Tiles.Trees
{
    public class RadioactiveCactus : ModCactus
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
			return Mod.Assets.Request<Texture2D>("Tiles/Trees/RadioactiveCactus").Value;
		}
    }
}