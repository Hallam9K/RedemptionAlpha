using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Placeable.Banners;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Items.Usable.Potions;
using Terraria.Utilities;
using Terraria.UI;
using Redemption.Base;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Projectiles.Hostile;
using Terraria.ModLoader.Utilities;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameContent.UI;
using Redemption.Items.Usable;
using System.IO;
using Redemption.UI.ChatUI;
using Redemption.Textures;

namespace Redemption.NPCs.HM
{
    public class Android : ModNPC
    {
        public enum ActionState
        {
            Idle,
            Wander,
            Scan,
            Alert,
            RocketFist,
            Teleport
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public ref float Variant => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Android Mk.I");
            Main.npcFrameCount[NPC.type] = 19;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 46;
            NPC.friendly = false;
            NPC.damage = 40;
            NPC.defense = 40;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.aiStyle = -1;
            NPC.value = 500;
            NPC.knockBackResist = 0.1f;
            NPC.chaseable = false;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AndroidBanner>();
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
            PickType();
            if (Variant == 2)
            {
                NPC.defense *= 10;
                NPC.lifeMax /= 4;
                NPC.life = NPC.lifeMax;
                NPC.GivenName = "Apidroid Mk.I";
            }

            TimerRand = Main.rand.Next(80, 120);
            NPC.netUpdate = true;
        }
        private NPC closeNPC;
        private static Texture2D Bubble => CommonTextures.TextBubble_Slayer.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.8f };
        public override void AI()
        {
            CustomFrames(52);

            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();

            if (Variant >= 10)
            {
                NPC.LookAtEntity(player);
                NPC.knockBackResist = 0;
                if (AITimer++ == 10)
                {
                    DialogueChain chain = new();
                    if (Variant == 11)
                    {
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Message.Attacked1"), Color.LightBlue, Color.DarkCyan, voice, .03f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Message.Attacked2"), Color.LightBlue, Color.DarkCyan, voice, .03f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Message.Message"), Color.LightBlue, Color.DarkCyan, CustomSounds.Voice6 with { Pitch = 0.2f }, .03f, 3f, .5f, true, bubble: Bubble, endID: 1));
                    }
                    else if (Variant == 12)
                    {
                        NPC.dontTakeDamage = true;
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Message.Variant"), Color.LightBlue, Color.DarkCyan, voice, .03f, 2f, 0, false, bubble: Bubble));
                    }
                    else
                    {
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Message.1"), Color.LightBlue, Color.DarkCyan, voice, .03f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Message.2"), Color.LightBlue, Color.DarkCyan, voice, .03f, 2f, 0, false, bubble: Bubble))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.KS3Message.Message"), Color.LightBlue, Color.DarkCyan, CustomSounds.Voice6 with { Pitch = 0.2f }, .03f, 3f, .5f, true, bubble: Bubble, endID: 1));
                    }
                    chain.OnEndTrigger += Chain_OnEndTrigger;
                    ChatUI.Visible = true;
                    ChatUI.Add(chain);
                }
                if (Variant == 12 && AITimer >= 120)
                {
                    RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SlayerSpawner>(), 3);
                    AITimer = 2000;
                }
                if (AITimer >= 2000)
                {
                    SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.1f }, NPC.position);
                    DustHelper.DrawDustImage(NPC.Center, DustID.Frost, 0.1f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
                    for (int i = 0; i < 15; i++)
                    {
                        ParticleManager.NewParticle(NPC.RandAreaInEntity(), RedeHelper.SpreadUp(1), new LightningParticle(), Color.White, 3);
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 3f);
                        Main.dust[dust].velocity *= 6f;
                        Main.dust[dust].noGravity = true;
                    }
                    NPC.active = false;

                    if (Variant != 12 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        RedeWorld.slayerMessageGiven = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                }
                return;
            }

            if ((globalNPC.attacker is Player || (globalNPC.attacker is NPC spirit && spirit.Redemption().spiritSummon)) && AIState > ActionState.Scan)
                NPC.chaseable = true;

            if (AIState is not ActionState.RocketFist)
                NPC.LookByVelocity();

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
                    if (Main.rand.NextBool(200) && BaseAI.HitTileOnSide(NPC, 3))
                    {
                        if (NPC.DistanceSQ(player.Center) < 100 * 100)
                        {
                            TimerRand = 1;
                            AITimer = 0;
                            AIState = ActionState.Scan;
                        }
                        else if (RedeHelper.ClosestNPCToNPC(NPC, ref closeNPC, 100, NPC.Center) && !closeNPC.dontTakeDamage)
                        {
                            AITimer = 0;
                            AIState = ActionState.Scan;
                        }
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
                    if (Main.rand.NextBool(200) && BaseAI.HitTileOnSide(NPC, 3))
                    {
                        if (NPC.DistanceSQ(player.Center) < 100 * 100)
                        {
                            TimerRand = 1;
                            AITimer = 0;
                            AIState = ActionState.Scan;
                            NPC.netUpdate = true;
                        }
                        else if (RedeHelper.ClosestNPCToNPC(NPC, ref closeNPC, 100, NPC.Center) && !closeNPC.dontTakeDamage)
                        {
                            AITimer = 0;
                            AIState = ActionState.Scan;
                            NPC.netUpdate = true;
                        }
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1.2f, 12, 16, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Scan:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    NPC.LookAtEntity(TimerRand == 1 ? player : closeNPC);
                    AITimer++;
                    if (AITimer >= 200)
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }
                    if (AITimer == 10)
                        NPC.Shoot(NPC.Center + new Vector2(19 * NPC.spriteDirection, -8), ModContent.ProjectileType<Scan_Proj>(), 0, Vector2.Zero, CustomSounds.BallFire, NPC.whoAmI);

                    if (AITimer == 180 && !Main.dedServ)
                    {
                        if (TimerRand == 1)
                        {
                            string s = Language.GetTextValue("Mods.Redemption.Cutscene.AndroidScan.Human");
                            if (player.IsFullTBot())
                                s = Language.GetTextValue("Mods.Redemption.Cutscene.AndroidScan.Robot");
                            else if (player.RedemptionPlayerBuff().ChickenForm)
                                s = Language.GetTextValue("Mods.Redemption.Cutscene.AndroidScan.Chicken");
                            Dialogue d1 = new(NPC, s + (Language.GetTextValue("Mods.Redemption.Cutscene.AndroidScan.Scan")), Color.LightBlue, Color.DarkCyan, voice, .01f, .5f, .5f, true, bubble: Bubble); // 65
                            ChatUI.Visible = true;
                            ChatUI.Add(d1);
                        }
                        else
                        {
                            string s = closeNPC.TypeName;
                            if (closeNPC.TypeName == "")
                                s = Language.GetTextValue("Mods.Redemption.Cutscene.AndroidScan.Unknown");
                            Dialogue d1 = new(NPC, s + (Language.GetTextValue("Mods.Redemption.Cutscene.AndroidScan.Scan")), Color.LightBlue, Color.DarkCyan, voice, .01f, .5f, .5f, true, bubble: Bubble); // 65
                            ChatUI.Visible = true;
                            ChatUI.Add(d1);
                        }
                    }
                    SightCheck();
                    break;

                case ActionState.Alert:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 3))
                    {
                        runCooldown = 0;
                        AIState = ActionState.Wander;
                    }
                    else
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC others = Main.npc[i];
                            if (!others.active || others.whoAmI == NPC.whoAmI || others.ai[0] >= 3)
                                continue;

                            if (others.type != Type && others.type != ModContent.NPCType<PrototypeSilver>() && others.type != ModContent.NPCType<SpacePaladin>())
                                continue;

                            if (NPC.DistanceSQ(others.Center) >= 600 * 600)
                                continue;

                            others.GetGlobalNPC<RedeNPC>().attacker = globalNPC.attacker;
                            others.ai[1] = 0;
                            others.ai[0] = 3;
                        }
                    }
                    if (NPC.life <= NPC.lifeMax / 3 && player.Redemption().slayerStarRating <= 1)
                    {
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 60);
                        runCooldown = 0;
                        AITimer = 0;
                        AIState = ActionState.Teleport;
                    }

                    if (!NPC.Sight(globalNPC.attacker, 800, true, true))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    NPC.DamageHostileAttackers(0, 5);
                    if (Main.rand.NextBool(200) && NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) > 60 * 60)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.X = 0;
                        AIState = ActionState.RocketFist;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.15f, 2.6f, 12, 16, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.RocketFist:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 3))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (NPC.frame.Y == 19 * 46 && AITimer++ < 30)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        NPC.frameCounter = 0;
                    }
                    break;

                case ActionState.Teleport:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    if (NPC.life > NPC.lifeMax / 3)
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }

                    AITimer++;
                    if (AITimer >= 20)
                    {
                        if (AITimer % 3 == 0)
                        {
                            int dust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y - 800), NPC.width, NPC.height + 750, DustID.Frost);
                            Main.dust[dust].noGravity = true;
                        }
                    }
                    if (AITimer++ >= 180)
                    {
                        SoundEngine.PlaySound(SoundID.Item74 with { Pitch = 0.1f }, NPC.position);
                        DustHelper.DrawDustImage(NPC.Center, DustID.Frost, 0.1f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
                        for (int i = 0; i < 15; i++)
                        {
                            ParticleManager.NewParticle(NPC.RandAreaInEntity(), RedeHelper.SpreadUp(1), new LightningParticle(), Color.White, 3);
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, Scale: 3f);
                            Main.dust[dust].velocity *= 6f;
                            Main.dust[dust].noGravity = true;
                        }
                        NPC.netUpdate = true;
                        if (player.Redemption().slayerStarRating <= 1 && !NPC.AnyNPCs(ModContent.NPCType<SlayerSpawner>()))
                        {
                            player.Redemption().slayerStarRating++;
                            NPC.SetDefaults(ModContent.NPCType<SlayerSpawner>());
                        }
                        else
                            NPC.active = false;
                    }
                    break;
            }
        }
        private void CustomFrames(int frameHeight)
        {
            if (AIState is ActionState.RocketFist)
            {
                NPC.rotation = 0;
                if (NPC.frame.Y < 13 * frameHeight)
                    NPC.frame.Y = 13 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y == 16 * frameHeight)
                    {
                        int var = (int)Variant;
                        if (var >= 3)
                            var = 0;

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.MissileFire1 with { Volume = 0.5f }, NPC.position);
                        NPC.Shoot(NPC.Center + new Vector2(19 * NPC.spriteDirection, -1), ModContent.ProjectileType<Android_Proj>(), NPC.damage, new Vector2(14 * NPC.spriteDirection, 0), NPC.whoAmI, var);
                        NPC.velocity.X -= 3 * NPC.spriteDirection;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 3;
                    }
                    if (NPC.frame.Y > 18 * frameHeight)
                    {
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.frame.Y = frameHeight;
                        AIState = ActionState.Alert;
                    }
                }
                return;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 2000;
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            int var = (int)Variant;
            if (var >= 3)
                var = 0;
            NPC.frame.X = NPC.frame.Width * var;

            if (AIState is ActionState.Scan)
            {
                NPC.rotation = 0;
                NPC.frame.Y = 12 * frameHeight;
                return;
            }
            if (AIState is ActionState.RocketFist)
                return;

            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = frameHeight;
                else
                {
                    if (NPC.frame.Y < 2 * frameHeight)
                        NPC.frame.Y = 2 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 2 or <= -2)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 11 * frameHeight)
                            NPC.frame.Y = 2 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 0;
            }
        }
        public void PickType()
        {
            if (Variant >= 10)
                return;
            WeightedRandom<int> choice = new(Main.rand);
            choice.Add(0, 10); // 71%
            choice.Add(1, 4); // 28%
            choice.Add(2, 0.04f); // .28%

            Variant = choice;
            NPC.netUpdate = true;
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            if (player.Redemption().slayerStarRating > 0)
            {
                if (NPC.Sight(player, 600, false, true))
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC others = Main.npc[i];
                        if (!others.active || others.whoAmI == NPC.whoAmI || others.ai[0] >= 3)
                            continue;

                        if (others.type != Type && others.type != ModContent.NPCType<PrototypeSilver>() && others.type != ModContent.NPCType<SpacePaladin>())
                            continue;

                        if (NPC.DistanceSQ(others.Center) >= 600 * 600)
                            continue;

                        others.GetGlobalNPC<RedeNPC>().attacker = player;
                        others.ai[1] = 0;
                        others.ai[0] = 3;
                    }
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 1) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glow, NPC.Center + new Vector2(0, 1) - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override bool CanHitNPC(NPC target) => AIState == ActionState.Alert;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState == ActionState.Alert;

        public override void OnKill()
        {
            if (Variant == 10)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SlayerSpawner>(), 1);
            else if (Variant == 11 && RedeWorld.slayerRep > 2)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SlayerSpawner>(), 2);
            if (Variant == 2 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                RedeWorld.apidroidKilled = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CarbonMyofibre>(), 2, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Plating>(), 4, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Capacitor>(), 4, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AIChip>(), 8, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnergyCell>(), 20));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<P0T4T0>(), 150));
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
                    Main.dust[dustIndex].velocity *= 5;
                }
                int var = (int)Variant;
                if (var >= 3)
                    var = 0;

                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/AndroidGore1" + (var + 1)).Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/AndroidGore2" + (var + 1)).Type, 1);
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/AndroidGore3" + (var + 1)).Type, 1);
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/AndroidGore4" + (var + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (Variant < 10 && AIState is ActionState.Idle or ActionState.Wander or ActionState.Scan)
            {
                AITimer = 0;
                AIState = ActionState.Alert;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDay.Chance * (Main.hardMode ? 0.04f : 0f);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.Android")),
                new AndroidBestiaryText(Language.GetTextValue("Mods.Redemption.Bestiary.Android"))
            });
        }
    }
    public class AndroidBestiaryText : FlavorTextBestiaryInfoElement, IBestiaryInfoElement
    {
        public AndroidBestiaryText(string languageKey) : base(languageKey)
        {
        }

        public new UIElement ProvideUIElement(BestiaryUICollectionInfo info)
        {
            if (RedeWorld.apidroidKilled)
            {
                return base.ProvideUIElement(info);
            }
            return null;
        }
    }
}