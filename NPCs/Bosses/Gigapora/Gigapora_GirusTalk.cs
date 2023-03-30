using Redemption.UI;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_GirusTalk : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ominous Voice");
        }
        public override void SetDefaults()
        {
            Projectile.width = 144;
            Projectile.height = 144;
        }
        public override void AI()
        {
            Projectile.timeLeft = 10;
            if (Projectile.ai[0]++ >= 540)
                RedeSystem.Silence = true;

            if (!Main.dedServ)
            {
                if (Projectile.ai[0] == 540)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Hah, not bad!", 100, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 640)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("A little sad how useless Teochrome's shield technology was,", 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 900)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Can't completely encompass something as the Shield projector will throw a hissy-fit if it's inside.", 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1300)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I did get some ideas on how to use this to my advantage, though...", 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1600)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Anyways! Same as last time, I'll ready another machine for a test.", 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1900)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Might take a bit longer though, as I'll try out a design of my own. Until then.", 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
            }
            if (Projectile.ai[0] >= 2300)
                Projectile.Kill();

            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.GirusTier;
        }
    }
}