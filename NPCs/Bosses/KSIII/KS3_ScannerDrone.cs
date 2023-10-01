using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_ScannerDrone : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Scanner Drone Mk.I");
            Main.npcFrameCount[NPC.type] = 8;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 14;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 260;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<KS3>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

                new FlavorTextBestiaryInfoElement("A drone used to collect data and store it within the SoS.")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        Vector2 DefaultPos;
        public int frameType;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            float soundVolume = NPC.velocity.Length() / 50;
            if (soundVolume > 2f) { soundVolume = 2f; }
            if (NPC.soundDelay == 0)
            {
                SoundEngine.PlaySound(SoundID.Item24 with { Volume = soundVolume }, NPC.position);
                NPC.soundDelay = 10;
            }
            if (NPC.ai[1]++ % 50 == 0)
                DefaultPos = new Vector2(Main.rand.Next(180, 280) * NPC.RightOfDir(player), Main.rand.Next(-60, -40));

            switch (NPC.ai[0])
            {
                case 0: // Fly Down
                    NPC.ai[2]++;
                    if (NPC.Distance(DefaultPos) < 40 || NPC.ai[2] > 120)
                    {
                        NPC.ai[0] = 1;
                        NPC.ai[2] = 0;
                    }
                    else
                        NPC.Move(DefaultPos, 11, 15, true);
                    break;
                case 1: // Stop and Scan
                    NPC.velocity *= 0.96f;
                    if (NPC.ai[2]++ == 30)
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Scan_Proj>(), 0, Vector2.Zero, CustomSounds.BallFire, NPC.whoAmI);

                    if (NPC.ai[2] > 240)
                    {
                        NPC.ai[2] = 0;
                        frameType = 1;
                        NPC.ai[0] = 2;
                        if (RedeBossDowned.slayerDeath == 0 && RedeWorld.alignment > 0)
                            CombatText.NewText(NPC.getRect(), Colors.RarityCyan, "TARGET UNIMPORTANT...", true, true);
                    }
                    break;
                case 2: // Yeet out
                    if (NPC.ai[2]++ > 10)
                    {
                        NPC.velocity.Y -= 0.3f;
                        if (NPC.DistanceSQ(player.Center) > 1500 * 1500)
                            NPC.active = false;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            switch (frameType)
            {
                case 0:
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                    break;
                case 1:
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 7 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                    break;
            }
        }

        public override bool CheckDead()
        {
            NPC npc2 = Main.npc[(int)NPC.ai[3]];
            npc2.ai[1]++;
            return true;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}