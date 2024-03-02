using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Friendly.TownNPCs;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals.NPC
{
    public class ExclaimMarkNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Terraria.NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<Daerel>() || entity.type == ModContent.NPCType<Zephos>() || entity.type == ModContent.NPCType<Fallen>();
        }
        public bool[] exclaimationMark = new bool[6];
        public override void LoadData(Terraria.NPC npc, TagCompound tag)
        {
            for (int k = 0; k < exclaimationMark.Length; k++)
                exclaimationMark[k] = tag.GetBool("exMark" + k);
        }
        public override void SaveData(Terraria.NPC npc, TagCompound tag)
        {
            for (int k = 0; k < exclaimationMark.Length; k++)
                tag["exMark" + k] = exclaimationMark[k];
        }
        private int questFrame;
        private int questCounter;
        public override void FindFrame(Terraria.NPC npc, int frameHeight)
        {
            if (++questCounter >= 4)
            {
                questCounter = 0;
                if (questFrame++ > 1)
                    questFrame = 0;
            }
        }
        public override void PostDraw(Terraria.NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool mark = false;
            for (int k = 0; k < exclaimationMark.Length; k++)
            {
                if (exclaimationMark[k])
                {
                    mark = true;
                    break;
                }
            }
            if (mark)
            {
                Texture2D questMark = ModContent.Request<Texture2D>("Redemption/Textures/QuestMark").Value;
                int Height = questMark.Height / 3;
                int y = Height * questFrame;
                Rectangle rect = new(0, y, questMark.Width, Height);
                Vector2 origin = new(questMark.Width / 2f, Height / 2f);
                spriteBatch.Draw(questMark, npc.Top - screenPos - new Vector2(0, 20), new Rectangle?(rect), npc.GetAlpha(Color.LightGoldenrodYellow), 0, origin, 1, 0, 0);
            }
        }
    }
}