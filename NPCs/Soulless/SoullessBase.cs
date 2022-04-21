using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Globals.NPC;
using Redemption.Globals;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Soulless
{
    public abstract class SoullessBase : ModNPC
    {
        public enum MaskState
        {
            Normal, Angry, Happy
        }

        public bool HasEyes;
        public int HeadY;
        public int HeadX;
        public int HeadOffsetX;
        public int HeadOffsetY;

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public MaskState MaskType
        {
            get => (MaskState)NPC.ai[3];
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
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<LaceratedDebuff>(),
                    ModContent.BuffType<BlackenedHeartDebuff>()
                }
            });
        }

        public bool AttackerIsSoulless()
        {
            RedeNPC globalNPC = NPC.Redemption();
            if (globalNPC.attacker is NPC && NPCLists.Soulless.Contains((globalNPC.attacker as NPC).type))
                return true;

            return false;
        }
        public virtual int SetHeadOffsetY(ref int frameHeight)
        {
            return 0;
        }
        public virtual int SetHeadOffsetX(ref int frameHeight)
        {
            return 0;
        }
        public virtual void SetStats()
        {
            switch (MaskType)
            {
                case MaskState.Happy:
                    NPC.lifeMax = (int)(NPC.lifeMax * 0.9f);
                    NPC.life = (int)(NPC.life * 0.9f);
                    NPC.damage = (int)(NPC.damage * 0.8f);
                    SpeedMultiplier = 1.3f;
                    break;
                case MaskState.Angry:
                    NPC.lifeMax = (int)(NPC.lifeMax * 1.05f);
                    NPC.life = (int)(NPC.life * 1.05f);
                    NPC.damage = (int)(NPC.damage * 1.05f);
                    NPC.value = (int)(NPC.value * 1.25f);
                    VisionIncrease = 100;
                    SpeedMultiplier = 1.1f;
                    break;
            }
            if (HasEyes)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 1.1f);
                NPC.life = (int)(NPC.life * 1.1f);
                NPC.defense = (int)(NPC.defense * 1.05f);
                NPC.damage = (int)(NPC.damage * 1.05f);
                NPC.value = (int)(NPC.value * 1.1f);
                VisionRange = 600 + VisionIncrease;
            }
            else
                VisionRange = 200 + VisionIncrease;
        }
    }
}
