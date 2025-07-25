using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.NPCs.Friendly;
using Redemption.Projectiles.Hostile;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class MoonflareSkeleton : SkeletonBase
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Cast
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public bool HasCrown;
        public bool HasHood;

        public override void SetSafeStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 19;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new();
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
            ElementID.NPCFire[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 48;
            NPC.damage = 22;
            NPC.friendly = false;
            NPC.defense = 9;
            NPC.lifeMax = 72;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 250;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<MoonflareSkeletonBanner>();
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Nature] *= .8f;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Fire] *= .8f;
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
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/MoonflareSkeletonGore").Type, 1);

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/MoonflareSkeletonGore2").Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/MoonflareSkeletonGore3").Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, Color.SandyBrown);

            if (AIState is ActionState.Idle or ActionState.Wander)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.SkeletonNotice, NPC.position);
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(HasCrown);
            writer.Write(HasHood);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            HasCrown = reader.ReadBoolean();
            HasHood = reader.ReadBoolean();
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
                SoundEngine.PlaySound(AmbientSound, NPC.position);

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
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }
                    if (globalNPC.attacker is Player && (NPC.PlayerDead() || (globalNPC.attacker as Player).RedemptionPlayerBuff().skeletonFriendly))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }

                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, interactDoorStyle: HasEyes ? 2 : 0);

                    if (!NPC.Sight(globalNPC.attacker, 600, HasEyes, HasEyes, false))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.Sight(globalNPC.attacker, 100, false, true))
                    {
                        SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, NPC.position);
                        AITimer = 0;
                        AIState = ActionState.Cast;
                        NPC.netUpdate = true;
                        break;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 2f * SpeedMultiplier * (NPC.RedemptionNPCBuff().rallied ? 1.2f : 1), 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);

                    break;

                case ActionState.Cast:
                    if (NPC.ThreatenedCheck(ref runCooldown, 180, 2))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    if (globalNPC.attacker is Player && (NPC.PlayerDead() || (globalNPC.attacker as Player).RedemptionPlayerBuff().skeletonFriendly))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                        break;
                    }

                    int chargeReduce = HasEyes ? 20 : 0;
                    if (HasCrown)
                        chargeReduce -= 10;

                    if (AITimer < 60 - chargeReduce)
                        NPC.LookAtEntity(globalNPC.attacker);
                    NPC.velocity *= 0;

                    for (int i = 0; i < 2; i++)
                    {
                        int dustIndex = Dust.NewDust(NPC.BottomLeft - new Vector2(0, 1), NPC.width, 1, ModContent.DustType<MoonflareDust>(), 0f, 0f, 100, default, .8f);
                        Main.dust[dustIndex].velocity.Y -= 4f;
                        Main.dust[dustIndex].velocity.X *= 0f;
                        Main.dust[dustIndex].noGravity = true;
                    }

                    AITimer++;

                    if (AITimer == 60 - chargeReduce)
                    {
                        Vector2 shootPos = new(NPC.Center.X + (17 * NPC.spriteDirection), NPC.Center.Y - 20);
                        DustHelper.DrawCircle(shootPos, DustID.AmberBolt, 10, dustDensity: 2f, dustSize: 2, nogravity: true);
                        DustHelper.DrawCircle(shootPos, DustID.AmberBolt, 12f, dustDensity: 2f, dustSize: 1.6f, nogravity: true);
                        DustHelper.DrawCircle(shootPos, DustID.AmberBolt, 14f, dustDensity: 2f, dustSize: 1.2f, nogravity: true);
                        DustHelper.DrawCircle(shootPos, DustID.AmberBolt, 16f, dustDensity: 2f, dustSize: 1f, nogravity: true);
                        NPC.Shoot(shootPos, ModContent.ProjectileType<MoonflareForce_Proj>(), NPC.damage, Vector2.Zero, SoundID.DD2_EtherianPortalDryadTouch, knockback: 9, ai2: NPC.whoAmI);
                    }
                    if (AITimer >= 120 - chargeReduce)
                    {
                        AITimer = 0;
                        AIState = ActionState.Alert;
                    }
                    break;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Cast)
            {
                if (NPC.frame.Y < 13 * frameHeight)
                    NPC.frame.Y = 13 * frameHeight;

                NPC.rotation = 0;
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    int chargeReduce = HasEyes ? 20 : 0;
                    if (HasCrown)
                        chargeReduce -= 10;

                    if (AITimer < 55 - chargeReduce)
                    {
                        if (NPC.frame.Y > 14 * frameHeight)
                            NPC.frame.Y = 13 * frameHeight;
                    }
                    else
                    {
                        if (NPC.frame.Y > 18 * frameHeight)
                            NPC.frame.Y = 17 * frameHeight;
                    }
                }
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
                            NPC.frame.Y = 0;
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
        public int GetNearestNPC(int[] WhitelistNPC = default, bool friendly = false)
        {
            WhitelistNPC ??= new int[] { NPCID.Guide };

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
                    if (!WhitelistNPC.Contains(target.type) && (target.lifeMax <= 5 || (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])))
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
            int gotNPC = GetNearestNPC(!HasEyes ? (Personality == PersonalityState.Aggressive ? NPCLists.HasLostSoul.ToArray() :
                new int[] { ModContent.NPCType<LostSoulNPC>() }) : default);
            if (Personality != PersonalityState.Calm)
            {
                if (!player.RedemptionPlayerBuff().skeletonFriendly && NPC.Sight(player, 600, HasEyes, HasEyes, false))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(NoticeSound, NPC.position);
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
                if (!HasEyes && Personality == PersonalityState.Aggressive && Main.rand.NextBool(1800))
                {
                    if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 600, false, false))
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(NoticeSound, NPC.position);
                        globalNPC.attacker = Main.npc[gotNPC];
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        AIState = ActionState.Alert;
                        NPC.netUpdate = true;
                    }
                    return;
                }
                gotNPC = GetNearestNPC(!HasEyes ? new[] { ModContent.NPCType<LostSoulNPC>() } : default);

                if (player.RedemptionPlayerBuff().skeletonFriendly)
                    gotNPC = GetNearestNPC(friendly: true);

                if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], 600, HasEyes, HasEyes, false))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(NoticeSound, NPC.position);
                    globalNPC.attacker = Main.npc[gotNPC];
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
            }
        }
        public void ChoosePersonality()
        {
            if (Main.rand.NextBool(3))
                HasHood = true;
            if (Main.rand.NextBool(3))
                HasCrown = true;

            if (Personality == 0)
            {
                WeightedRandom<PersonalityState> choice = new(Main.rand);
                choice.Add(PersonalityState.Normal, 10);
                choice.Add(PersonalityState.Calm, 7);
                choice.Add(PersonalityState.Aggressive, 3);

                Personality = choice;
            }
            if (Main.rand.NextBool(3))
                HasEyes = true;
            NPC.netUpdate = true;
        }
        public override void SetStats()
        {
            int VisionIncrease = 0;
            if (HasCrown)
            {
                NPC.defense += 1;
                NPC.value = (int)(NPC.value * 1.25f);
            }
            if (HasHood)
            {
                NPC.defense += 3;
                VisionIncrease -= 60;
            }

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
                    VisionIncrease += 100;
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
        Asset<Texture2D> glow;
        Asset<Texture2D> staffGlow;
        Asset<Texture2D> hood;
        Asset<Texture2D> crown;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            glow ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            staffGlow ??= ModContent.Request<Texture2D>(Texture + "_Glow2");
            crown ??= ModContent.Request<Texture2D>(Texture + "_Crown");
            hood ??= ModContent.Request<Texture2D>(Texture + "_Hood");
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 pos = NPC.Center - new Vector2(0, 5);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasEyes && !HasHood)
                spriteBatch.Draw(glow.Value, pos - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.Draw(staffGlow.Value, pos - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (HasHood)
            {
                Rectangle rect = hood.Frame(1, 19, 0, NPC.frame.Y / 66);
                spriteBatch.Draw(hood.Value, pos - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(rect.Width / 2 + (1 * NPC.spriteDirection), NPC.frame.Height / 2 - 6), NPC.scale, effects, 0);
            }
            if (HasCrown)
            {
                Rectangle rect = crown.Frame(1, 19, 0, NPC.frame.Y / 66);
                spriteBatch.Draw(crown.Value, pos - screenPos, rect, NPC.GetAlpha(Color.White), NPC.rotation, new Vector2(rect.Width / 2 + (1 * NPC.spriteDirection), NPC.frame.Height / 2 - 8), NPC.scale, effects, 0);
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => NPC.aiStyle == 7;
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonflareFragment>(), 1, 4, 8));
            npcLoot.Add(ItemDropRule.Food(ItemID.MilkCarton, 150));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EpidotrianSkull>(), 100));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 2));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float ice = SpawnCondition.OverworldNightMonster.Chance;
            float multiplier = spawnInfo.Player.ZoneSnow && Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType is TileID.SnowBlock or TileID.IceBlock ? 0.1f : 0f;

            return ice * multiplier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.MoonflareSkeleton"))
            });
        }
    }
}