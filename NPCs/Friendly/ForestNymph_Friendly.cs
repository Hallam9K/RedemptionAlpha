using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.PreHM;
using Redemption.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class ForestNymph_Friendly : ForestNymph
    {
        public override string Texture => "Redemption/NPCs/PreHM/ForestNymph";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Forest Nymph");
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.AllowDoorInteraction[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.townNPC = true;
            NPC.friendly = true;
            if (RedeQuest.forestNymphVar < 4)
                TownNPCStayingHomeless = true;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax *= 2;
            NPC.damage *= 2;
            NPC.defense *= 2;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.hardMode)
                modifiers.FinalDamage *= 2;
        }
        public override bool UsesPartyHat() => false;
        public override bool CanChat() => AIState <= ActionState.Wander;
        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Nyssa",
                "Ammi",
                "Alderis",
                "Maple",
                "Lavender",
                "Ambrose",
                "Nelida",
                "Syllessa"
            };
        }
        private bool setStats;
        private int playerFollow;
        public override void PostAI()
        {
            if (++NPC.breath <= 0)
                NPC.breath = 9000;
            if (!setStats)
            {
                if (Main.expertMode)
                {
                    NPC.lifeMax *= Main.masterMode ? 3 : 2;
                    NPC.life = NPC.lifeMax;
                    NPC.damage *= Main.masterMode ? 3 : 2;
                    NPC.defense *= Main.masterMode ? 3 : 2;
                }
                SetStats();
                if (RedeQuest.forestNymphVar > 1)
                {
                    NPC.lifeMax += 500;
                    NPC.life += 500;
                }
                setStats = true;
            }
            if (RedeQuest.forestNymphVar < 4)
                NPC.homeless = true;
            else
                TownNPCStayingHomeless = false;

            if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].whoAmI == NPC.whoAmI)
            {
                NPC.LookAtEntity(Main.LocalPlayer);
                AIState = ActionState.Idle;
                AITimer = 0;
            }
            else if (following && (AIState is ActionState.Idle or ActionState.Wander) && playerFollow != -1)
            {
                AITimer = 0;
                AIState = ActionState.Wander;
                moveTo = Main.player[playerFollow].Center / 16;
                if (NPC.Center.X > Main.player[playerFollow].Center.X - 2 && NPC.Center.X < Main.player[playerFollow].Center.X + 2)
                    NPC.velocity.X = 0;
            }
            if (!Main.dayTime && !NPC.homeless && !following)
            {
                if (!IsNpcOnscreen(NPC.Center) && Vector2.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), new Vector2(NPC.homeTileX, NPC.homeTileY) * 16) > Main.screenWidth / 2 + 100)
                {
                    NPC.Center = new Vector2(NPC.homeTileX, NPC.homeTileY - 2) * 16;
                    if (AIState <= ActionState.Wander)
                        AIState = ActionState.Sleeping;
                }
            }
        }
        private static bool IsNpcOnscreen(Vector2 center)
        {
            int w = NPC.sWidth + NPC.safeRangeX * 2;
            int h = NPC.sHeight + NPC.safeRangeY * 2;
            Rectangle npcScreenRect = new((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
            foreach (Player player in Main.player)
            {
                if (player.active && player.getRect().Intersects(npcScreenRect)) return true;
            }
            return false;
        }
        public override bool CheckConditions(int left, int right, int top, int bottom)
        {
            int score = 0;
            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    int type = Main.tile[x, y].TileType;
                    if (type is TileID.LivingWood or TileID.LivingMahoganyLeaves or TileID.LivingMahogany or TileID.LeafBlock)
                        score++;

                    if (Main.tile[x, y].WallType is WallID.LivingWood or WallID.LivingLeaf)
                        score++;
                }
            }

            return score >= (right - left) * (bottom - top) / 2;
        }
        public static int ChatNumber = 0;
        public bool following;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            if ((RedeWorld.alignment < 0 && !RedeBossDowned.downedTreebark) || (RedeWorld.alignment < 2 && RedeBossDowned.downedTreebark))
                return;
            button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.Cycle");
            switch (ChatNumber)
            {
                case 0:
                    switch (RedeQuest.forestNymphVar)
                    {
                        default:
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.1");
                            break;
                        case 1:
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.2");
                            break;
                        case 2:
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.3");
                            break;
                        case 3:
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.4");
                            break;
                        case 4:
                            if (NPC.homeless)
                                button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.HomeRequirements");
                            else
                                button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.5");
                            break;
                        case 5:
                            button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Talk");
                            break;
                    }
                    break;
                case 1:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Trade");
                    break;
                case 2:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Blessing");
                    break;
                case 3:
                    if (!following)
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Follow");
                    else
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.SFollow");
                    break;
                case 4:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.DHair");
                    break;
                case 5:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.SHair");
                    break;
                case 6:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Crux");
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.LocalPlayer;
            if (firstButton)
            {
                switch (ChatNumber)
                {
                    case 0:
                        if (RedeQuest.forestNymphVar == 0)
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);

                            int HerbBag = player.FindItem(ItemID.HerbBag);
                            if (HerbBag >= 0)
                            {
                                player.inventory[HerbBag].stack--;
                                if (player.inventory[HerbBag].stack <= 0)
                                    player.inventory[HerbBag] = new Item();

                                string line = Personality switch
                                {
                                    PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueCalm"),
                                    PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueShy"),
                                    PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueJolly"),
                                    PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueAggressive"),
                                    _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogue"),
                                };
                                Main.npcChatText = line;

                                RedeQuest.forestNymphVar++;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);

                                player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<ForestCore>());
                                Main.npcChatCornerItem = ModContent.ItemType<ForestCore>();
                                SoundEngine.PlaySound(SoundID.Chat);
                                return;
                            }
                            else
                            {
                                Main.npcChatCornerItem = ItemID.HerbBag;
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                        }
                        else if (RedeQuest.forestNymphVar == 1)
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);

                            int Flower = player.FindItem(ModContent.ItemType<AnglonicMysticBlossom>());
                            if (Flower >= 0)
                            {
                                player.inventory[Flower].stack--;
                                if (player.inventory[Flower].stack <= 0)
                                    player.inventory[Flower] = new Item();

                                string line = Personality switch
                                {
                                    PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueCalm"),
                                    PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueShy"),
                                    PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueJolly"),
                                    PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueAggressive"),
                                    _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogue"),
                                };
                                Main.npcChatText = line + Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueCont");
                                NPC.lifeMax += 500;
                                NPC.life += 500;
                                RedeQuest.forestNymphVar++;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);

                                player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<ForestNymphsSickle>());
                                Main.npcChatCornerItem = ModContent.ItemType<ForestNymphsSickle>();
                                SoundEngine.PlaySound(SoundID.Chat);
                                return;
                            }
                            else
                            {
                                Main.npcChatCornerItem = ModContent.ItemType<AnglonicMysticBlossom>();
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                        }
                        else if (RedeQuest.forestNymphVar == 2)
                        {
                            SoundEngine.PlaySound(SoundID.Chat);
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeDialogue1");
                            RedeQuest.forestNymphVar++;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);

                        }
                        else if (RedeQuest.forestNymphVar >= 3)
                        {
                            SoundEngine.PlaySound(SoundID.Chat);
                            if (RedeQuest.forestNymphVar >= 5)
                            {
                                Main.npcChatText = ChitChat();
                                break;
                            }
                            if (RedeQuest.forestNymphVar == 3)
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeDialogue2");
                                RedeQuest.forestNymphVar++;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            else if (RedeQuest.forestNymphVar == 4)
                            {
                                if (NPC.homeless)
                                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeDialogue3");
                                else
                                {
                                    int score = 0;
                                    for (int x = -40; x <= 40; x++)
                                    {
                                        for (int y = -25; y <= 25; y++)
                                        {
                                            Tile tile = Framing.GetTileSafely(NPC.homeTileX + x, NPC.homeTileY + y);
                                            if (tile.LiquidAmount >= 255 && tile.LiquidType == LiquidID.Water)
                                                score++;
                                        }
                                    }
                                    if (score < 20)
                                    {
                                        string line = Personality switch
                                        {
                                            PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueCalm"),
                                            PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueShy"),
                                            PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueJolly"),
                                            PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueAggressive"),
                                            _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogue"),
                                        };
                                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueStart") + line + Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.NeedWaterRequirement", score.ToString());
                                    }
                                    else
                                    {
                                        string line = Personality switch
                                        {
                                            PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueCalm"),
                                            PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueShy"),
                                            PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueJolly"),
                                            PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueAggressive"),
                                            _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogue"),
                                        };
                                        Main.npcChatText = line + Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueCont");

                                        if (RedeQuest.forestNymphVar < 5)
                                        {
                                            RedeWorld.alignment++;
                                            for (int p = 0; p < Main.maxPlayers; p++)
                                            {
                                                Player player2 = Main.player[p];
                                                if (!player2.active)
                                                    continue;

                                                CombatText.NewText(player2.getRect(), Color.Gold, "+1", true, false);

                                                if (!RedeWorld.alignmentGiven)
                                                    continue;

                                                if (!Main.dedServ)
                                                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.ForestNymphHoused"), 240, 30, 0, Color.DarkGoldenrod);

                                            }
                                            RedeQuest.forestNymphVar = 5;
                                        }
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.WorldData);
                                    }
                                }
                            }
                        }
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        TradeUI.Visible = true;
                        break;
                    case 2:
                        for (int i = 0; i < 20; i++)
                        {
                            int dustIndex = Dust.NewDust(new Vector2(Main.LocalPlayer.position.X, Main.LocalPlayer.Bottom.Y - 2), Main.LocalPlayer.width, 2, DustID.DryadsWard);
                            Main.dust[dustIndex].velocity.Y = -Main.rand.Next(3, 7);
                            Main.dust[dustIndex].velocity.X = 0;
                            Main.dust[dustIndex].noGravity = true;
                        }
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.position);
                        Main.LocalPlayer.AddBuff(BuffID.Lucky, 10800 * ((RedeWorld.alignment / 2) + 1));
                        break;
                    case 3:
                        playerFollow = Main.LocalPlayer.whoAmI;
                        following = !following;
                        if (following)
                        {
                            string line = Personality switch
                            {
                                PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueCalm"),
                                PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueShy"),
                                PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueJolly"),
                                PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueAggressive"),
                                _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogue"),
                            };
                            Main.npcChatText = line;
                        }
                        break;
                    case 4:
                        FlowerType++;
                        if (FlowerType > 5)
                            FlowerType = 0;
                        break;
                    case 5:
                        HairExtType++;
                        if (HairExtType > 2)
                            HairExtType = 0;
                        break;
                    case 6:
                        if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.NoRealmCruxDialogue");
                            ChatNumber--;
                            return;
                        }
                        int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                        if (card >= 0)
                        {
                            Main.LocalPlayer.inventory[card].stack--;
                            if (Main.LocalPlayer.inventory[card].stack <= 0)
                                Main.LocalPlayer.inventory[card] = new Item();

                            Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardForestNymph>());
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.CruxDialogue");
                            Main.npcChatCornerItem = ModContent.ItemType<CruxCardForestNymph>();
                            SoundEngine.PlaySound(SoundID.Chat);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.NoCruxDialogue");
                            Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                        }
                        ChatNumber--;
                        break;
                }
            }
            else
            {
                bool skip = true;
                while (skip)
                {
                    ChatNumber++;
                    if (ChatNumber > 6)
                        ChatNumber = 0;
                    if (RedeQuest.forestNymphVar < 1 && (ChatNumber == 4 || ChatNumber == 5))
                        skip = true;
                    else if (RedeWorld.alignment <= 0 && ChatNumber == 2)
                        skip = true;
                    else if (RedeQuest.forestNymphVar < 2 && ChatNumber == 3)
                        skip = true;
                    else if (RedeQuest.forestNymphVar < 5 && ChatNumber == 1)
                        skip = true;
                    else if (ChatNumber == 6 && (RedeQuest.forestNymphVar < 5 || !Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive || Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardForestNymph>())))
                        skip = true;
                    else
                        skip = false;
                }
            }
        }
        private string ChitChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            string bothChat = BothChat();
            if (bothChat != null)
                chat.Add(bothChat);
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue1"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue2"));
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue3"));
            string s = "";
            if (NPC.GivenName == "Nyssa")
                s = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue4Const");
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue4") + s);
            if (RedeWorld.alignment >= 4)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue5"));
            else
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue6"));
            return chat;
        }
        private string BothChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            Player player = Main.player[Main.myPlayer];
            if (RedeBossDowned.downedTreebark)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue7"));
            if (RedeBossDowned.downedADD)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue8"));
            if (RedeBossDowned.nukeDropped)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue9"));
            if (BasePlayer.HasArmorSet(player, "Common Guard", true) || BasePlayer.HasArmorSet(player, "Common Guard", false))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue10"));
            }
            if (RedeBossDowned.downedThorn)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue11"));
            }
            if (BasePlayer.HasArmorSet(player, "Dryad", true) || BasePlayer.HasArmorSet(player, "Dryad", false))
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue12"));
            if (BasePlayer.HasHelmet(player, ItemID.GarlandHat, true))
            {
                string line = Personality switch
                {
                    PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue13ContCalm"),
                    PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue13ContShy"),
                    PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue13ContJolly"),
                    PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue13ContAggressive"),
                    _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue13Cont"),
                };
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue13") + line);
            }
            return chat;
        }
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);
            if ((RedeWorld.alignment < 0 && !RedeBossDowned.downedTreebark) || (RedeWorld.alignment < 2 && RedeBossDowned.downedTreebark))
                return Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.DialogueDistrust");

            if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue14"), 2);
            if (BasePlayer.HasHelmet(player, ModContent.ItemType<ThornMask>(), true))
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue15"));
            if (BasePlayer.HasArmorSet(player, "Living Wood", true) || BasePlayer.HasArmorSet(player, "Living Wood", false))
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue16"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue17"));
            }
            string bothChat = BothChat();
            if (bothChat != null)
                chat.Add(bothChat);

            if (Personality is PersonalityState.Aggressive && RedeQuest.forestNymphVar < 2)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue18"));
            if (Personality is PersonalityState.Shy)
                chat.Add("...");
            if (RedeQuest.forestNymphVar < 2)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue19"));
            else
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue20"));
            if (Personality is PersonalityState.Jolly)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue21"));
            else if (RedeQuest.forestNymphVar < 2)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue22"));
            else
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue23"));
            return chat;
        }
        public override void LoadData(TagCompound tag)
        {
            Personality = (PersonalityState)tag.GetInt("Personality");
            EyeType = tag.GetInt("EyeType");
            HairExtType = tag.GetInt("HairExtType");
            HairType = tag.GetInt("HairType");
            HasHat = tag.GetBool("HasHat");
            FlowerType = tag.GetInt("FlowerType");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Personality"] = (int)Personality;
            tag["EyeType"] = EyeType;
            tag["HairExtType"] = HairExtType;
            tag["HairType"] = HairType;
            tag["FlowerType"] = FlowerType;
            tag["HasHat"] = HasHat;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0;
    }
}
