using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Waters
{
    public class RuinedKingdomWaterfallStyle : ModWaterfallStyle
    {
        public override void AddLight(int i, int j) =>
            Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Cyan.ToVector3() * 0.5f);
    }
}