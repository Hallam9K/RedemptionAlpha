using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class SpiritWalkerButton : UIState
    {
        private readonly UIImage ButtonTexture = new(ModContent.Request<Texture2D>("Redemption/Textures/Abilities/Spiritwalker", AssetRequestMode.ImmediateLoad));
        private readonly Asset<Texture2D> Button_MouseOverTexture = ModContent.Request<Texture2D>("Redemption/Textures/Abilities/Spiritwalker_Hover", AssetRequestMode.ImmediateLoad);

        public UIImage Icon;
        public UIHoverTextImageButton IconHighlight;

        public override void OnActivate()
        {
            Icon = ButtonTexture;
            Icon.Left.Set(570, 0f);
            Icon.Top.Set(105, 0f);
            Append(Icon);

            IconHighlight = new UIHoverTextImageButton(Button_MouseOverTexture, "Hold [Spirit Walker Ability Key] for 1 second to peek into the Spirit Realm");
            IconHighlight.Left.Set(-2, 0f);
            IconHighlight.Top.Set(-2, 0f);
            IconHighlight.SetVisibility(1f, 0f);
            Icon.Append(IconHighlight);

            base.OnActivate();
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            string s = "Hold [Spirit Walker Ability Key] for 1 second to peek into the Spirit Realm.";
            foreach (string key in Redemption.RedeSpiritwalkerAbility.GetAssignedKeys())
            {
                s = "Hold " + key + " for 1 second to peek into the Spirit Realm.";
            }
            IconHighlight.Text = "[c/60D0D7:Spirit Walker]\n" + s + "\nGrants unrestricted interactions with Spirits and allows you to hit Spirit Realm enemies using any weapon.\n" +
                "While in the Spirit Realm, Spirit Currents can spawn, giving you a way to transport near soulful remains to [i:Redemption/DeadRinger]Dead Ring.";
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Main.LocalPlayer.RedemptionAbility().Spiritwalker || !Main.playerInventory)
                return;
            base.Draw(spriteBatch);
        }
    }
}
