using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Misc
{
	public class DeadTreeFX : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			gore.velocity = new Vector2(Main.rand.NextFloat() - 0.5f, RedeHelper.RandomRotation());
			gore.Frame = new SpriteFrame(1, 8, 0, (byte)Main.rand.Next(8));
			gore.frameCounter = (byte)Main.rand.Next(8);
			UpdateType = 910;
		}
	}
}