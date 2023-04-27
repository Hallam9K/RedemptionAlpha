using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace Redemption.UI.ChatUI
{
    public class DialogueChain : IDialogue
    {
        public delegate void SymbolTrigger(Dialogue dialogue, string signature);
        public event SymbolTrigger OnSymbolTrigger;
        public delegate void EndTrigger(Dialogue dialogue, int id);
        public event EndTrigger OnEndTrigger;
        public List<Dialogue> Dialogue;
        public Entity anchor;
        public Vector2 modifier;

        public DialogueChain(Entity anchor = null, Vector2 modifier = default)
        {
            Dialogue = new List<Dialogue>();
            this.anchor = anchor;
            this.modifier = modifier;
        }
        public void Update(GameTime gameTime)
        {
            Dialogue[0].Update(gameTime);
            if (Dialogue.Count == 0)
                ChatUI.Dialogue.Remove(this);
        }
        public DialogueChain Add(Dialogue dialogue)
        {
            dialogue.chain = this;
            Dialogue.Add(dialogue);
            return this;
        }
        public Dialogue Get() => Dialogue[0];
        public void TriggerSymbol(string signature) => OnSymbolTrigger?.Invoke(Dialogue[0], signature);
        public void TriggerEnd(int ID) => OnEndTrigger?.Invoke(Dialogue[0], ID);
    }
}
