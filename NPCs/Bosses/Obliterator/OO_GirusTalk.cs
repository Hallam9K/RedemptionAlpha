using Redemption.Globals;
using Redemption.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OO_GirusTalk : ModProjectile
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
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.1"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 840)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.2"), 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1100)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.3"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1500)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.4"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1800)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.5"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 2100)
                {
                    if (RedeBossDowned.downedSlayer)
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.v1"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                    else
                    {
                        if (RedeBossDowned.slayerDeath > 1)
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.v2"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                        else
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter5.v3"), 400, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
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
