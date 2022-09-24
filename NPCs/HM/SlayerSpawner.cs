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
        public override bool PreAI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            Vector2 pos = Vector2.Zero;
            switch (player.Redemption().slayerStarRating)
            {
                case 1:
                    if (NPC.ai[1]++ == 300)
                    {
                        for (int i = 0; i < 3; i++)
                            SpawnAndroid(pos);
                        NPC.active = false;
                    }
                    break;
                case 2:
                    if (NPC.ai[1]++ == 300)
                    {
                        for (int i = 0; i < Main.rand.Next(2, 4); i++)
                            SpawnAndroid(pos);
                        SpawnPrototypeSilver(pos);
                        NPC.active = false;
                    }
                    break;
                case 3:
                    if (NPC.ai[1]++ == 300)
                    {
                        for (int i = 0; i < 2; i++)
                            SpawnAndroid(pos);
                        for (int i = 0; i < 2; i++)
                            SpawnPrototypeSilver(pos);
                        NPC.active = false;
                    }
                    break;
                case 4:
                    if (NPC.ai[1]++ == 300)
                    {
                        SpawnSpacePaladin(pos);
                        NPC.active = false;
                    }
                    break;
                default:
                    if (NPC.ai[1]++ == 300)
                    {
                        if (RedeBossDowned.slayerDeath < 2)
                        {
                            RedeBossDowned.slayerDeath = 2;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        player.Redemption().slayerStarRating = 0;
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<KS3_Start>(), 5);
                        NPC.active = false;
                    }
                    break;
            }
            return true;
        }
        private void SpawnAndroid(Vector2 pos)
        {
            pos = RedeHelper.FindGround(NPC, 18);
            pos *= 16;
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
            RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X, (int)pos.Y, ModContent.NPCType<Android>());
        }
        private void SpawnPrototypeSilver(Vector2 pos)
        {
            pos = RedeHelper.FindGround(NPC, 18);
            pos *= 16;
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
        private void SpawnSpacePaladin(Vector2 pos)
        {
            pos = RedeHelper.FindGround(NPC, 18);
            pos *= 16;
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
    }
}