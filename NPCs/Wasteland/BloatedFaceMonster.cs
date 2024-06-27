using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity.Intruder;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.HM.Ranged;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class BloatedFaceMonster : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Roar
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
            Main.npcFrameCount[NPC.type] = 14;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.FaceMonster;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 3 };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCPoison[Type] = true;
            ElementID.NPCBlood[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 66;
            NPC.friendly = false;
            NPC.damage = 70;
            NPC.defense = 20;
            NPC.lifeMax = 350;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.aiStyle = -1;
            NPC.value = 500f;
            NPC.knockBackResist = 0.2f;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandCrimsonBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BloatedFaceMonsterBanner>();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            moveTo = reader.ReadVector2();
        }
        private Vector2 moveTo;
        private int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            TimerRand = Main.rand.Next(80, 120);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            NPC.LookByVelocity();

            if (Main.rand.NextBool(1000) && !Main.dedServ)
                SoundEngine.PlaySound(SoundID.Zombie8, NPC.position);

            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    SightCheck();
                    break;

                case ActionState.Wander:
                    SightCheck();

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 120);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1.3f, 16, 10, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 380))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 800, true, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (Main.rand.NextBool(100) && NPC.Sight(globalNPC.attacker, 300, false, true))
                    {
                        AITimer = 0;
                        AIState = ActionState.Roar;
                        NPC.netUpdate = true;
                        return;
                    }

                    NPC.DamageHostileAttackers(0, 6);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.3f, 5.5f, 20, 16, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.Roar:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    NPC.LookAtEntity(player);
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= .9f;

                    if (AITimer++ == 25)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie8 with { Pitch = -.6f }, NPC.position);
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Roar1 with { Pitch = .4f }, NPC.position);
                    }

                    if (AITimer == 30)
                    {
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 10;
                    }
                    if (AITimer > 25 && AITimer % 10 == 0)
                        NPC.Shoot(new Vector2(NPC.Center.X + (-2 * NPC.spriteDirection), NPC.Center.Y + 10), ModContent.ProjectileType<BloatedFaceMonster_Screech>(), 0, new Vector2(6 * NPC.spriteDirection, 0));

                    if (AITimer >= 80)
                        AIState = ActionState.Alert;
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 6 * frameHeight;
                else
                {
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 13 * frameHeight)
                            NPC.frame.Y = frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 0;
            }
        }

        public int GetNearestNPC()
        {
            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type]))
                    continue;

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
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC();
            if (NPC.Sight(player, 800, true, true))
            {
                SoundEngine.PlaySound(SoundID.Zombie8 with { Pitch = -.4f }, NPC.position);
                globalNPC.attacker = player;
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 800, true, true))
            {
                SoundEngine.PlaySound(SoundID.Zombie8 with { Pitch = -.4f }, NPC.position);
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 2) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override bool CanHitNPC(NPC target) => AIState == ActionState.Alert;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Alert;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.BeatAnyMechBoss(), ModContent.ItemType<Xenomite>(), 4, 3, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 4, 3, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GasMask>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DoubleRifle>(), 100));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.FaceMonster, false);
            foreach (var dropRule in dropRules)
            {
                npcLoot.Add(dropRule);
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(300, 1200));
        }
        public override void HitEffect(NPC.HitInfo hit)
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
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BloatedGhoulGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                SoundEngine.PlaySound(SoundID.Zombie8 with { Pitch = -.4f }, NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.BloatedFaceMonster"))
            });
        }
    }
    public class BloatedFaceMonster_Screech : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 50;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.alpha += 4;
            Projectile.scale += .04f;
            Projectile.width = 14;
            Projectile.height = 50;
            if (Projectile.alpha >= 255)
                Projectile.Kill();

            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                    continue;
                if (Projectile.Hitbox.Intersects(npc.Hitbox))
                    npc.AddBuff(BuffID.Confused, 200);
            }
            for (int n = 0; n < Main.maxPlayers; n++)
            {
                Player player = Main.player[n];
                if (!player.active || player.dead)
                    continue;
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    player.AddBuff(BuffID.Confused, 200);
                    player.AddBuff(BuffID.Obstructed, 30);
                }
            }
        }
    }
}