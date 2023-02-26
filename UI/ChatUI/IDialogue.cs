using Microsoft.Xna.Framework;

namespace Redemption.UI.ChatUI
{
	public interface IDialogue
	{
		public void Update(GameTime gameTime);
		public Dialogue Get();
	}
}
