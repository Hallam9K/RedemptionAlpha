using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Redemption.NPCs.Minibosses.Calavia
{
    public class Calavia_NPC : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/Calavia/Calavia";
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calavia");
            Main.npcFrameCount[Type] = 20;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.friendly = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.npcSlots = 0;
            NPC.townNPC = true;
            TownNPCStayingHomeless = true;
            NPC.dontTakeDamage = true;
        }
        public override bool UsesPartyHat() => false;
        public override bool CanChat() => true;
        public override bool CheckActive() => false;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (NPC.Sight(player, 600, false, true))
                NPC.LookAtEntity(player);
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 19 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
            else
                NPC.frame.Y = 5 * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D cloak = ModContent.Request<Texture2D>(Texture + "_Cloak").Value;
            Texture2D legs = ModContent.Request<Texture2D>(Texture + "_Legs").Value;
            Texture2D arm = ModContent.Request<Texture2D>(Texture + "_Arm").Value;
            Texture2D tex = ModContent.Request<Texture2D>(Texture + "2").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(cloak, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(legs, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(arm, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}