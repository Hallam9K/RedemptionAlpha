using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Redemption.UI;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.NPCs.Lab.Behemoth
{
    public class IB_GirusTalk : ModProjectile
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
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter1.1"), 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 900)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter1.2"), 340, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1240)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter1.3"), 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1500)
                {
                    Vector2 DronePos = new(((RedeGen.LabVector.X + 113) * 16) + 8, (RedeGen.LabVector.Y + 154) * 16);
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[i];
                        if (!player.active)
                            continue;

                        RedeHelper.SpawnNPC(Projectile.GetSource_FromAI(), (int)DronePos.X, (int)DronePos.Y, ModContent.NPCType<GiftDrone2>(), 0, i);
                    }
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter1.4"), 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                }
                if (Projectile.ai[0] == 1800)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.Girus.Encounter1.5"), 200, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
            }
            if (Projectile.ai[0] >= 2000)
                Projectile.Kill();

            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.GirusTier;
        }
    }
}
