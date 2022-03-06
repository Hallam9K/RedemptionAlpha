﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Globals.NPC;
using Redemption.Globals;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.BaseExtension;
using Terraria.GameContent.UI;
using Terraria.Utilities;

namespace Redemption.NPCs.PreHM
{
    public abstract class SkeletonBase : ModNPC
    {
        public enum PersonalityState
        {
            Normal, Aggressive, Calm, Greedy, Soulful
        }

        public bool HasEyes;
        public int CoinsDropped;
        public string SoundString = "Skeleton";

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
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                    BuffID.Poisoned,
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>()
                }
            });
        }

        public bool AttackerIsUndead()
        {
            RedeNPC globalNPC = NPC.Redemption();
            if (globalNPC.attacker is NPC && (NPCLists.Undead.Contains((globalNPC.attacker as NPC).type) || NPCLists.Skeleton.Contains((globalNPC.attacker as NPC).type)))
                return true;

            return false;
        }
        public override bool PreAI()
        {
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
            if (Main.rand.NextBool(2000) && NPC.alpha <= 10 && (globalNPC.attacker == null || !globalNPC.attacker.active))
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
                    NPC.value = 4;
                    SpeedMultiplier = 1.8f;
                    break;
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
        }
    }
}
