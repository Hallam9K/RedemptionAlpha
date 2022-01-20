using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Projectiles.Hostile;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab
{
    public class BloatedScientist : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Wander,
            Alert,
            Puke
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 22;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 48;
            NPC.friendly = false;
            NPC.damage = 85;
            NPC.defense = 34;
            NPC.lifeMax = 1800;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            NPC.value = 0f;
            NPC.knockBackResist = 0.01f;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BloatedScientistBanner>();
        }
        private Vector2 moveTo;
        private int runCooldown;
        public override void AI()
        {
            if (LabArea.Active)
                NPC.DiscourageDespawn(60);

            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            NPC.TargetClosest();
            if (AIState is not ActionState.Puke)
                NPC.LookByVelocity();

            if (Main.rand.NextBool(1000))
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, Main.rand.NextBool() ? 1 : 3);

            switch (AIState)
            {
                case ActionState.Begin:
                    TimerRand = Main.rand.Next(80, 280);
                    AIState = ActionState.Idle;
                    break;

                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(15);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    SightCheck();
                    break;

                case ActionState.Wander:
                    SightCheck();

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
                    }

                    bool jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 0.7f, 6, 6, NPC.Center.Y > player.Center.Y);
                    break;

                case ActionState.Alert:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 380)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 600, false, true, blind: true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && Main.rand.NextBool(100) && NPC.DistanceSQ(globalNPC.attacker.Center) < 180 * 180)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.Y = 0;
                        NPC.velocity.X = 2 * NPC.spriteDirection;
                        AIState = ActionState.Puke;
                    }
                    angle = 0;
                    NPC.DamageHostileAttackers(0, 5);

                    jumpDownPlatforms = false;
                    NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                    if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                    else { NPC.noTileCollide = false; }
                    RedeHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.15f, 1.4f * (NPC.GetGlobalNPC<BuffNPC>().rallied ? 1.2f : 1), 6, 6, NPC.Center.Y > globalNPC.attacker.Center.Y);

                    break;
                case ActionState.Puke:
                    if (globalNPC.attacker == null || !globalNPC.attacker.active || NPC.PlayerDead() || NPC.DistanceSQ(globalNPC.attacker.Center) > 1400 * 1400 || runCooldown > 380)
                    {
                        angle = 0;
                        runCooldown = 0;
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;
                    break;
            }
        }
        float angle;
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Puke)
            {
                NPC.rotation = 0;
                if (NPC.frame.Y < 10 * frameHeight)
                    NPC.frame.Y = 10 * frameHeight;

                if (NPC.frameCounter++ >= 7)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y == 16 * frameHeight && !Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/VomitAttack").WithPitchVariance(0.1f), NPC.position);

                    if (NPC.frame.Y > 21 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                        AIState = ActionState.Alert;
                    }
                }
                if (NPC.frame.Y >= 16 * frameHeight && NPC.frame.Y <= 19 * frameHeight && NPC.frameCounter % 3 == 0)
                {
                    NPC.Shoot(NPC.Center + RedeHelper.PolarVector(10, -MathHelper.PiOver2 + (angle * NPC.spriteDirection)), ModContent.ProjectileType<OozeBall_Proj>(), NPC.damage, RedeHelper.PolarVector(11, -MathHelper.PiOver2 + (angle * NPC.spriteDirection)), false, SoundID.Item1.WithVolume(0), "", NPC.whoAmI);
                    angle += 0.12f;
                }
                return;
            }
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                {
                    if (NPC.frameCounter++ >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 4 * frameHeight)
                        NPC.frame.Y = 4 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 9 * frameHeight)
                            NPC.frame.Y = 4 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 4 * frameHeight;
            }
        }

        public int GetNearestNPC(bool grr = false)
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan)
                    continue;

                if (grr)
                {
                    if (target.lifeMax <= 5 || target.boss)
                        continue;
                }
                else
                {
                    if (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
                        continue;
                }

                if (nearestNPCDist != -1 && !(target.Distance(NPC.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(NPC.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.GetGlobalNPC<RedeNPC>();
            int gotNPC = GetNearestNPC(true);
            if (NPC.Sight(player, 300, false, true, blind: true))
            {
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 2);
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(15);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
            if (Main.rand.NextBool(600) && gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 300, false, true, blind: true))
            {
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(15);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
            gotNPC = GetNearestNPC();
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 300, false, true, blind: true))
            {
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 3);
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(15);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        public override bool? CanHitNPC(NPC target) => AIState == ActionState.Alert ? null : false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Alert;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<XenomiteShard>(), 4, 8, 16));
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(800, 3000));
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 30; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.2f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BlisterScientistGore" + (i + 1)).Type, 1);
                Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SludgeScientistGore1").Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                SoundEngine.PlaySound(SoundID.Zombie, NPC.position, 2);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "An unfortunate scientist, mutilated and disfigured by the Xenomite infection. This strain is mildly aggressive, and has bloated their gastric track to monstrous sizes... God, that must feel vile...")
            });
        }
    }
}