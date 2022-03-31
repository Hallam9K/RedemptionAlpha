using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Misc
{
	public class ElderTreeFX : ModGore
	{
		public override void OnSpawn(Gore gore)
		{
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() * MathHelper.TwoPi);
			gore.Frame = new SpriteFrame(1, 8, 0, (byte)Main.rand.Next(8));
			gore.frameCounter = (byte)Main.rand.Next(8);
			UpdateType = 910;
		}
	}
}