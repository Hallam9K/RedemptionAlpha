using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Redemption.BaseExtension;
using Terraria.Audio;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.NPCs.Bosses.KSIII;
using System.IO;
using Terraria.ModLoader.Utilities;
using Redemption.Biomes;
using Redemption.Base;
using Redemption.NPCs.Friendly;
using SubworldLibrary;

namespace Redemption.NPCs.HM
{
    public class SlayerSpawner : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 22;
            NPC.lifeMax = 10000;
            NPC.aiStyle = -1;
            NPC.friendly = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.chaseable = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Pos);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Pos = reader.ReadVector2();
        }
        private Vector2 Pos;
        public override bool PreAI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (!player.active || player.dead)
                NPC.active = false;

            if (RedeWorld.slayerRep > 0 && NPC.downedMoonlord && !RedeWorld.slayerMessageGiven && !RedeBossDowned.downedOmega3 && !RedeBossDowned.downedNebuleus)
            {
                int floor = BaseWorldGen.GetFirstTileFloor((int)player.position.X / 16, (int)player.position.Y / 16);
                NPC.position = new Vector2(player.position.X, floor * 16);
                if (NPC.ai[0] == 1)
                {
                    if (NPC.ai[1]++ >= 120)
                    {
                        SpawnAndroid(ref Pos, 11);
                        NPC.active = false;
                    }
                    return true;
                }
                else if (NPC.ai[0] == 2)
                {
                    if (NPC.ai[1]++ >= 120)
                    {
                        SpawnAndroid(ref Pos, 12);
                        NPC.active = false;
                    }
                    return true;
                }
                else if (NPC.ai[0] == 3)
                {
                    player.Redemption().slayerStarRating = 1;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        RedeWorld.slayerMessageGiven = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }

                    for (int i = 0; i < 6; i++)
                        SpawnSpacePaladin(ref Pos);
                    NPC.active = false;
                    return true;
                }
                bool enemyNear = false;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                        continue;

                    if (player.DistanceSQ(npc.Center) > 800 * 800)
                        continue;

                    enemyNear = true;
                }
                if (NPC.ai[1]++ >= 600 && player.velocity.Y == 0 && player.velocity.Length() < 4 && !RedeHelper.BossActive() && !enemyNear)
                {
                    SpawnAndroid(ref Pos, 10);
                    NPC.active = false;
                }
                return true;
            }
            switch (player.Redemption().slayerStarRating)
            {
                case 1:
                    if (NPC.ai[1]++ == 300)
                    {
                        for (int i = 0; i < 3; i++)
                            SpawnAndroid(ref Pos);
                        NPC.active = false;
                    }
                    break;
                case 2:
                    if (NPC.ai[1]++ == 300)
                    {
                        for (int i = 0; i < Main.rand.Next(2, 4); i++)
                            SpawnAndroid(ref Pos);
                        SpawnPrototypeSilver(ref Pos);
                        NPC.active = false;
                    }
                    break;
                case 3:
                    if (NPC.ai[1]++ == 300)
                    {
                        for (int i = 0; i < 2; i++)
                            SpawnAndroid(ref Pos);
                        for (int i = 0; i < 2; i++)
                            SpawnPrototypeSilver(ref Pos);
                        NPC.active = false;
                    }
                    break;
                case 4:
                    if (NPC.ai[1]++ == 300)
                    {
                        SpawnSpacePaladin(ref Pos);
                        NPC.active = false;
                    }
                    break;
                default:
                    if (NPC.ai[1]++ == 300)
                    {
                        if (RedeBossDowned.slayerDeath < 2 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.slayerDeath = 2;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }
                        player.Redemption().slayerStarRating = 6;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<KS3_Start>(), 5);
                        NPC.active = false;
                    }
                    break;
            }
            return true;
        }
        private void SpawnAndroid(ref Vector2 pos, int ID = 0)
        {
            pos = NPCHelper.FindGround(NPC, 18);
            pos *= 16;
            NPC.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.1f }, pos);
            DustHelper.DrawDustImage(pos - new Vector2(0, 22), DustID.Frost, 0.1f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
            for (int k = 0; k < 20; k++)
            {
                int dust = Dust.NewDust(new Vector2(pos.X, pos.Y - 800), 2, 2 + 750, DustID.Frost);
                Main.dust[dust].noGravity = true;
            }
            for (int j = 0; j < 15; j++)
            {
                ParticleManager.NewParticle(pos - new Vector2(0, 22), RedeHelper.Spread(4), new LightningParticle(), Color.White, 3);
                int dust = Dust.NewDust(pos - new Vector2(0, 22), 2, 2, DustID.Frost, Scale: 3f);
                Main.dust[dust].velocity *= 6f;
                Main.dust[dust].noGravity = true;
            }
            int npc = ModContent.NPCType<Android>();
            RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X, (int)pos.Y, npc, 0, 0, 0, ID);
        }
        private void SpawnPrototypeSilver(ref Vector2 pos)
        {
            pos = NPCHelper.FindGround(NPC, 18);
            pos *= 16;
            NPC.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.1f }, pos);
            DustHelper.DrawDustImage(pos - new Vector2(0, 30), DustID.Frost, 0.12f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
            for (int k = 0; k < 20; k++)
            {
                int dust = Dust.NewDust(new Vector2(pos.X, pos.Y - 800), 2, 2 + 750, DustID.Frost);
                Main.dust[dust].noGravity = true;
            }
            for (int j = 0; j < 15; j++)
            {
                ParticleManager.NewParticle(pos - new Vector2(0, 64), RedeHelper.Spread(4), new LightningParticle(), Color.White, 3);
                int dust = Dust.NewDust(pos - new Vector2(0, 64), 2, 2, DustID.Frost, Scale: 3f);
                Main.dust[dust].velocity *= 6f;
                Main.dust[dust].noGravity = true;
            }
            RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X, (int)pos.Y, ModContent.NPCType<PrototypeSilver>());
        }
        private void SpawnSpacePaladin(ref Vector2 pos)
        {
            pos = NPCHelper.FindGround(NPC, 18);
            pos *= 16;
            NPC.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.1f }, pos);
            DustHelper.DrawDustImage(pos - new Vector2(0, 64), DustID.Frost, 0.2f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
            for (int k = 0; k < 30; k++)
            {
                int dust = Dust.NewDust(new Vector2(pos.X, pos.Y - 800), 2, 2 + 750, DustID.Frost);
                Main.dust[dust].noGravity = true;
            }
            for (int j = 0; j < 25; j++)
            {
                ParticleManager.NewParticle(pos - new Vector2(0, 64), RedeHelper.Spread(4), new LightningParticle(), Color.White, 3);
                int dust = Dust.NewDust(pos - new Vector2(0, 64), 2, 2, DustID.Frost, Scale: 3f);
                Main.dust[dust].velocity *= 6f;
                Main.dust[dust].noGravity = true;
            }
            RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X, (int)pos.Y, ModContent.NPCType<SpacePaladin>());
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (RedeWorld.slayerRep < 1 || RedeWorld.slayerRep >= 4 || !NPC.downedMoonlord || RedeWorld.slayerMessageGiven || RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus)
                return 0;
            if (spawnInfo.Player.InModBiome<SlayerShipBiome>() || SubworldSystem.Current != null)
                return 0;

            float baseChance = SpawnCondition.OverworldDay.Chance;
            float m = NPC.AnyNPCs(ModContent.NPCType<GiftDrone3>()) || NPC.AnyNPCs(ModContent.NPCType<Android>()) || NPC.AnyNPCs(Type) ? 0 : 100;

            return baseChance * m;
        }
    }
}