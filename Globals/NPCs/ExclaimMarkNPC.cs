using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Redemption.NPCs.Friendly.TownNPCs;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals.NPCs
{
    public class ExclaimMarkNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Terraria.NPC entity, bool lateInstantiation)
        {
            return entity.type == NPCType<Daerel>() || entity.type == NPCType<Zephos>() || entity.type == NPCType<Fallen>();
        }
        public bool[] exclaimationMark = new bool[6];
        public bool adviceMarkerActive;

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
            adviceMarkerActive = false;
            bool mark = false;
            for (int k = 0; k < exclaimationMark.Length; k++)
            {
                if (exclaimationMark[k])
                {
                    if (exclaimationMark[4] && (Terraria.NPC.FindFirstNPC(NPCType<Fallen>()) == -1 || !Main.LocalPlayer.HasItemInAnyInventory(ItemType<GolemEye>())))
                        continue;

                    mark = true;
                    break;
                }
            }
            if (npc.type == NPCType<Daerel>() || npc.type == NPCType<Zephos>())
            {
                for (int k = 0; k < RedeQuest.adviceUnlocked.Length; k++)
                {
                    if (RedeQuest.adviceUnlocked[k] && !RedeQuest.adviceSeen[k])
                    {
                        if (k == (int)RedeQuest.Advice.UkkoEye && (Terraria.NPC.FindFirstNPC(NPCType<Fallen>()) == -1 || !Main.LocalPlayer.HasItemInAnyInventory(ItemType<GolemEye>())))
                            continue;
                        adviceMarkerActive = true;
                        mark = true;
                        break;
                    }
                }
            }
            if (mark)
            {
                Texture2D questMark = Request<Texture2D>("Redemption/Textures/QuestMark").Value;
                int Height = questMark.Height / 3;
                int y = Height * questFrame;
                Rectangle rect = new(0, y, questMark.Width, Height);
                Vector2 origin = new(questMark.Width / 2f, Height / 2f);
                float scaleOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi * 2f) / 6f;
                spriteBatch.Draw(questMark, npc.Top - screenPos - new Vector2(0, 20), new Rectangle?(rect), npc.GetAlpha(RedeColor.QuestMarkerColour), 0, origin, 1 + scaleOffset, 0, 0);
            }
        }
    }
}