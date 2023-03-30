using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Waters
{
	public class WastelandWaterfallStyle : ModWaterfallStyle
	{
        public override void AddLight(int i, int j) =>
            Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Green.ToVector3() * 0.5f);
    }
}