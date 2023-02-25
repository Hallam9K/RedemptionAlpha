using Terraria.ModLoader;

namespace Redemption.Globals.ILEdits
{
	public abstract class ILEdit
	{
		public abstract void Load(Mod mod);

		public virtual void Unload(Mod mod) { }
	}
}
