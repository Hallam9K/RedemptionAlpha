using System.IO;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs
{
    public abstract class ModRedeNPC : ModNPC
    {
        public Vector2 moveTo;
        public float SpeedMultiplier = 1f;

        #region Dialect Methods
        public virtual bool HasTalkButton() => false;
        public virtual bool HasReviveButton() => false;
        public virtual bool HasCruxButton(Player player) => false;
        public virtual string CruxButtonText(Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        public virtual void CruxButton(Player player) { }

        public const short EPIDOTRA = 1, KINGDOM = 2, CAVERN = 3, LIDEN = 4, OMEGA = 5, SLAYER = 6, NEBULEUS = 7, BLACK = 8, DEMON = 9;
        public struct HangingButtonParams(int count = 1, bool glow = false, float positionY = 0)
        {
            public float PositionY = positionY;
            public bool Glow = glow;
            public int Count = count;
        }

        public short DialogueBoxStyle;
        public virtual HangingButtonParams LeftHangingButton(Player player) => new(1);
        public virtual HangingButtonParams RightHangingButton(Player player) => new(1);
        public virtual bool HasLeftHangingButton(Player player) => false;
        public virtual bool HasRightHangingButton(Player player) => false;
        #endregion

        public void BasicGroundedFrames(int frameHeight, int idleFrameMax, int walkFrameMin, int walkFrameMax, int jumpFrame, int frameCounter = 10, int walkFrameCounter = 3)
        {
            if (NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    if (++NPC.frameCounter >= frameCounter)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > idleFrameMax * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
                else
                {
                    if (NPC.frame.Y < walkFrameMin * frameHeight)
                        NPC.frame.Y = walkFrameMin * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= walkFrameCounter || NPC.frameCounter <= -walkFrameCounter)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > walkFrameMax * frameHeight)
                            NPC.frame.Y = walkFrameMin * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = jumpFrame * frameHeight;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
        }
    }
}