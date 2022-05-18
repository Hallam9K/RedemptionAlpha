using Redemption.Globals;
using Redemption.UI;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OO_GirusTalk : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ominous Voice");
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
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Well done! If you're wondering about the AI, I put it elsewhere for safekeeping.", 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 840)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I'd rather not discard something I spent ages on creating.", 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1100)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("This research has proven very fruitful; I'm getting excited from thinking of how to put it all to good use!", 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1500)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Ah, right. The last test is coming up not too far from now!", 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1800)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("And I've got something amazing to show you! Ah, but It'll have to be a secret until then.", 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 2100)
                {
                    if (RedeBossDowned.downedSlayer)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Alright, I'll say this. It's a collaborative project between me and \"a mutual of ours\"", 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                    else
                    {
                        if (RedeBossDowned.slayerDeath > 1)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Alright, I'll say this. It's a collaborative project between me and one of the few to best you.", 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                        else
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Alright, I'll say this. It's a collaborative project between me and another.", 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                    }
                }
            }
            if (Projectile.ai[0] >= 2500)
                Projectile.Kill();

            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.GirusTier;
        }
    }
}