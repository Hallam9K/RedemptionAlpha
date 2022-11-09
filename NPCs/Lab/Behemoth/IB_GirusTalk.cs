using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Redemption.UI;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Behemoth
{
    public class IB_GirusTalk : ModProjectile
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
            Projectile.position = Main.player[Projectile.owner].position;
            Projectile.timeLeft = 10;
            if (Projectile.ai[0]++ >= 540)
                RedeSystem.Silence = true;

            if (!Main.dedServ)
            {
                if (Projectile.ai[0] == 540)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("...", 100, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 640)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Hello, I don't believe we've met.", 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 900)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You don't know anything about me, but I know a lot about you.", 340, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

                if (Projectile.ai[0] == 1240)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Let's keep it that way for now.", 260, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);

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
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Here, I've sent a drone to offer you a gift, please accept it.", 300, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
                }
                if (Projectile.ai[0] == 1800)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Till we meet again.", 200, 1, 0.6f, "???:", 1, RedeColor.GirusTier, null, null, null, 0, sound: true);
            }
            if (Projectile.ai[0] >= 2000)
                Projectile.Kill();

            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextColor = RedeColor.GirusTier;
        }
    }
}