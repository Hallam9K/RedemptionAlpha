using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.WorldGeneration;

namespace Redemption.NPCs.Lab.Behemoth
{
    public class IrradiatedBehemoth_Holo : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradiated Behemoth");
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 412;
            NPC.height = 56;
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
                    Vector2 BehemothPos = new(((RedeGen.LabVector.X + 214) * 16) - 4, (RedeGen.LabVector.Y + 45) * 16);
                    NPC.Center = BehemothPos;
                    NPC.alpha -= 10;
                    if (NPC.alpha <= 0)
                    {
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)BehemothPos.X, (int)BehemothPos.Y + 20, ModContent.NPCType<IrradiatedBehemoth>());
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
            Texture2D HeadAni = ModContent.Request<Texture2D>(Texture + "_Head").Value;
            Texture2D HandAni = ModContent.Request<Texture2D>(Texture + "_Hand").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(new Color(255, 255, 255, 0)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            int Height = HeadAni.Height / 2;
            int y = Height * AniFrameY;
            Rectangle rect = new(0, y, HeadAni.Width, Height);
            Vector2 origin = new(HeadAni.Width / 2f, Height / 2f);
            spriteBatch.Draw(HeadAni, NPC.Center - screenPos + new Vector2(0, 32), new Rectangle?(rect), NPC.GetAlpha(new Color(255, 255, 255, 0)), NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Draw(HandAni, NPC.Center - screenPos + new Vector2(0, 32), new Rectangle?(rect), NPC.GetAlpha(new Color(255, 255, 255, 0)), NPC.rotation, origin, NPC.scale, effects, 0);
            return false;
        }
    }
}