using Terraria;
using Terraria.ModLoader;

namespace Redemption.UI.ChatUI
{
    // Server side dialogue updater
    public class ChatUISystem : ModSystem
    {
        public override void Load()
        {
            if (Main.dedServ)
                ChatUI.Dialogue = new();
        }

        public override void PreUpdateEntities()
        {
            if (Main.dedServ && ChatUI.Visible && ChatUI.Dialogue.Count > 0)
            {
                for (int i = 0; i < ChatUI.Dialogue.Count; i++)
                {
                    IDialogue dialogue = ChatUI.Dialogue[i];
                    dialogue.Update(Main.gameTimeCache);
                }
            }
        }
    }
}
