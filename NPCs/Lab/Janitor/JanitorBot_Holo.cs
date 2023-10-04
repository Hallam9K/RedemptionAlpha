using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.WorldGeneration;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBot_Holo : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 40;
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
            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);
            switch (NPC.ai[0])
            {
                case 0:
                    Vector2 JanitorPos = new((RedeGen.LabVector.X + 173) * 16, (RedeGen.LabVector.Y + 19) * 16 + 6);
                    NPC.position = JanitorPos;
                    NPC.alpha -= 10;
                    if (NPC.alpha <= 0)
                    {
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)JanitorPos.X, (int)JanitorPos.Y, ModContent.NPCType<JanitorBot>());
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    NPC.alpha += 20;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = 0;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(new Color(255, 255, 255, 0)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
}