using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.WorldGeneration;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACEProject_Holo : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("MACE Project");
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 92;
            NPC.height = 164;
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
                    Vector2 MacePos = new(((RedeGen.LabVector.X + 74) * 16) - 8, (RedeGen.LabVector.Y + 169) * 16);
                    Vector2 MacePos2 = new(((RedeGen.LabVector.X + 74) * 16) - 8, (RedeGen.LabVector.Y + 164) * 16);
                    NPC.Center = MacePos2;
                    NPC.alpha -= 10;
                    if (NPC.alpha <= 0)
                    {
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)MacePos.X, (int)MacePos.Y, ModContent.NPCType<MACEProject>());
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    NPC.alpha += 15;
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
            Texture2D jawTex = ModContent.Request<Texture2D>(Texture + "_Jaw").Value;
            Vector2 drawCenter = new(NPC.Center.X, NPC.Center.Y - 18);
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int Height = jawTex.Height / 2;
            int y = Height * AniFrameY;
            Rectangle rect = new(0, y, jawTex.Width, Height);
            Vector2 drawCenterJaw = new(drawCenter.X - 1, drawCenter.Y - 1);
            Main.spriteBatch.Draw(jawTex, drawCenterJaw - screenPos, new Rectangle?(rect), NPC.GetAlpha(new Color(255, 255, 255, 0)), NPC.rotation, new Vector2(jawTex.Width / 2f, Height / 2f - 60), NPC.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, drawCenter - screenPos, NPC.frame, NPC.GetAlpha(new Color(255, 255, 255, 0)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
}