using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Usable;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Redemption.WorldGeneration;
using Redemption.Base;

namespace Redemption.NPCs.Lab.Blisterface
{
    [AutoloadBossHead]
    public class Blisterface : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 64;
            NPC.friendly = false;
            NPC.damage = 100;
            NPC.defense = 10;
            NPC.lifeMax = 32520;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.knockBackResist = 0.0f;
            NPC.SpawnWithHigherTime(30);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.boss = true;
            NPC.netAlways = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("An unfortunate fish, disfigured and mutilated beyond recognition by the Xenomite infection. This strain seems to be similar to that of the Blistered Scientists..."),
                new FlavorTextBestiaryInfoElement("That's a bigass fish.")
            });
        }
        public override bool CheckActive()
        {
            return !LabArea.Active;
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            Point water = NPC.Center.ToTileCoordinates();
            if (Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                damage /= 10;
            return true;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.5f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
            }
        }
        public override void OnKill()
        {
            Player player = Main.LocalPlayer;
            if (!LabArea.labAccess[2])
                Item.NewItem(NPC.GetSource_Loot(), (int)player.position.X, (int)player.position.Y, player.width, player.height, ModContent.ItemType<ZoneAccessPanel3>());

            Item.NewItem(NPC.GetSource_Loot(), (int)player.position.X, (int)player.position.Y, player.width, player.height, ModContent.ItemType<Keycard2>());

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedBlisterface, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.Heart;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(AITimer[0]);
                writer.Write(AITimer[1]);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                AITimer[0] = reader.ReadInt32();
                AITimer[1] = reader.ReadInt32();
            }
        }

        private readonly int[] AITimer = new int[2];
        public override void PostAI()
        {
            NPC.LookByVelocity();
            if (AITimer[0] < 1)
            {
                if (NPC.Center.Y < (RedeGen.LabVector.Y + 186) * 16)
                    NPC.velocity.Y += 0.1f;
                if (NPC.Center.Y > (RedeGen.LabVector.Y + 191) * 16)
                    NPC.velocity.Y -= 0.1f;
            }
            switch (AITimer[0])
            {
                case 0:
                    DespawnHandler();

                    AITimer[1]++;
                    int jump = NPC.life > NPC.lifeMax / 2 ? 320 : 170;
                    if (AITimer[1] == jump - 40)
                        GlowActive = true;
                    if (AITimer[1] >= jump)
                    {
                        AITimer[0] = 1;
                        AITimer[1] = 0;
                        NPC.netUpdate = true;
                    }
                    NPC.noTileCollide = false;
                    if (Main.rand.NextBool(20))
                    {
                        NPC.Shoot(new Vector2(NPC.position.X + Main.rand.Next(0, NPC.width), NPC.position.Y + Main.rand.Next(0, NPC.height)), ModContent.ProjectileType<Blisterface_Bubble>(), 80, Vector2.Zero, true, SoundID.Item111);
                    }
                    if (NPC.CountNPCS(ModContent.NPCType<BlisteredFish2>()) <= 5)
                    {
                        if (Main.rand.NextBool(250))
                            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BlisteredFish2>());
                    }
                    break;
                case 1:
                    NPC.noTileCollide = true;
                    NPC.velocity.X = 0;
                    if (AITimer[1]++ == 0)
                    {
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 15 * -1;
                    }
                    if (AITimer[1] >= 50 && AITimer[1] < 70)
                    {
                        if (AITimer[1] % 2 == 0)
                        {
                            NPC.Shoot(new Vector2(NPC.Center.X + 12f * NPC.spriteDirection, NPC.Center.Y), ModContent.ProjectileType<Blisterface_Bubble>(), 80, new Vector2(Main.rand.Next(6, 13) * NPC.spriteDirection, Main.rand.Next(-2, 3)), true, SoundID.NPCDeath13, 0, 1);
                        }
                    }
                    if (AITimer[1] >= 68)
                    {
                        NPC.velocity.Y += 0.15f;
                    }
                    if (AITimer[1] >= 180 && NPC.wet)
                    {
                        AITimer[1] = 0;
                        AITimer[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            BaseAI.AIFish(NPC, ref NPC.ai, true);
        }
        private bool GlowActive;
        private int GlowTimer;
        public override void FindFrame(int frameHeight)
        {
            if (GlowActive)
            {
                if (GlowTimer++ > 60)
                {
                    GlowActive = false;
                    GlowTimer = 0;
                }
            }
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 7 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color colour = Color.Lerp(Color.White, Color.White, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (GlowActive)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, colour, NPC.rotation, NPC.frame.Size() / 2, 1f, effects, 0);
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.alpha += 5;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(800, 1600));
        }
    }
}