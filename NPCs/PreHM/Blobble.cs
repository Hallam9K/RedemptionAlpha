using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Utilities;
using Redemption.Items.Placeable.Banners;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.NPCs.PreHM
{
    public class Blobble : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Bounce
        }

        public enum HatState
        {
            None, GodsTophat, Fez, Crown, Flatcap, OldTophat, Serb
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public HatState HatType
        {
            get => (HatState)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blobble");
            Main.npcFrameCount[Type] = 3;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<LaceratedDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 18;
            NPC.damage = 2;
            NPC.defense = 0;
            NPC.lifeMax = 11;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.6f;
            NPC.value = 60;
            NPC.aiStyle = -1;
            NPC.alpha = 60;
            NPC.rarity = 1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BlobbleBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, new Color(178, 203, 177), 2);
                    Main.dust[dust].velocity *= 3f;
                    Main.dust[dust].noGravity = true;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, new Color(178, 203, 177), 2);
        }

        public int Xvel;
        public override void AI()
        {
            Player player = Main.player[NPC.GetNearestAlivePlayer()];
            NPC.TargetClosest();

            switch (AIState)
            {
                case ActionState.Begin:
                    PickHat();
                    if (HatType is HatState.Serb)
                        NPC.GivenName = "Serbble";

                    TimerRand = Main.rand.Next(30, 120);
                    AIState = ActionState.Idle;
                    break;

                case ActionState.Idle:
                    NPC.LookAtEntity(player);
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;

                    AITimer++;
                    if (AITimer >= TimerRand && (NPC.collideY || NPC.velocity.Y == 0))
                    {
                        Xvel = Main.rand.Next(3, 7);
                        NPC.velocity.X = Xvel * NPC.spriteDirection;
                        NPC.velocity.Y -= Main.rand.Next(4, 7);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(30, 120);
                        AIState = ActionState.Bounce;
                    }
                    break;

                case ActionState.Bounce:
                    NPC.velocity.X = Xvel * NPC.spriteDirection;
                    if (NPC.collideY || NPC.velocity.Y == 0)
                    {
                        AIState = ActionState.Idle;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 7;
                NPC.frame.X = NPC.frame.Width * (int)HatType;
                if (NPC.collideY || NPC.velocity.Y == 0)
                {
                    NPC.rotation = 0;
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 2 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.X * 0.03f;
                    NPC.frame.Y = 2 * frameHeight;
                }
            }
        }

        public void PickHat()
        {
            WeightedRandom<HatState> choice = new(Main.rand);
            choice.Add(HatState.None, 10);
            choice.Add(HatState.Crown, 2);
            choice.Add(HatState.Fez, 2);
            choice.Add(HatState.Flatcap, 2);
            choice.Add(HatState.GodsTophat, 1);
            choice.Add(HatState.OldTophat, 1);
            choice.Add(HatState.Serb, 0.3);

            HatType = choice;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 scale = new((NPC.velocity.X * 0.1f) + 1, (NPC.velocity.Y * 0.1f) + 1);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos - new Vector2(0, 2), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2,
                new Vector2(MathHelper.Clamp(scale.X, 1, 5), MathHelper.Clamp(scale.Y, 1, 5)), effects, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(BuffID.Slimed, 120);
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Slimed, 120);

        public override void OnKill()
        {
            if (Main.rand.NextBool(2) && !RedeWorld.blobbleSwarm && RedeWorld.blobbleSwarmCooldown <= 0)
            {
                Main.NewText("A blobble swarm approaches!", Color.PaleGreen);
                RedeWorld.blobbleSwarm = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 100));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDaySlime.Chance * 0.005f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

                new FlavorTextBestiaryInfoElement(
                    "An exceptionally rare slime native to Ithon. It may look harmless, but the acid it is composed of can dissolve iron in less than a minute.")
            });
        }
    }
}