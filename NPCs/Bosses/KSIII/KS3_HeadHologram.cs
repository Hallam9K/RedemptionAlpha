using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_HeadHologram : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hologram");
            Main.projFrames[Projectile.type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.alpha = 255;
        }

        public int faceType;
        public override void AI()
        {
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightCyan, Color.Cyan, Color.LightCyan);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2 * (faceType + 1))
                    Projectile.frame = 2 * faceType;
            }

            if (Projectile.localAI[0]++ < 30)
                Projectile.alpha -= 4;

            if (!Main.dedServ)
            {
                if (Projectile.localAI[0] == 30)
                {
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Hey, get lost.", 150, 1, 0.6f, "King Slayer III:", 0.4f, color, null, null, Projectile.Center, sound: true);
                }
                if (Projectile.localAI[0] == 170)
                {
                    faceType = 1;
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("You really aren't worth my time, ya know.", 180, 1, 0.6f, "King Slayer III:", 0.4f, color, null, null, Projectile.Center, sound: true);
                }
                if (Projectile.localAI[0] == 350)
                {
                    faceType = 3;
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("So stop bothering me and leave me to my 4D chess.", 200, 1, 0.6f, "King Slayer III:", 0.4f, color, null, null, Projectile.Center, sound: true);
                            break;
                        case 1:
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("So stop bothering me, I have a certain android I need to 'lecture'.", 200, 1, 0.6f, "King Slayer III:", 0.4f, color, null, null, Projectile.Center, sound: true);
                            break;
                        case 2:
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("So stop bothering me, I don't care about you.", 200, 1, 0.6f, "King Slayer III:", 0.4f, color, null, null, Projectile.Center, sound: true);
                            break;
                        case 3:
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue("So stop bothering me and we can all go about our day.", 200, 1, 0.6f, "King Slayer III:", 0.4f, color, null, null, Projectile.Center, sound: true);
                            break;
                    }
                }
                if (Projectile.localAI[0] == 550)
                {
                    faceType = 0;
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("I'll beat you up if you annoy me again.", 180, 1, 0.6f, "King Slayer III:", 0.4f, color, null, null, Projectile.Center, sound: true);
                }
            }
            if (Projectile.localAI[0] > 730)
            {
                if (RedeBossDowned.slayerDeath < 1)
                    RedeBossDowned.slayerDeath = 1;

                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (MoRDialogueUI.Visible)
            {
                RedeSystem.Instance.DialogueUIElement.PointPos = Projectile.Center;
                RedeSystem.Instance.DialogueUIElement.TextColor = color;
            }
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}