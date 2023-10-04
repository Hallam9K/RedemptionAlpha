using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.IO;
using Terraria.Audio;
using Redemption.Dusts;

namespace Redemption.NPCs.PreHM
{
    public class SurfaceSkeletonSpawner : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 54;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
        }
        public enum SpawnType { Noble, Warden, Flagbearer, SmallGroup, Group, LargeGroup }
        public SpawnType AIState
        {
            get => (SpawnType)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float TypeNPC => ref NPC.ai[1];
        private Vector2 Pos;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Pos);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Pos = reader.ReadVector2();
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> SpawnChoice = new(Main.rand);
            SpawnChoice.Add(SpawnType.Noble, 8); // 21%
            SpawnChoice.Add(SpawnType.Warden, 10); // 26%
            SpawnChoice.Add(SpawnType.Flagbearer, 10);
            SpawnChoice.Add(SpawnType.SmallGroup, 6); // 15.7%
            SpawnChoice.Add(SpawnType.Group, 3); // 7.9%
            SpawnChoice.Add(SpawnType.LargeGroup, 1); // 2.6%

            WeightedRandom<int> NPCType = new(Main.rand);
            NPCType.Add(ModContent.NPCType<SkeletonNoble>());
            NPCType.Add(ModContent.NPCType<SkeletonWarden>());
            NPCType.Add(ModContent.NPCType<EpidotrianSkeleton>());

            AIState = SpawnChoice;
            NPC.netUpdate = true;
            switch (AIState)
            {
                case SpawnType.Noble:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonNoble>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonNoble).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonNoble).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.netUpdate = true;
                    break;
                case SpawnType.Warden:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonWarden>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWarden).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWarden).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.netUpdate = true;
                    break;
                case SpawnType.Flagbearer:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonFlagbearer>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonFlagbearer).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonFlagbearer).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.netUpdate = true;
                    break;
                case SpawnType.SmallGroup:
                    if (Main.rand.NextBool(2))
                    {
                        Pos = NPCHelper.FindGround(NPC, 10);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, ModContent.NPCType<SkeletonFlagbearer>());
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        TypeNPC = NPCType;
                        Pos = NPCHelper.FindGround(NPC, 5);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, (int)TypeNPC);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Group:
                    if (!Main.rand.NextBool(3))
                    {
                        Pos = NPCHelper.FindGround(NPC, 10);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, ModContent.NPCType<SkeletonFlagbearer>());
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        TypeNPC = NPCType;
                        Pos = NPCHelper.FindGround(NPC, 8);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, (int)TypeNPC);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.LargeGroup:
                    Pos = NPCHelper.FindGround(NPC, 10);
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, ModContent.NPCType<SkeletonFlagbearer>());
                    for (int i = 0; i < 5; i++)
                    {
                        TypeNPC = NPCType;
                        Pos = NPCHelper.FindGround(NPC, 10);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, (int)TypeNPC);
                    }
                    NPC.active = false;
                    break;
            }
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldNightMonster.Chance * (!Main.hardMode ? 0.07f : 0.05f);

            return baseChance;
        }
    }
    public class CavernSkeletonSpawner : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 54;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
        }
        public enum SpawnType { Normal, Wanderer, Assassin, Duelist, SmallGroup, Group, LargeGroup, Dance }
        public SpawnType AIState
        {
            get => (SpawnType)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float TypeNPC => ref NPC.ai[1];
        private Vector2 Pos;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Pos);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Pos = reader.ReadVector2();
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> choice = new(Main.rand);
            choice.Add(SpawnType.Normal, 10); // 21%
            choice.Add(SpawnType.Wanderer, 10);
            choice.Add(SpawnType.Assassin, 9); // 19%
            choice.Add(SpawnType.Duelist, 8); // 17%
            choice.Add(SpawnType.SmallGroup, 6); // 12.7%
            choice.Add(SpawnType.Group, 3); // 6.38%
            choice.Add(SpawnType.LargeGroup, 1); // 2%
            if (Main.player[RedeHelper.GetNearestAlivePlayer(NPC)].ZoneRockLayerHeight)
                choice.Add(SpawnType.Dance, 0.002); // 0.0043%

            WeightedRandom<int> NPCType = new(Main.rand);
            NPCType.Add(ModContent.NPCType<SkeletonWanderer>());
            NPCType.Add(ModContent.NPCType<SkeletonAssassin>());
            NPCType.Add(ModContent.NPCType<SkeletonDuelist>());
            NPCType.Add(ModContent.NPCType<EpidotrianSkeleton>());

            AIState = choice;
            NPC.netUpdate = true;
            switch (AIState)
            {
                case SpawnType.Normal:
                    NPC.SetDefaults(ModContent.NPCType<EpidotrianSkeleton>());
                    (Main.npc[NPC.whoAmI].ModNPC as EpidotrianSkeleton).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as EpidotrianSkeleton).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.alpha = 0;
                    NPC.netUpdate = true;
                    break;
                case SpawnType.Wanderer:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonWanderer>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWanderer).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWanderer).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.netUpdate = true;
                    break;
                case SpawnType.Assassin:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonAssassin>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonAssassin).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonAssassin).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.ai[0] = Main.rand.NextBool(2) ? 0 : 2;
                    NPC.netUpdate = true;
                    break;
                case SpawnType.Duelist:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonDuelist>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonDuelist).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonDuelist).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.netUpdate = true;
                    break;
                case SpawnType.SmallGroup:
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        TypeNPC = NPCType;
                        Pos = NPCHelper.FindGround(NPC, 5);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, (int)TypeNPC);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Group:
                    for (int i = 0; i < Main.rand.Next(4, 6); i++)
                    {
                        TypeNPC = NPCType;
                        Pos = NPCHelper.FindGround(NPC, 8);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, (int)TypeNPC);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.LargeGroup:
                    for (int i = 0; i < Main.rand.Next(6, 8); i++)
                    {
                        TypeNPC = NPCType;
                        Pos = NPCHelper.FindGround(NPC, 10);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, (int)TypeNPC);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Dance:
                    NPC.SetDefaults(ModContent.NPCType<DancingSkeleton>());
                    NPC.ai[3] = 1;
                    NPC.netUpdate = true;
                    break;
            }
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.Cavern.Chance;
            float multiplier = TileLists.AncientTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType) ? 0.3f : (!Main.hardMode ? 0.1f : 0.07f);

            return baseChance * multiplier;
        }
    }
    public class RaveyardSkeletonSpawner : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 54;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
        }
        public enum SpawnType { SmallGroup, Group, LargeGroup }
        public SpawnType AIState
        {
            get => (SpawnType)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float TypeNPC => ref NPC.ai[1];
        private Vector2 Pos;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Pos);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Pos = reader.ReadVector2();
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> SpawnChoice = new(Main.rand);
            SpawnChoice.Add(SpawnType.SmallGroup, 6);
            SpawnChoice.Add(SpawnType.Group, 3);
            SpawnChoice.Add(SpawnType.LargeGroup, 1);

            int NPCType = ModContent.NPCType<RaveyardSkeleton>();

            AIState = SpawnChoice;
            NPC.netUpdate = true;
            switch ((SpawnType)SpawnChoice)
            {
                case SpawnType.SmallGroup:
                    Pos = NPCHelper.FindGround(NPC, 15);
                    NPC.netUpdate = true;
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, NPCType);
                    for (int i = 0; i < 2; i++)
                    {
                        Pos = NPCHelper.FindGround(NPC, 15);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, NPCType, ai2: 1);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Group:
                    Pos = NPCHelper.FindGround(NPC, 15);
                    NPC.netUpdate = true;
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, NPCType);
                    for (int i = 0; i < 3; i++)
                    {
                        Pos = NPCHelper.FindGround(NPC, 15);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, NPCType, ai2: 1);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.LargeGroup:
                    Pos = NPCHelper.FindGround(NPC, 15);
                    NPC.netUpdate = true;
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, NPCType);
                    for (int i = 0; i < 5; i++)
                    {
                        Pos = NPCHelper.FindGround(NPC, 15);
                        NPC.netUpdate = true;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, NPCType, ai2: 1);
                    }
                    NPC.active = false;
                    break;
            }
            return true;
        }
    }
    public class GathicTomb_Spawner : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 54;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
            NPC.npcSlots = 0;
        }
        public ref float TypeNPC => ref NPC.ai[1];
        private Vector2 Pos;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Pos);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Pos = reader.ReadVector2();
        }
        private bool spawned;
        public override bool PreAI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            TypeNPC = ModContent.NPCType<SkeletonAssassin>();
            int clearNum = 3;
            switch (NPC.ai[0])
            {
                case 1:
                    TypeNPC = ModContent.NPCType<SkeletonNoble>();
                    clearNum = 2;
                    break;
                case 2:
                    TypeNPC = ModContent.NPCType<SkeletonWanderer>();
                    clearNum = 4;
                    break;
                case 3:
                    TypeNPC = ModContent.NPCType<AncientGladestoneGolem>();
                    clearNum = 3;
                    break;
                case 4:
                    TypeNPC = ModContent.NPCType<JollyMadman>();
                    clearNum = 2;
                    break;
            }
            if (player.dead)
            {
                spawned = false;
                return false;
            }
            if (spawned && !NPC.AnyNPCs((int)TypeNPC) && player.DistanceSQ(NPC.Center) < 500 * 500)
            {
                RedeWorld.spawnCleared[(int)NPC.ai[0]] = true;
                NPC.active = false;
                return false;
            }
            if (!spawned && player.DistanceSQ(NPC.Center) < 500 * 500)
            {
                for (int i = 0; i < clearNum; i++)
                {
                    Pos = NPCHelper.FindGround(NPC, 10);
                    NPC.netUpdate = true;
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)Pos.X * 16, (int)Pos.Y * 16, (int)TypeNPC, 0, 0, 0, 4);
                    for (int v = 0; v < 20; v++)
                    {
                        int dust = Dust.NewDust(Pos * 16 - new Vector2(32, 54), 32, 54, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Color dustColor = new(188, 244, 227) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath6, Pos * 16);
                }
                spawned = true;
            }
            return true;
        }
    }
}
