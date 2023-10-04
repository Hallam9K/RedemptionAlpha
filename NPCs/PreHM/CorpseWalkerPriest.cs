using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Friendly;
using Redemption.Projectiles.Hostile;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;
using Redemption.BaseExtension;
using Redemption.Base;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Redemption.NPCs.PreHM
{
    public class CorpseWalkerPriest : SkeletonBase
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Cast
        }

        private bool Healing;
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Corpse-Walker Priest");
            Main.npcFrameCount[NPC.type] = 15;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 54;
            NPC.damage = 22;
            NPC.friendly = false;
            NPC.defense = 8;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 150;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CorpseWalkerPriestBanner>();
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Holy] *= .5f;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, Color.SandyBrown);

                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/CorpseWalkerGore").Type, 1);

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/CorpseWalkerGore2").Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, Color.SandyBrown);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/SkeletonNotice"), NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }

        private int runCooldown;
        public override void OnSpawn(IEntitySource source)
        {
            ChoosePersonality();
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            NPC.netUpdate = true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();

            if (Main.rand.NextBool(1500) && !Main.dedServ)
                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/" + SoundString + "Ambient"), NPC.position);


            switch (AIState)
            {
                case ActionState.Idle:
                    NPC.LookByVelocity();
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
                    NPC.LookByVelocity();
                    SightCheck();

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 8, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Alert:
                    NPC.LookByVelocity();
                    if (NPC.ThreatenedCheck(ref runCooldown))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        Healing = false;
                    }
                    if (globalNPC.attacker is Player && (NPC.PlayerDead() || (globalNPC.attacker as Player).RedemptionPlayerBuff().skeletonFriendly))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        Healing = false;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && (NPC.DistanceSQ(globalNPC.attacker.Center) < 100 * 100 || Main.rand.NextBool(300)))
                    {
                        if (AttackerIsUndead() && (globalNPC.attacker as NPC).life < (globalNPC.attacker as NPC).lifeMax)
                            Healing = true;

                        AITimer = 0;
                        AIState = ActionState.Cast;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, NPC.life < NPC.lifeMax / 3 && !AttackerIsUndead() ?
                        new Vector2(NPC.Center.X + (100 * NPC.RightOfDir(globalNPC.attacker)), NPC.Center.Y) : globalNPC.attacker.Center,
                        0.2f, 2.2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);

                    break;

                case ActionState.Cast:
                    if (NPC.ThreatenedCheck(ref runCooldown))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        Healing = false;
                    }
                    if (Healing && globalNPC.attacker is NPC attackerNPC && attackerNPC.life >= attackerNPC.lifeMax)
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        Healing = false;
                    }

                    NPC.LookAtEntity(globalNPC.attacker);
                    NPC.velocity *= 0;

                    for (int i = 0; i < 2; i++)
                    {
                        int dustIndex = Dust.NewDust(NPC.BottomLeft, NPC.width, 1, DustID.GoldFlame, 0f, 0f, 100, default, 1);
                        Main.dust[dustIndex].velocity.Y -= 3f;
                        Main.dust[dustIndex].velocity.X *= 0f;
                        Main.dust[dustIndex].noGravity = true;
                    }

                    AITimer++;

                    if (AITimer >= 60 && AITimer % (HasEyes ? 5 : 6) == 0)
                    {
                        Flare = true;
                        FlareTimer = 0;
                        NPC.Shoot(new Vector2(NPC.Center.X + (22 * NPC.spriteDirection), NPC.Center.Y), ModContent.ProjectileType<CorpseWalkerBolt>(), NPC.damage, RedeHelper.PolarVector(8, (globalNPC.attacker.Center - NPC.Center).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f)), SoundID.Item125, NPC.whoAmI, globalNPC.attacker is NPC ? globalNPC.attacker.whoAmI : -1);
                    }
                    if (AITimer >= 76)
                    {
                        AIState = ActionState.Alert;
                    }
                    break;
            }
            if (Flare)
            {
                FlareTimer += 2;
                if (FlareTimer > 60)
                {
                    Flare = false;
                    FlareTimer = 0;
                }
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private bool Flare;
        private float FlareTimer;
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Cast)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 10)
                    NPC.frame.Y = 13 * frameHeight;
                else if (NPC.frameCounter < 20)
                    NPC.frame.Y = 14 * frameHeight;
                else
                    NPC.frameCounter = 0;
                return;
            }
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
        public int GetNearestNPC(int[] WhitelistNPC = default, bool nearestUndead = false, bool friendly = false)
        {
            if (WhitelistNPC == null)
                WhitelistNPC = new int[] { NPCID.Guide };

            float nearestNPCDist = -1;
            int nearestNPC = -1;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.whoAmI == NPC.whoAmI || target.dontTakeDamage || target.type == NPCID.OldMan || target.type == NPCID.TargetDummy)
                    continue;

                if (friendly)
                {
                    if (target.friendly || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type] || NPCLists.SkeletonHumanoid.Contains(target.type))
                        continue;
                }
                else
                {
                    if (nearestUndead && ((!NPCLists.Undead.Contains(target.type) && !NPCLists.Skeleton.Contains(target.type)) || target.life >= target.lifeMax))
                        continue;

                    if (!nearestUndead && !WhitelistNPC.Contains(target.type) && (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])))
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
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC(nearestUndead: true);
            if (gotNPC != -1 && Main.rand.NextBool(100) && NPC.velocity.Y == 0 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                AITimer = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
            if (Personality != PersonalityState.Calm)
            {
                gotNPC = GetNearestNPC(!HasEyes ? new[] { ModContent.NPCType<LostSoulNPC>() } : default);

                if (player.RedemptionPlayerBuff().skeletonFriendly)
                    gotNPC = GetNearestNPC(friendly: true);

                if (!player.RedemptionPlayerBuff().skeletonFriendly && NPC.Sight(player, VisionRange, HasEyes, HasEyes))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/SkeletonNotice"), NPC.position);
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, HasEyes, HasEyes, false))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new("Redemption/Sounds/Custom/SkeletonNotice"), NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                }
                NPC.netUpdate = true;
            }
        }

        public void ChoosePersonality()
        {
            WeightedRandom<PersonalityState> choice = new(Main.rand);
            choice.Add(PersonalityState.Normal, 10);
            choice.Add(PersonalityState.Calm, 9);
            choice.Add(PersonalityState.Aggressive, 6);

            Personality = choice;
            if (Main.rand.NextBool(3))
                HasEyes = true;
            NPC.netUpdate = true;
        }
        public override void SetStats()
        {
            switch (Personality)
            {
                case PersonalityState.Calm:
                    NPC.lifeMax = (int)(NPC.lifeMax * 0.9f);
                    NPC.life = (int)(NPC.life * 0.9f);
                    NPC.damage = (int)(NPC.damage * 0.8f);
                    SpeedMultiplier = 0.8f;
                    break;
                case PersonalityState.Aggressive:
                    NPC.lifeMax = (int)(NPC.lifeMax * 1.05f);
                    NPC.life = (int)(NPC.life * 1.05f);
                    NPC.damage = (int)(NPC.damage * 1.05f);
                    NPC.value = (int)(NPC.value * 1.25f);
                    VisionIncrease = 100;
                    SpeedMultiplier = 1.1f;
                    break;
            }
            if (HasEyes)
            {
                NPC.lifeMax = (int)(NPC.lifeMax * 1.1f);
                NPC.life = (int)(NPC.life * 1.1f);
                NPC.defense = (int)(NPC.defense * 1.05f);
                NPC.damage = (int)(NPC.damage * 1.05f);
                NPC.value = (int)(NPC.value * 1.1f);
                VisionRange = 600 + VisionIncrease;
            }
            else
                VisionRange = 200 + VisionIncrease;
            NPC.netUpdate = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes)
                spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flare)
            {
                Vector2 position = NPC.Center - screenPos + new Vector2(22 * NPC.spriteDirection, 0);
                RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, position, Color.Yellow, NPC.rotation);
            }
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void OnKill()
        {
            if (HasEyes)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.8f));
            else if (Main.rand.NextBool(2))
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.6f));
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CorpseWalkerStaff>(), 12));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CorpseWalkerSkullVanity>(), 100));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 2));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float desert = SpawnCondition.OverworldNightMonster.Chance;
            float desertCave = SpawnCondition.DesertCave.Chance * 0.04f;
            float multiplier = spawnInfo.Player.ZoneDesert && Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType == TileID.Sand ? 0.2f : 0f;

            return Math.Max(desert * multiplier, desertCave);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundDesert,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.CorpseWalkerPriest"))
            });
        }
    }
}