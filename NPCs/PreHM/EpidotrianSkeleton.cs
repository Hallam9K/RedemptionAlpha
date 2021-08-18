using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Usable;
using Redemption.Projectiles.Hostile;
using Redemption.Tiles.Tiles;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class EpidotrianSkeleton : ModNPC
    {
        private enum PersonalityState
        {
            Normal, Aggressive, Calm, Greedy, Soulful
        }
        private enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Alert
        }

        private bool HasEyes;

        public ref float AIState => ref NPC.ai[0];

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public ref float Personality => ref NPC.ai[3];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.Skeletons[NPC.type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 48;
            NPC.damage = 18;
            NPC.friendly = false;
            NPC.defense = 7;
            NPC.lifeMax = 54;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 95;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                string SkeleType = Personality == (float)PersonalityState.Greedy ? "Greedy" : "Epidotrian";

                if (Personality == (float)PersonalityState.Soulful)
                {
                    for (int i = 0; i < 15; i++)
                        Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.SpectreStaff,
                            NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                }

                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, Personality == (float)PersonalityState.Greedy ? DustID.GoldCoin : DustID.Bone,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/" + SkeleType + "SkeletonGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, Personality == (float)PersonalityState.Greedy ? DustID.GoldCoin : DustID.Bone,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is (float)ActionState.Idle or (float)ActionState.Wander)
            {
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 2);
                AITimer = 0;
                AIState = (float)ActionState.Alert;
            }
        }

        private Vector2 moveTo;
        private int runCooldown;
        private int VisionRange;
        private int VisionIncrease;
        private float SpeedMultiplier = 1f;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            NPC.TargetClosest();
            NPC.LookByVelocity();

            switch (AIState)
            {
                case (float)ActionState.Begin:
                    Personality = ChoosePersonality();
                    SetStats();

                    TimerRand = Main.rand.Next(80, 280);
                    AIState = (float)ActionState.Idle;
                    break;

                case (float)ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = (float)ActionState.Wander;
                    }

                    if (Personality != (float)PersonalityState.Calm && NPC.Sight(player, VisionRange, !HasEyes, !HasEyes))
                    {
                        SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                        globalNPC.attacker = player;
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        AIState = (float)ActionState.Alert;
                    }
                    break;

                case (float)ActionState.Wander:
                    if (Personality != (float)PersonalityState.Calm && NPC.Sight(player, VisionRange, !HasEyes, !HasEyes))
                    {
                        SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                        globalNPC.attacker = player;
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        AIState = (float)ActionState.Alert;
                    }

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = (float)ActionState.Idle;
                    }

                    bool jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 8, NPC.Center.Y > player.Center.Y);
                    break;

                case (float)ActionState.Alert:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 180)
                    {
                        runCooldown = 0;
                        AIState = (float)ActionState.Wander;
                    }

                    if (!NPC.Sight(player, VisionRange, !HasEyes, !HasEyes))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.4f, 2.5f * SpeedMultiplier, 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y);

                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 3;
            NPC.frame.X = Personality switch
            {
                (float)PersonalityState.Soulful => NPC.frame.Width,
                (float)PersonalityState.Greedy => NPC.frame.Width * 2,
                _ => 0,
            };

            NPC.frame.X = 0;
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    if (++NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 12 * frameHeight)
                            NPC.frame.Y = 5 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        public float ChoosePersonality()
        {
            WeightedRandom<float> choice = new();
            choice.Add((float)PersonalityState.Normal, 5);
            choice.Add((float)PersonalityState.Calm, 4);
            choice.Add((float)PersonalityState.Aggressive, 4);
            choice.Add((float)PersonalityState.Soulful, 1);
            choice.Add((float)PersonalityState.Greedy, 1);

            if (Main.rand.NextBool(4) || choice == (float)PersonalityState.Soulful)
                HasEyes = true;
            return choice;
        }
        public void SetStats()
        {
            switch (Personality)
            {
                case (float)PersonalityState.Calm:
                    NPC.lifeMax *= (int)0.9f;
                    NPC.life *= (int)0.9f;
                    NPC.damage *= (int)0.8f;
                    SpeedMultiplier = 0.7f;
                    break;
                case (float)PersonalityState.Aggressive:
                    NPC.lifeMax *= (int)1.05f;
                    NPC.life *= (int)1.05f;
                    NPC.damage *= (int)1.05f;
                    NPC.value *= (int)1.25f;
                    VisionIncrease = 200;
                    SpeedMultiplier = 1.1f;
                    break;
                case (float)PersonalityState.Soulful:
                    NPC.lifeMax *= (int)1.25f;
                    NPC.life *= (int)1.25f;
                    NPC.defense *= (int)1.15f;
                    NPC.damage *= (int)1.25f;
                    NPC.value *= 2;
                    VisionIncrease = 400;
                    SpeedMultiplier = 1.3f;
                    break;
                case (float)PersonalityState.Greedy:
                    NPC.lifeMax *= (int)1.2f;
                    NPC.life *= (int)1.2f;
                    NPC.defense *= (int)1.25f;
                    NPC.damage *= (int)0.6f;
                    NPC.value *= 4;
                    SpeedMultiplier = 1.4f;
                    break;
            }
            if (HasEyes)
            {
                NPC.lifeMax *= (int)1.05f;
                NPC.life *= (int)1.05f;
                NPC.defense *= (int)1.05f;
                NPC.damage *= (int)1.05f;
                NPC.value *= (int)1.05f;
                VisionRange = 800 + VisionIncrease;
            }
            VisionRange = 300 + VisionIncrease;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/NPCs/PreHM/" + GetType().Name + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientGoldCoin>(), 1, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hook, 25));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ItemID.BoneSword, 204));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(
                    "These skeletons are from Epidotra's mainland, they are slightly taller and smarter than the Terrarian ones. Their strength is dependent on their soul, which is also known as Willpower.")
            });
        }
    }
}