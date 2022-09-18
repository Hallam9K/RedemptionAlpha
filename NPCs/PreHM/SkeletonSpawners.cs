﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Redemption.Tiles.Tiles;
using System.Linq;
using Terraria.DataStructures;

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
        public enum SpawnType
        {
            Noble, Warden, Flagbearer, SmallGroup, Group, LargeGroup
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> SpawnChoice = new(Main.rand);
            SpawnChoice.Add(SpawnType.Noble, 8);
            SpawnChoice.Add(SpawnType.Warden, 10);
            SpawnChoice.Add(SpawnType.Flagbearer, 10);
            SpawnChoice.Add(SpawnType.SmallGroup, 6);
            SpawnChoice.Add(SpawnType.Group, 3);
            SpawnChoice.Add(SpawnType.LargeGroup, 1);

            WeightedRandom<int> NPCType = new(Main.rand);
            NPCType.Add(ModContent.NPCType<SkeletonNoble>());
            NPCType.Add(ModContent.NPCType<SkeletonWarden>());
            NPCType.Add(ModContent.NPCType<EpidotrianSkeleton>());

            Vector2 pos = Vector2.Zero;
            switch ((SpawnType)SpawnChoice)
            {
                case SpawnType.Noble:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonNoble>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonNoble).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonNoble).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    break;
                case SpawnType.Warden:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonWarden>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWarden).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWarden).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    break;
                case SpawnType.Flagbearer:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonFlagbearer>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonFlagbearer).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonFlagbearer).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    break;
                case SpawnType.SmallGroup:
                    if (Main.rand.NextBool(2))
                    {
                        pos = RedeHelper.FindGround(NPC, 10);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, ModContent.NPCType<SkeletonFlagbearer>());
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 5);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Group:
                    if (!Main.rand.NextBool(3))
                    {
                        pos = RedeHelper.FindGround(NPC, 10);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, ModContent.NPCType<SkeletonFlagbearer>());
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 8);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.LargeGroup:
                    pos = RedeHelper.FindGround(NPC, 10);
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, ModContent.NPCType<SkeletonFlagbearer>());
                    for (int i = 0; i < 5; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 10);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    }
                    NPC.active = false;
                    break;
            }
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldNightMonster.Chance * 0.07f;

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
        public enum SpawnType
        {
            Normal, Wanderer, Assassin, Duelist, SmallGroup, Group, LargeGroup, Dance
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> choice = new(Main.rand);
            choice.Add(SpawnType.Normal, 10);
            choice.Add(SpawnType.Wanderer, 10);
            choice.Add(SpawnType.Assassin, 9);
            choice.Add(SpawnType.Duelist, 8);
            choice.Add(SpawnType.SmallGroup, 6);
            choice.Add(SpawnType.Group, 3);
            choice.Add(SpawnType.LargeGroup, 1);
            if (Main.player[RedeHelper.GetNearestAlivePlayer(NPC)].ZoneRockLayerHeight)
                choice.Add(SpawnType.Dance, 0.001);

            WeightedRandom<int> NPCType = new(Main.rand);
            NPCType.Add(ModContent.NPCType<SkeletonWanderer>());
            NPCType.Add(ModContent.NPCType<SkeletonAssassin>());
            NPCType.Add(ModContent.NPCType<SkeletonDuelist>());
            NPCType.Add(ModContent.NPCType<EpidotrianSkeleton>());

            switch ((SpawnType)choice)
            {
                case SpawnType.Normal:
                    NPC.SetDefaults(ModContent.NPCType<EpidotrianSkeleton>());
                    (Main.npc[NPC.whoAmI].ModNPC as EpidotrianSkeleton).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as EpidotrianSkeleton).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.alpha = 0;
                    break;
                case SpawnType.Wanderer:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonWanderer>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWanderer).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonWanderer).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    break;
                case SpawnType.Assassin:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonAssassin>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonAssassin).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonAssassin).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    NPC.ai[0] = Main.rand.NextBool(2) ? 0 : 2;
                    break;
                case SpawnType.Duelist:
                    NPC.SetDefaults(ModContent.NPCType<SkeletonDuelist>());
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonDuelist).ChoosePersonality();
                    (Main.npc[NPC.whoAmI].ModNPC as SkeletonDuelist).SetStats();
                    NPC.ai[2] = Main.rand.Next(80, 280);
                    break;
                case SpawnType.SmallGroup:
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        Vector2 pos = RedeHelper.FindGround(NPC, 5);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Group:
                    for (int i = 0; i < Main.rand.Next(4, 6); i++)
                    {
                        Vector2 pos = RedeHelper.FindGround(NPC, 8);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.LargeGroup:
                    for (int i = 0; i < Main.rand.Next(6, 8); i++)
                    {
                        Vector2 pos = RedeHelper.FindGround(NPC, 10);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Dance:
                    NPC.SetDefaults(ModContent.NPCType<DancingSkeleton>());
                    NPC.ai[3] = 1;
                    break;
            }
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.Cavern.Chance;
            float multiplier = TileLists.AncientTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType) ? 0.3f : 0.1f;

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
        public enum SpawnType
        {
            SmallGroup, Group, LargeGroup
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> SpawnChoice = new(Main.rand);
            SpawnChoice.Add(SpawnType.SmallGroup, 6);
            SpawnChoice.Add(SpawnType.Group, 3);
            SpawnChoice.Add(SpawnType.LargeGroup, 1);

            int NPCType = ModContent.NPCType<RaveyardSkeleton>();

            Vector2 pos = Vector2.Zero;
            switch ((SpawnType)SpawnChoice)
            {
                case SpawnType.SmallGroup:
                    pos = RedeHelper.FindGround(NPC, 15);
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    for (int i = 0; i < 2; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 15);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType, ai2: 1);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Group:
                    pos = RedeHelper.FindGround(NPC, 15);
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    for (int i = 0; i < 3; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 15);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType, ai2: 1);
                    }
                    NPC.active = false;
                    break;
                case SpawnType.LargeGroup:
                    pos = RedeHelper.FindGround(NPC, 15);
                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType);
                    for (int i = 0; i < 5; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 15);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, NPCType, ai2: 1);
                    }
                    NPC.active = false;
                    break;
            }
            return true;
        }
    }
}
