using Redemption.BaseExtension;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals.Players
{
    public class DialoguePlayer : ModPlayer
    {
        public enum TalkType : byte
        {
            // There'll be a lot more here in 0.8.........
            AdamShop,

            Count
        }

        public bool SetTalkState(TalkType talkID, bool reset = false)
        {
            talkComplete[(int)talkID] = !reset;
            return !reset;
        }
        public static bool SetTalkStateLocal(TalkType talkID, bool reset = false)
        {
            Main.LocalPlayer.RedemptionDialogue().talkComplete[(int)talkID] = !reset;
            return !reset;
        }
        public bool GetTalkState(TalkType talkID) => talkComplete[(int)talkID];
        public static bool GetTalkStateLocal(TalkType talkID) => Main.LocalPlayer.RedemptionDialogue().talkComplete[(int)talkID];

        public bool[] talkComplete = new bool[(int)TalkType.Count];

        public override void Initialize()
        {
            for (int i = 0; i < talkComplete.Length; i++)
                talkComplete[i] = false;
        }

        public override void SaveData(TagCompound tag)
        {
            var talkS = new List<string>();
            for (int k = 0; k < talkComplete.Length; k++)
            {
                if (talkComplete[k])
                    talkS.Add("talk" + k);
            }

            tag["talkS"] = talkS;
        }

        public override void LoadData(TagCompound tag)
        {
            var talkS = tag.GetList<string>("talkS");
            for (int k = 0; k < talkComplete.Length; k++)
                talkComplete[k] = talkS.Contains("talk" + k);
        }
    }
}