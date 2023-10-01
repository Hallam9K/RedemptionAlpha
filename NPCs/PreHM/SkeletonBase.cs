using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals.NPC;
using Redemption.Globals;
using Redemption.BaseExtension;
using Terraria.GameContent.UI;
using Terraria.Utilities;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Redemption.NPCs.PreHM
{
    public abstract class SkeletonBase : ModNPC
    {
        public static Asset<Texture2D> head;
        public static Asset<Texture2D> SlashAni;
        public static Asset<Texture2D> SlashGlow;
        public static Asset<Texture2D> head2;
        public static Asset<Texture2D> NobleSlashAni;
        public static Asset<Texture2D> NobleSlashGlow;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            head = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/Skeleton_Heads");
            SlashAni = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/SkeletonDuelist_Slashes");
            SlashGlow = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/SkeletonDuelist_Slashes_Glow");
            head2 = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/Skeleton_Heads2");
            NobleSlashAni = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/SkeletonNoble_HalberdSlash");
            NobleSlashGlow = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/SkeletonNoble_HalberdSlash_Glow");
        }
        public override void Unload()
        {
            head = null;
            SlashAni = null;
            SlashGlow = null;
            head2 = null;
            NobleSlashAni = null;
            NobleSlashGlow = null;
        }
        public enum PersonalityState
        {
            Normal, Aggressive, Calm, Greedy, Soulful
        }

        public bool HasEyes;
        public int HeadType;
        public int HeadX;
        public int HeadOffset;
        public int CoinsDropped;
        public string SoundString = "Skeleton";
        public Vector2 moveTo;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasEyes);
            writer.Write(HeadType);
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasEyes = reader.ReadBoolean();
            HeadType = reader.ReadInt32();
            moveTo = reader.ReadVector2();
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public PersonalityState Personality
        {
            get => (PersonalityState)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        public int VisionRange;
        public int VisionIncrease;
        public float SpeedMultiplier = 1f;
        public float[] doorVars = new float[3];

        public virtual void SetSafeStaticDefaults() { }
        public override void SetStaticDefaults()
        {
            SetSafeStaticDefaults();
        }

        public bool AttackerIsUndead()
        {
            RedeNPC globalNPC = NPC.Redemption();
            if (globalNPC.attacker is NPC && (NPCLists.Undead.Contains((globalNPC.attacker as NPC).type) || NPCLists.Skeleton.Contains((globalNPC.attacker as NPC).type)))
                return true;

            return false;
        }
        public virtual int SetHeadOffset(ref int frameHeight)
        {
            return (NPC.frame.Y / frameHeight) switch
            {
                1 => -2,
                2 => -2,
                3 => -2,
                4 => 2,
                5 => -2,
                9 => -2,
                14 => -2,
                _ => 0,
            };
        }
        public override bool PreAI()
        {
            if (NPC.Redemption().spiritSummon)
                return true;

            Player player = Main.player[NPC.target];
            if (player.RedemptionPlayerBuff().skeletonFriendly)
                NPC.friendly = true;
            else
                NPC.friendly = false;

            RedeNPC globalNPC = NPC.Redemption();
            if (NPC.type == ModContent.NPCType<RaveyardSkeleton>() && Main.rand.NextBool(900))
            {
                switch (Main.rand.Next(4))
                {
                    case 0:
                        EmoteBubble.NewBubble(15, new WorldUIAnchor(NPC), 120);
                        break;
                    case 1:
                        EmoteBubble.NewBubble(136, new WorldUIAnchor(NPC), 120);
                        break;
                    case 2:
                        EmoteBubble.NewBubble(139, new WorldUIAnchor(NPC), 120);
                        break;
                    case 3:
                        EmoteBubble.NewBubble(17, new WorldUIAnchor(NPC), 120);
                        break;
                }
            }
            if (Main.rand.NextBool(2000) && NPC.alpha <= 10 && NPC.type != ModContent.NPCType<DancingSkeleton>() && (globalNPC.attacker == null || !globalNPC.attacker.active))
            {
                WeightedRandom<int> emoteID = new(Main.rand);
                switch (Personality)
                {
                    case PersonalityState.Aggressive:
                        emoteID.Add(87);
                        emoteID.Add(93);
                        emoteID.Add(138);
                        emoteID.Add(135);
                        break;
                    case PersonalityState.Calm:
                        emoteID.Add(10);
                        emoteID.Add(16);
                        break;
                    case PersonalityState.Greedy:
                        emoteID.Add(85);
                        break;
                }
                emoteID.Add(94, 0.5);
                emoteID.Add(134, 0.5);
                emoteID.Add(85, 0.3);
                emoteID.Add(90, 0.3);
                emoteID.Add(30, 0.4);
                emoteID.Add(124, 0.3);
                emoteID.Add(67, 0.4);
                emoteID.Add(69, 0.4);
                emoteID.Add(72, 0.4);
                EmoteBubble.NewBubble(emoteID, new WorldUIAnchor(NPC), 120);
            }
            return true;
        }
        public virtual void SetStats()
        {
            switch (HeadType)
            {
                case 3:
                    NPC.lifeMax = (int)(NPC.lifeMax * 0.9f);
                    NPC.life = (int)(NPC.life * 0.9f);
                    break;
                case 4:
                    NPC.lifeMax = (int)(NPC.lifeMax * 0.9f);
                    NPC.life = (int)(NPC.life * 0.9f);
                    break;
                case 5:
                    NPC.defense += 2;
                    break;
                case 6:
                    NPC.defense += 3;
                    break;
                case 7:
                    NPC.value = (int)(NPC.value * 1.25f);
                    break;
                case 12:
                    NPC.defense += 4;
                    break;
                case 13:
                    NPC.defense += 4;
                    break;
            }
            if (!NPC.Redemption().spiritSummon)
            {
                switch (Personality)
                {
                    case PersonalityState.Calm:
                        NPC.lifeMax = (int)(NPC.lifeMax * 0.9f);
                        NPC.life = (int)(NPC.life * 0.9f);
                        NPC.damage = (int)(NPC.damage * 0.8f);
                        SpeedMultiplier = 0.8f;
                        break;
                    case PersonalityState.Aggressive:
                        NPC.lifeMax = (int)(NPC.lifeMax * 1.05f);
                        NPC.life = (int)(NPC.life * 1.05f);
                        NPC.damage = (int)(NPC.damage * 1.05f);
                        NPC.value = (int)(NPC.value * 1.25f);
                        VisionIncrease = 100;
                        SpeedMultiplier = 1.1f;
                        break;
                    case PersonalityState.Soulful:
                        NPC.lifeMax = (int)(NPC.lifeMax * 1.4f);
                        NPC.life = (int)(NPC.life * 1.4f);
                        NPC.defense = (int)(NPC.defense * 1.15f);
                        NPC.damage = (int)(NPC.damage * 1.25f);
                        NPC.value *= 2;
                        VisionIncrease = 300;
                        SpeedMultiplier = 1.3f;
                        break;
                    case PersonalityState.Greedy:
                        NPC.lifeMax = (int)(NPC.lifeMax * 1.2f);
                        NPC.life = (int)(NPC.life * 1.2f);
                        NPC.defense = (int)(NPC.defense * 1.25f);
                        NPC.damage = (int)(NPC.damage * 0.6f);
                        NPC.value *= 4;
                        SpeedMultiplier = 1.8f;
                        break;
                }
            }
            if (HasEyes)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 1.1f);
                NPC.life = (int)(NPC.life * 1.1f);
                NPC.defense = (int)(NPC.defense * 1.05f);
                NPC.damage = (int)(NPC.damage * 1.05f);
                NPC.value = (int)(NPC.value * 1.1f);
                VisionRange = 600 + VisionIncrease + (NPC.type == ModContent.NPCType<SkeletonNoble>() ? 50 : 0);
            }
            else
                VisionRange = 200 + VisionIncrease + (NPC.type == ModContent.NPCType<SkeletonNoble>() ? 50 : 0);

            if (Personality is PersonalityState.Greedy)
                SoundString = "GreedySkeleton";
            else if (Personality is PersonalityState.Soulful)
                SoundString = "SoulfulSkeleton";
            NPC.netUpdate = true;
        }
    }
}
