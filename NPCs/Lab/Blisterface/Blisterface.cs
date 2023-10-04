using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Usable;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Redemption.WorldGeneration;
using Redemption.Base;
using Terraria.Localization;
using Redemption.Globals.NPC;

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

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
            ElementID.NPCPoison[Type] = true;
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
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Blisterface1")),
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Blisterface2"))
            });
        }
        public override bool CheckActive()
        {
            return !Main.LocalPlayer.InModBiome<LabBiome>();
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            Point water = NPC.Center.ToTileCoordinates();
            if (Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                modifiers.FinalDamage /= 10;
        }
        public override void HitEffect(NPC.HitInfo hit)
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
            writer.Write(AITimer[0]);
            writer.Write(AITimer[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AITimer[0] = reader.ReadInt32();
            AITimer[1] = reader.ReadInt32();
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
            if (NPC.Center.X < (RedeGen.LabVector.X + 194) * 16)
                NPC.velocity.X += 1f;
            if (NPC.Center.X > (RedeGen.LabVector.X + 222) * 16)
                NPC.velocity.X -= 1f;

            if (NPC.Center.Y > (RedeGen.LabVector.Y + 196) * 16)
            {

                if (AITimer[0] > 0)
                    AITimer[0] = 0;
                NPC.velocity.Y = -3f;
            }
            switch (AITimer[0])
            {
                case 0:
                    if (NPC.DespawnHandler(1, 5))
                        return;

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
                        NPC.Shoot(new Vector2(NPC.position.X + Main.rand.Next(0, NPC.width), NPC.position.Y + Main.rand.Next(0, NPC.height)), ModContent.ProjectileType<Blisterface_Bubble>(), 80, Vector2.Zero, SoundID.Item111);
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
                            NPC.Shoot(new Vector2(NPC.Center.X + 12f * NPC.spriteDirection, NPC.Center.Y), ModContent.ProjectileType<Blisterface_Bubble>(), 80, new Vector2(Main.rand.Next(6, 13) * NPC.spriteDirection, Main.rand.Next(-2, 3)), SoundID.NPCDeath13, 0, 1);
                        }
                    }
                    if (AITimer[1] >= 68)
                    {
                        NPC.velocity.Y += 0.15f;
                    }
                    Point water = NPC.Center.ToTileCoordinates();
                    if (AITimer[1] >= 180 && Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                    {
                        AITimer[1] = 0;
                        AITimer[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            BaseAI.AIFish(NPC, ref NPC.ai, true);
            if (GlowActive)
            {
                if (GlowTimer++ > 60)
                {
                    GlowActive = false;
                    GlowTimer = 0;
                }
            }
        }
        private bool GlowActive;
        private int GlowTimer;
        public override void FindFrame(int frameHeight)
        {
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
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color colour = Color.Lerp(Color.White, Color.White, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (GlowActive)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, colour, NPC.rotation, NPC.frame.Size() / 2, 1f, effects, 0);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(800, 1600));
        }
    }
}
