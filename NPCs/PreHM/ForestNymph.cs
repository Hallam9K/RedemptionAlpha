using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Placeable.Tiles;
using Redemption.NPCs.Friendly;
using Redemption.Particles;
using Redemption.Projectiles.Hostile;
using Redemption.Projectiles.Minions;
using Redemption.UI.ChatUI;
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
using Terraria.GameContent.UI;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class ForestNymph : ModNPC
    {
        public static Asset<Texture2D> hair1;
        public static Asset<Texture2D> hair1b;
        public static Asset<Texture2D> hair1c;
        public static Asset<Texture2D> hair2;
        public static Asset<Texture2D> hair3;
        public static Asset<Texture2D> tophat;
        public static Asset<Texture2D> eye;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            hair1 = ModContent.Request<Texture2D>(Texture + "_Extra1");
            hair1b = ModContent.Request<Texture2D>(Texture + "_Extra1b");
            hair1c = ModContent.Request<Texture2D>(Texture + "_Extra1c");
            hair2 = ModContent.Request<Texture2D>(Texture + "_Extra2");
            hair3 = ModContent.Request<Texture2D>(Texture + "_Extra3");
            tophat = ModContent.Request<Texture2D>(Texture + "_Extra4");
            eye = ModContent.Request<Texture2D>(Texture + "_Eye");
        }
        public override void Unload()
        {
            hair1 = null;
            hair1b = null;
            hair1c = null;
            hair2 = null;
            hair3 = null;
            tophat = null;
            eye = null;
        }
        public enum ActionState
        {
            Idle,
            Wander,
            Alert,
            Retreating,
            Attacking,
            Slash,
            RootAtk,
            Sleeping,
            Pixie
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public enum PersonalityState
        {
            Normal, Aggressive, Calm, Shy, Jolly
        }
        public PersonalityState Personality
        {
            get => (PersonalityState)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        public int HairExtType;
        public bool HasHat;
        public int EyeType;
        public int HairType;
        public int FlowerType;
        public Vector2 EyeOffset;
        public int VisionRange;
        public int VisionIncrease;
        public float SpeedMultiplier = 1f;

        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.AllowDoorInteraction[Type] = true;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCNature[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 48;
            NPC.damage = 28;
            NPC.friendly = false;
            NPC.defense = 5;
            NPC.lifeMax = 250;
            NPC.lifeRegen = 1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 500;
            NPC.knockBackResist = 0.3f;
            NPC.rarity = 2;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ForestNymphBanner>();
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Nature] *= .75f;
        }
        public Vector2 SetEyeOffset(ref int frameHeight)
        {
            return (NPC.frame.Y / frameHeight) switch
            {
                0 => new Vector2(0, -6),
                2 => new Vector2(2, 0),
                3 => new Vector2(2, -2),
                4 => new Vector2(0, -2),
                5 => new Vector2(-2, -2),
                6 => new Vector2(0, -2),
                7 => new Vector2(-10, 0),
                8 => new Vector2(-8, 0),
                9 => new Vector2(-6, 0),
                _ => new Vector2(0, 0),
            };
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GrassBlades,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                for (int i = 0; i < 7; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/ForestNymphGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GrassBlades,
                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

            if (AIState is ActionState.Idle or ActionState.Wander or ActionState.Alert)
            {
                AITimer = 0;
                TimerRand = 0;
                AIState = ActionState.Attacking;
            }
        }
        public override bool CheckActive() => false;

        public Vector2 moveTo;
        private int runCooldown;
        public float[] doorVars = new float[3];
        public override void OnSpawn(IEntitySource source)
        {
            ChoosePersonality();
            SetStats();

            TimerRand = Main.rand.Next(80, 280);
            NPC.alpha = 0;
            NPC.netUpdate = true;
        }
        public void Emotes()
        {
            if (Main.rand.NextBool(2000) && NPC.alpha <= 10)
            {
                WeightedRandom<int> emoteID = new(Main.rand);
                switch (Personality)
                {
                    case PersonalityState.Aggressive:
                        emoteID.Add(135);
                        emoteID.Add(93);
                        break;
                    case PersonalityState.Calm:
                        emoteID.Add(136);
                        emoteID.Add(16);
                        break;
                    case PersonalityState.Shy:
                        emoteID.Add(16);
                        emoteID.Add(134);
                        break;
                    case PersonalityState.Jolly:
                        emoteID.Add(15);
                        emoteID.Add(136);
                        break;
                }
                emoteID.Add(17, 0.4);
                emoteID.Add(14, 0.4);
                emoteID.Add(23, 0.3);
                emoteID.Add(4, 0.3);
                emoteID.Add(6, 0.3);
                emoteID.Add(95, 0.3);
                emoteID.Add(13, 0.4);
                emoteID.Add(62, 0.4);
                emoteID.Add(63, 0.4);
                emoteID.Add(68, 0.4);
                EmoteBubble.NewBubble(emoteID, new WorldUIAnchor(NPC), 120);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HairExtType);
            writer.Write(HasHat);
            writer.Write(EyeType);
            writer.Write(HairType);
            writer.Write(FlowerType);
            writer.WriteVector2(moveTo);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HairExtType = reader.ReadByte();
            HasHat = reader.ReadBoolean();
            EyeType = reader.ReadByte();
            HairType = reader.ReadByte();
            FlowerType = reader.ReadByte();
            moveTo = reader.ReadVector2();
        }
        private int YippieeTimer;
        public override void AI()
        {
            CustomFrames(94);

            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            NPC.TargetClosest();
            if (AIState is not ActionState.Alert)
                NPC.LookByVelocity();
            if (AIState is not ActionState.Idle)
                YippieeTimer = 0;
            RegenCheck();
            if ((globalNPC.attacker is Player || (globalNPC.attacker is NPC spirit && spirit.Redemption().spiritSummon)) && AIState > ActionState.Alert && AIState is not ActionState.Sleeping)
                NPC.chaseable = true;

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
                    if (Main.rand.NextBool(300) && Personality is PersonalityState.Jolly && BaseAI.HitTileOnSide(NPC, 3))
                        YippieeTimer = 1;
                    if (YippieeTimer > 0)
                    {
                        YippieeTimer++;
                        EyeState = 1;
                        if ((YippieeTimer == 5 || YippieeTimer == 26) && BaseAI.HitTileOnSide(NPC, 3))
                            NPC.velocity.Y -= 3;
                        if (YippieeTimer >= 50)
                            YippieeTimer = 0;
                    }
                    else
                    {
                        YippieeTimer = 0;
                        EyeState = 0;
                    }

                    Point tileBelow = NPC.Bottom.ToTileCoordinates();
                    Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);
                    if (Main.rand.NextBool(500) && !Main.dayTime && tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType] && TileLists.WoodLeaf.Contains(tile.TileType) && (Framing.GetTileSafely(NPC.Center).WallType == WallID.LivingWoodUnsafe || Framing.GetTileSafely(NPC.Center).WallType == WallID.LivingWood))
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(3000, 6000);
                        AIState = ActionState.Sleeping;
                        NPC.netUpdate = true;
                    }

                    Emotes();
                    SightCheck();
                    break;

                case ActionState.Wander:
                    SightCheck();
                    EyeState = 0;

                    AITimer++;
                    if (AITimer >= TimerRand || NPC.Center.X + 20 > moveTo.X * 16 && NPC.Center.X - 20 < moveTo.X * 16)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(80, 280);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, 10, interactDoorStyle: 2);

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, (moveTo.Y - 32) * 16);
                    NPCHelper.HorizontallyMove(NPC, moveTo * 16, 0.4f, 1 * SpeedMultiplier, 12, 8, NPC.Center.Y > moveTo.Y * 16);
                    break;

                case ActionState.Alert:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    EyeState = 2;
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 1))
                    {
                        runCooldown = 0;
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                        break;
                    }
                    NPC.LookAtEntity(globalNPC.attacker);

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, false, EyeType != 4, false, EyeType == 4, headOffset: 30))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (AITimer++ == 0)
                    {
                        if ((globalNPC.attacker is NPC attackerNPC2 && attackerNPC2.life >= NPC.life) || NPC.DistanceSQ(globalNPC.attacker.Center) <= 100 * 100)
                        {
                            EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 120);
                            AITimer = 0;
                            TimerRand = 0;
                            AIState = ActionState.Attacking;
                        }
                        else
                        {
                            if (Personality is PersonalityState.Shy)
                                EmoteBubble.NewBubble(2, new WorldUIAnchor(NPC), 60);
                            else if (Personality is PersonalityState.Aggressive)
                                EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 60);
                            else
                                EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 60);
                        }
                    }
                    else
                    {
                        int threatRange = 200;
                        int threatTime = 300;
                        switch (Personality)
                        {
                            case PersonalityState.Aggressive:
                                threatRange += 200;
                                threatTime = 240;
                                break;
                            case PersonalityState.Calm:
                                threatRange -= 100;
                                threatTime = 600;
                                break;
                            case PersonalityState.Shy:
                                threatRange += 400;
                                threatTime = 120;
                                break;
                            case PersonalityState.Jolly:
                                threatRange = 40;
                                threatTime = 900;
                                break;
                        }
                        if (NPC.Sight(globalNPC.attacker, threatRange, false, EyeType != 4, false, EyeType == 4, headOffset: 30))
                            TimerRand++;

                        if (TimerRand == threatTime / 2)
                        {
                            if (Personality is not PersonalityState.Shy)
                                EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 60);
                        }
                        if (TimerRand >= threatTime)
                        {
                            AITimer = 0;
                            TimerRand = 0;
                            AIState = ActionState.Attacking;
                        }
                    }
                    break;

                case ActionState.Attacking:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 1))
                    {
                        runCooldown = 0;
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }
                    EyeState = 0;
                    BaseAI.AttemptOpenDoor(NPC, ref doorVars[0], ref doorVars[1], ref doorVars[2], 80, 1, 10, interactDoorStyle: 2);

                    if (!NPC.Sight(globalNPC.attacker, VisionRange, false, EyeType != 4, false, EyeType == 4, headOffset: 30))
                        runCooldown++;
                    else if (runCooldown > 0)
                        runCooldown--;

                    if (NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) < 60 * 60)
                    {
                        NPC.LookAtEntity(globalNPC.attacker);
                        AITimer = 0;
                        NPC.frameCounter = 0;
                        NPC.velocity.X = 0;
                        AIState = ActionState.Slash;
                    }
                    if (Main.rand.NextBool(200) && NPC.velocity.Y == 0 && NPC.DistanceSQ(globalNPC.attacker.Center) > 100 * 100)
                    {
                        AITimer = 0;
                        AIState = ActionState.RootAtk;
                        NPC.netUpdate = true;
                    }

                    NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20, globalNPC.attacker.Center.Y);
                    if ((globalNPC.attacker is NPC attackerNPC && attackerNPC.life >= NPC.life) || Personality is PersonalityState.Shy)
                    {
                        NPCHelper.HorizontallyMove(NPC, new Vector2(NPC.Center.X + (100 * NPC.RightOfDir(globalNPC.attacker)), NPC.Center.Y), 0.2f, 2f * SpeedMultiplier, 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                        break;
                    }
                    NPCHelper.HorizontallyMove(NPC, globalNPC.attacker.Center, 0.2f, 2f * SpeedMultiplier, 12, 8, NPC.Center.Y > globalNPC.attacker.Center.Y, globalNPC.attacker);
                    break;

                case ActionState.Slash:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 1))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }
                    NPC.LookAtEntity(globalNPC.attacker);
                    Rectangle SlashHitbox = new((int)(NPC.spriteDirection == -1 ? NPC.Center.X - 50 : NPC.Center.X), (int)(NPC.Center.Y - 33), 50, 80);

                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y = 0;
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.9f;

                    if (NPC.frame.Y == 6 * 94 && AITimer++ < 10)
                        NPC.frameCounter = 0;
                    if (NPC.frame.Y == 6 * 94 && AITimer == 9 && NPC.Hitbox.Intersects(globalNPC.attacker.Hitbox))
                        NPC.velocity.X -= 6 * NPC.spriteDirection;

                    if (NPC.frame.Y == 7 * 94 && globalNPC.attacker.Hitbox.Intersects(SlashHitbox))
                    {
                        int damage = NPC.RedemptionNPCBuff().disarmed ? NPC.damage / 3 : NPC.damage;
                        if (globalNPC.attacker is NPC attackerNPC2 && attackerNPC2.immune[NPC.whoAmI] <= 0)
                        {
                            attackerNPC2.immune[NPC.whoAmI] = 10;
                            int hitDirection = attackerNPC2.RightOfDir(NPC);
                            BaseAI.DamageNPC(attackerNPC2, damage, 5, hitDirection, NPC);
                        }
                        else if (globalNPC.attacker is Player attackerPlayer)
                        {
                            int hitDirection = attackerPlayer.RightOfDir(NPC);
                            BaseAI.DamagePlayer(attackerPlayer, damage, 5, hitDirection, NPC);
                        }
                    }
                    break;

                case ActionState.RootAtk:
                    if (NPC.ThreatenedCheck(ref runCooldown, 300, 1))
                    {
                        runCooldown = 0;
                        AITimer = 0;
                        moveTo = NPC.FindGround(20);
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Wander;
                        NPC.netUpdate = true;
                    }
                    EyeState = 1;
                    for (int i = 0; i < 2; i++)
                    {
                        int dustIndex = Dust.NewDust(NPC.BottomLeft, NPC.width, 1, DustID.DryadsWard, 0f, 0f, 100, default, 1);
                        Main.dust[dustIndex].velocity.Y -= 4f;
                        Main.dust[dustIndex].velocity.X *= 0f;
                        Main.dust[dustIndex].noGravity = true;
                    }

                    NPC.velocity.X *= 0.5f;

                    AITimer++;
                    if (AITimer == 5)
                    {
                        int tilePosY = BaseWorldGen.GetFirstTileFloor((int)(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30)) / 16, (int)(globalNPC.attacker.Bottom.Y / 16) - 2);
                        NPC.Shoot(new Vector2(globalNPC.attacker.Center.X + (globalNPC.attacker.velocity.X * 30), (tilePosY * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero);
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC target = Main.npc[i];
                            if (!target.active || target.whoAmI == NPC.whoAmI || target.whoAmI == globalNPC.attacker.whoAmI || target.Redemption().invisible)
                                continue;

                            if (target.lifeMax < 5 || target.damage == 0 || NPC.DistanceSQ(target.Center) > 400 * 400 || target.type == NPC.type || NPCLists.Plantlike.Contains(target.type))
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Bottom.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero);
                        }
                        for (int p = 0; p < Main.maxPlayers; p++)
                        {
                            Player target = Main.player[p];
                            if (globalNPC.attacker is NPC)
                                continue;

                            if (!target.active || NPC.DistanceSQ(target.Center) > 400 * 400)
                                continue;

                            if (Main.rand.NextBool(3))
                                continue;

                            int tilePosY2 = BaseWorldGen.GetFirstTileFloor((int)(target.Center.X + (target.velocity.X * 30)) / 16, (int)(target.Bottom.Y / 16) - 2);
                            NPC.Shoot(new Vector2(target.Center.X + (target.velocity.X * 30), (tilePosY2 * 16) + 30), ModContent.ProjectileType<LivingBloomRoot>(), NPC.damage, Vector2.Zero);
                        }
                    }
                    else if (AITimer >= 80)
                    {
                        AITimer = 0;
                        AIState = ActionState.Attacking;
                    }
                    break;

                case ActionState.Sleeping:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;
                    EyeState = 1;
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    if (AITimer % 300 == 0)
                        EmoteBubble.NewBubble(89, new WorldUIAnchor(NPC), 180);

                    if (Main.dayTime || NPC.velocity.Length() > 0.01f || NPC.Sight(player, 100, false, false, false, true, 4, 40))
                    {
                        moveTo = NPC.FindGround(20);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(120, 260);
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;

                case ActionState.Pixie:
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X = 0;

                    EyeState = 0;
                    NPC.dontTakeDamage = true;
                    if (AITimer++ == 30)
                        EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 120);
                    if (AITimer == 220)
                        EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 60);
                    if (AITimer == 300 && !Main.dedServ)
                    {
                        Texture2D bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra").Value;
                        SoundStyle voice = CustomSounds.Voice3 with { Pitch = 1.3f };

                        string line1 = Personality switch
                        {
                            PersonalityState.Calm => "Your fae says you're kind-hearted.[0.5] I will put it in good faith.",
                            PersonalityState.Shy => "Your fae tells me you're safe.[0.5] I'm still rather sceptical,[0.1] but I'll trust it's words.",
                            PersonalityState.Jolly => "Your fae just told me how pleasant you are.[0.5] A friend of nature is a friend of me.",
                            PersonalityState.Aggressive => "Are you sure, fae?[0.5] This human doesn't look very kindly,[0.1] but I will trust your word.",
                            _ => "Your fae says you have a good heart.[0.5] I will be less wary of you from now on.",
                        };
                        DialogueChain chain = new();
                        chain.Add(new(NPC, line1 + "[@End]", Color.LightGreen, Color.ForestGreen, voice, .05f, 2, .5f, true, bubble: bubble));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    break;
            }
            if (!Main.rand.NextBool(20) || !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                return;
            ParticleManager.NewParticle(NPC.RandAreaInEntity(), RedeHelper.Spread(2), new SpiritParticle(), Color.White, .5f);
        }
        private void CustomFrames(int frameHeight)
        {
            if (AIState is ActionState.Slash)
            {
                NPC.rotation = 0;

                if (NPC.frame.Y < 4 * frameHeight)
                    NPC.frame.Y = 4 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y == 7 * frameHeight)
                    {
                        SoundEngine.PlaySound(SoundID.Item71 with { Volume = .7f }, NPC.position);
                        NPC.velocity.X += 4 * NPC.spriteDirection;
                    }
                    if (NPC.frame.Y > 8 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0;
                        AIState = ActionState.Attacking;
                    }
                }
                EyeOffset = SetEyeOffset(ref frameHeight);
                return;
            }
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            Main.BestiaryTracker.Kills.RegisterKill(NPC);

            PersonalityState p = Personality;
            NPC.SetDefaults(ModContent.NPCType<ForestNymph_Friendly>());
            (Main.npc[NPC.whoAmI].ModNPC as ForestNymph_Friendly).EyeType = EyeType;
            (Main.npc[NPC.whoAmI].ModNPC as ForestNymph_Friendly).Personality = p;
            (Main.npc[NPC.whoAmI].ModNPC as ForestNymph_Friendly).HairExtType = HairExtType;
            (Main.npc[NPC.whoAmI].ModNPC as ForestNymph_Friendly).HairType = HairType;
            (Main.npc[NPC.whoAmI].ModNPC as ForestNymph_Friendly).FlowerType = FlowerType;
            (Main.npc[NPC.whoAmI].ModNPC as ForestNymph_Friendly).HasHat = HasHat;
            NPC.GivenName = NPC.getNewNPCName();
            TimerRand = Main.rand.Next(80, 280);
            NPC.alpha = 0;
            NPC.netUpdate = true;
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        private int EyeFrame;
        private int EyeFrameCounter;
        private int EyeState;
        public override void FindFrame(int frameHeight)
        {
            switch (EyeState)
            {
                case 0:
                    if (EyeFrameCounter++ % 10 == 0)
                    {
                        if (EyeFrame >= 2)
                            EyeFrame = 0;
                        if (EyeFrame == 1)
                            EyeFrame++;
                        if (EyeFrame == 0 && Main.rand.NextBool(20))
                            EyeFrame = 1;
                    }
                    break;
                case 1:
                    if (EyeFrameCounter++ % 10 == 0)
                    {
                        if (EyeFrame >= 2)
                            EyeFrame = 2;
                        if (EyeFrame == 1)
                            EyeFrame++;
                        if (EyeFrame == 0)
                            EyeFrame = 1;
                    }
                    break;
                case 2:
                    EyeFrame = 1;
                    break;
            }
            if (AIState is ActionState.Slash)
                return;
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = frameHeight;
                else
                {
                    if (NPC.frame.Y < frameHeight)
                        NPC.frame.Y = frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter is >= 3 or <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 4 * frameHeight)
                            NPC.frame.Y = frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 0;
            }
            EyeOffset = SetEyeOffset(ref frameHeight);
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

                if (target.lifeMax <= 5 || target.friendly || target.damage <= 0 || NPCLists.Plantlike.Contains(target.type))
                    continue;

                if (nearestNPCDist != -1 && !(target.Distance(NPC.Center) < nearestNPCDist))
                    continue;

                nearestNPCDist = target.Distance(NPC.Center);
                nearestNPC = target.whoAmI;
            }

            return nearestNPC;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dark.Contains(target.type))
                modifiers.FinalDamage *= 1.5f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.DryadsWardDebuff, 300);
        }
        public void SightCheck()
        {
            Player player = Main.player[NPC.target];
            RedeNPC globalNPC = NPC.Redemption();
            int gotNPC = GetNearestNPC();
            if (NPC.type != ModContent.NPCType<ForestNymph_Friendly>() && Personality is not PersonalityState.Jolly && player.ownedProjectileCounts[ModContent.ProjectileType<NaturePixie>()] == 0)
            {
                if (NPC.Sight(player, VisionRange, EyeType != 4, EyeType != 4, false, EyeType == 4, headOffset: 30))
                {
                    globalNPC.attacker = player;
                    moveTo = NPC.FindGround(20);
                    AITimer = 0;
                    TimerRand = 0;
                    AIState = ActionState.Alert;
                    NPC.netUpdate = true;
                }
            }
            if (gotNPC != -1 && NPC.Sight(Main.npc[gotNPC], VisionRange, EyeType != 4, EyeType != 4, false, EyeType == 4, headOffset: 30))
            {
                globalNPC.attacker = Main.npc[gotNPC];
                moveTo = NPC.FindGround(20);
                AITimer = 0;
                TimerRand = 0;
                AIState = ActionState.Alert;
                NPC.netUpdate = true;
            }
        }
        public void ChoosePersonality()
        {
            WeightedRandom<byte> hair = new(Main.rand);
            hair.Add(0);
            hair.Add(1, 0.5);
            hair.Add(2, 0.5);
            hair.Add(3, 0.1);
            HairType = hair;
            FlowerType = (byte)Main.rand.Next(6);
            HairExtType = (byte)Main.rand.Next(3);
            if (Main.rand.NextBool(10))
                HasHat = true;
            WeightedRandom<byte> eyes = new(Main.rand);
            eyes.Add(0);
            eyes.Add(1);
            eyes.Add(2);
            eyes.Add(3);
            eyes.Add(4, 0.1);
            EyeType = eyes;

            WeightedRandom<PersonalityState> choice = new(Main.rand);
            choice.Add(PersonalityState.Normal, 10); // 37%
            choice.Add(PersonalityState.Calm, 5); // 18.5%
            choice.Add(PersonalityState.Aggressive, 6); // 22%
            choice.Add(PersonalityState.Shy, 3); // 11%
            choice.Add(PersonalityState.Jolly, 3);

            Personality = choice;
            NPC.netUpdate = true;
        }
        public void SetStats()
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
                case PersonalityState.Shy:
                    NPC.lifeMax = (int)(NPC.lifeMax * 0.8f);
                    NPC.life = (int)(NPC.life * 0.8f);
                    NPC.defense = (int)(NPC.defense * 1.15f);
                    NPC.damage = (int)(NPC.damage * 0.9f);
                    VisionIncrease = 200;
                    SpeedMultiplier = 1.6f;
                    break;
                case PersonalityState.Jolly:
                    NPC.lifeMax = (int)(NPC.lifeMax * 1.1f);
                    NPC.life = (int)(NPC.life * 1.1f);
                    SpeedMultiplier = 1.8f;
                    break;
            }
            if (EyeType == 5)
                VisionRange = 100;
            else
                VisionRange = 600 + VisionIncrease;
            NPC.netUpdate = true;
        }
        int regenTimer;
        public void RegenCheck()
        {
            int regenCooldown = NPC.wet && !NPC.lavaWet ? 30 : 40;
            if ((NPC.wet && !NPC.lavaWet) || NPC.HasBuff(BuffID.Wet) || (Main.raining && NPC.position.Y < Main.worldSurface * 16 && Framing.GetTileSafely(NPC.Center).WallType == WallID.None))
            {
                regenTimer++;
                if (regenTimer % regenCooldown == 0 && NPC.life < NPC.lifeMax)
                {
                    int heal = NPC.type == ModContent.NPCType<ForestNymph_Friendly>() ? 10 : 2;
                    NPC.life += heal;
                    NPC.HealEffect(heal);
                    if (NPC.life > NPC.lifeMax)
                        NPC.life = NPC.lifeMax;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 pos = NPC.Center + new Vector2(NPC.spriteDirection == -1 ? -19 : 17, -17);

            if (!NPC.IsABestiaryIconDummy && Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
            {
                int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                drawColor = Color.Black * .8f;
            }
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            int EyeHeight = eye.Value.Height / 3;
            int EyeWidth = eye.Value.Width / 6;
            int EyeY = EyeHeight * EyeFrame;
            int EyeX = EyeWidth * EyeType;
            Rectangle EyeRect = new(EyeX, EyeY, EyeWidth, EyeHeight);
            spriteBatch.Draw(eye.Value, pos - screenPos, new Rectangle?(EyeRect), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? -56 : -28, -32) - new Vector2(EyeOffset.X * -NPC.spriteDirection, EyeOffset.Y), NPC.scale, effects, 0);

            int Height = hair1.Value.Height / 10;
            int Width = hair1.Value.Width / 4;
            int Height2 = hair2.Value.Height / 10;
            int Width2 = hair2.Value.Width / 4;
            int Width3 = hair3.Value.Width / 5;
            int y = Height * (NPC.frame.Y / 94);
            int x = Width * HairType;
            int y2 = Height2 * (NPC.frame.Y / 94);
            int x2 = Width2 * HairType;
            int x3 = Width3 * FlowerType;
            Rectangle rect = new(x, y, Width, Height);
            Rectangle rect2 = new(x2, y2, Width2, Height2);
            Rectangle rect3 = new(x3, 0, Width3, hair3.Value.Height);
            Texture2D h1 = hair1.Value;
            switch (HairExtType)
            {
                case 1:
                    h1 = hair1b.Value;
                    break;
                case 2:
                    h1 = hair1c.Value;
                    Height = hair1c.Value.Height / 10;
                    Width = hair1c.Value.Width / 4;
                    y = Height * (NPC.frame.Y / 94);
                    x = Width * HairType;
                    rect = new(x, y, Width, Height);
                    break;
            }
            spriteBatch.Draw(h1, pos - screenPos, new Rectangle?(rect), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? -58 : 6, -12) - (HairExtType == 2 ? new Vector2(NPC.spriteDirection == -1 ? -2 : 8, -2) : Vector2.Zero), NPC.scale, effects, 0);

            spriteBatch.Draw(hair2.Value, pos - screenPos, new Rectangle?(rect2), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? -34 : -18, -16), NPC.scale, effects, 0);
            if (FlowerType <= 4)
                spriteBatch.Draw(hair3.Value, pos - screenPos, new Rectangle?(rect3), drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? -34 : -18, -16) - new Vector2(EyeOffset.X * -NPC.spriteDirection, EyeOffset.Y), NPC.scale, effects, 0);

            if (HasHat)
                spriteBatch.Draw(tophat.Value, pos - screenPos, null, drawColor, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? -44 : -24, -10) - new Vector2(EyeOffset.X * -NPC.spriteDirection, EyeOffset.Y), NPC.scale, effects, 0);
            if (!NPC.IsABestiaryIconDummy && Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
            {
                EyeRect = new(20, EyeY, EyeWidth, EyeHeight);
                spriteBatch.Draw(eye.Value, pos - screenPos, new Rectangle?(EyeRect), Color.White, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == -1 ? -56 : -28, -32) - new Vector2(EyeOffset.X * -NPC.spriteDirection, EyeOffset.Y), NPC.scale, effects, 0);

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            return false;
        }
        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.HerbBag, 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantMatter>(), 2, 8, 14));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnglonicMysticBlossom>(), 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ForestCore>(), 8));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Nymph, false);
            foreach (var dropRule in dropRules)
            {
                npcLoot.Add(dropRule);
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int score = 0;
            for (int x = -100; x <= 100; x++)
            {
                for (int y = -100; y <= 100; y++)
                {
                    int type = Framing.GetTileSafely(spawnInfo.SpawnTileX + x, spawnInfo.SpawnTileY + y).TileType;
                    if (type == TileID.LivingWood)
                        score++;
                }
            }
            int[] TileArray = { TileID.Grass, TileID.LivingWood };

            float baseChance = SpawnCondition.OverworldDay.Chance * (!NPC.AnyNPCs(NPC.type) && !NPC.AnyNPCs(ModContent.NPCType<ForestNymph_Friendly>()) ? 1 : 0);
            float multiplier = TileArray.Contains(Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY).TileType) ? (Main.raining ? 0.08f : 0.04f) : 0f;
            float pixie = spawnInfo.Player.HasBuff<NaturePixieBuff>() ? 10 : 1;
            float trees = score >= 5 ? 1 : 0;

            return baseChance * multiplier * trees * pixie;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], true, 1);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.ForestNymph"))
            });
        }
    }
}