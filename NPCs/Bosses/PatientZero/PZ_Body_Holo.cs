using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.WorldGeneration;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZ_Body_Holo : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Patient Zero");
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 80;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.alpha = 255;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void AI()
        {
            switch (NPC.ai[0])
            {
                case 0:
                    Vector2 KariPos = new((RedeGen.LabVector.X + 144) * 16, ((RedeGen.LabVector.Y + 193) * 16) + 1);
                    NPC.Center = KariPos;
                    NPC.alpha -= 10;
                    if (NPC.alpha <= 0)
                    {
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)KariPos.X, (int)KariPos.Y, ModContent.NPCType<PZ>());
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    NPC.alpha += 5;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    break;
            }
        }
        private int AniFrameY;
        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = 0;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                AniFrameY++;
                if (NPC.frame.Y > 1 * frameHeight)
                {
                    AniFrameY = 0;
                    NPC.frame.Y = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D SlimeAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Slime_Holo").Value;
            Vector2 drawCenter = new(NPC.Center.X - 2, NPC.Center.Y - 26);

            int Height = SlimeAni.Height / 2;
            int y = Height * AniFrameY;
            Rectangle rect = new(0, y, SlimeAni.Width, Height);
            Vector2 drawCenterC = new(NPC.Center.X + 5, NPC.Center.Y - 32);
            spriteBatch.Draw(SlimeAni, drawCenterC - screenPos, new Rectangle?(rect), NPC.GetAlpha(new Color(255, 255, 255, 0)), 0, new Vector2(SlimeAni.Width / 2f, Height / 2f), 1, 0, 0f);

            spriteBatch.Draw(texture, drawCenter - screenPos, NPC.frame, NPC.GetAlpha(new Color(255, 255, 255, 0)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, 0, 0f);
            return false;
        }
    }
}