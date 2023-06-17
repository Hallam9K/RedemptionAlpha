using Redemption.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.NPCs.Bosses.Cleaver
{
    public class OmegaCleaver_GirusTalk : ModProjectile
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
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.timeLeft = 10;
            if (Projectile.ai[0]++ >= 540)
                RedeSystem.Silence = true;

            if (!Main.dedServ)
            {
                if (Projectile.ai[0] == 540)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...", 100, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 640)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter2.1"), 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 900)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter2.2"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1300)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter2.3"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1600)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter2.4"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1900)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter2.5"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
            }
            if (Projectile.ai[0] >= 2300)
                Projectile.Kill();

            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.GirusTier;
        }
    }
}
