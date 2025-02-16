using BetterDialogue.UI;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.PreHM;
using Redemption.Textures.Emotes;
using Redemption.UI;
using Redemption.UI.Dialect;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly.TownNPCs
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
            NPCID.Sets.FaceEmote[Type] = EmoteBubbleType<ForestNymphTownNPCEmote>();

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.townNPC = true;
            NPC.rarity = 0;
            NPC.friendly = true;
            if (RedeQuest.forestNymphVar < 4)
                TownNPCStayingHomeless = true;

            DialogueBoxStyle = EPIDOTRA;
        }
        public override bool HasCruxButton(Player player) => RedeQuest.forestNymphVar >= 5 && !player.HasItem(ItemType<CruxCardForestNymph>());
        public override void CruxButton(Player player)
        {
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardForestNymph>(), "ForestNymph.NoCruxDialogue", "ForestNymph.CruxDialogue");
        }
        public override bool HasLeftHangingButton(Player player) => !RedeGlobalButton.talkActive;
        public override bool HasRightHangingButton(Player player) => !RedeGlobalButton.talkActive;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax *= 2;
            NPC.defDamage *= 2;
            NPC.defDefense *= 2;
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
        public int playerFollow;
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
                    NPC.defDamage *= Main.masterMode ? 3 : 2;
                    NPC.defDefense *= Main.masterMode ? 3 : 2;
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
            if (!Main.dayTime && !NPC.homeless && NPC.homeTileX != -1 && !following)
            {
                if (!IsNpcOnscreen(NPC.Center) && Vector2.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), new Vector2(NPC.homeTileX, NPC.homeTileY) * 16) > Main.screenWidth / 2 + 100)
                {
                    NPC.Center = new Vector2(NPC.homeTileX, NPC.homeTileY - 2) * 16;
                    if (AIState <= ActionState.Wander)
                        AIState = ActionState.Sleeping;
                }
            }
            float dmgInc = 1f;
            int defenseInc;
            if (Main.masterMode)
                defenseInc = NPC.dryadWard ? (NPC.defDefense + 14) : NPC.defDefense;
            else if (Main.expertMode)
                defenseInc = NPC.dryadWard ? (NPC.defDefense + 10) : NPC.defDefense;
            else
                defenseInc = NPC.dryadWard ? (NPC.defDefense + 6) : NPC.defDefense;

            if (NPC.combatBookWasUsed)
            {
                dmgInc += 0.2f;
                defenseInc += 6;
            }
            if (NPC.combatBookVolumeTwoWasUsed)
            {
                dmgInc += 0.2f;
                defenseInc += 6;
            }
            if (NPC.downedBoss1)
            {
                dmgInc += 0.1f;
                defenseInc += 3;
            }
            if (NPC.downedBoss2)
            {
                dmgInc += 0.1f;
                defenseInc += 3;
            }
            if (NPC.downedBoss3)
            {
                dmgInc += 0.1f;
                defenseInc += 3;
            }
            if (NPC.downedQueenBee)
            {
                dmgInc += 0.1f;
                defenseInc += 3;
            }
            if (Main.hardMode)
            {
                dmgInc += 0.4f;
                defenseInc += 12;
            }
            if (NPC.downedQueenSlime)
            {
                dmgInc += 0.15f;
                defenseInc += 6;
            }
            if (NPC.downedMechBoss1)
            {
                dmgInc += 0.15f;
                defenseInc += 6;
            }
            if (NPC.downedMechBoss2)
            {
                dmgInc += 0.15f;
                defenseInc += 6;
            }
            if (NPC.downedMechBoss3)
            {
                dmgInc += 0.15f;
                defenseInc += 6;
            }
            if (NPC.downedPlantBoss)
            {
                dmgInc += 0.15f;
                defenseInc += 8;
            }
            if (NPC.downedEmpressOfLight)
            {
                dmgInc += 0.15f;
                defenseInc += 8;
            }
            if (NPC.downedGolemBoss)
            {
                dmgInc += 0.15f;
                defenseInc += 8;
            }
            if (NPC.downedAncientCultist)
            {
                dmgInc += 0.15f;
                defenseInc += 8;
            }
            NPC.damage = (int)(NPC.defDamage * dmgInc);
            NPC.defense = defenseInc;
        }
        public static bool IsNpcOnscreen(Vector2 center)
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
                    if (TileLists.LivingWoodLeaf.Contains(type))
                        score++;

                    if (TileLists.LivingWoodLeafWall.Contains(Main.tile[x, y].WallType))
                        score++;
                }
            }

            return score >= (right - left) * (bottom - top) / 2;
        }
        public bool following;
        public string ChitChat()
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
                s = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue4Cont");
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue4") + s);
            if (RedeWorld.Alignment >= 4)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue5"));
            else
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue6"));
            return chat;
        }
        private string BothChat()
        {
            WeightedRandom<string> chat = new(Main.rand);
            Player player = Main.LocalPlayer;
            if (RedeBossDowned.downedTreebark)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue7"));
            if (RedeBossDowned.downedADD)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue8"));
            if (RedeBossDowned.nukeDropped)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue9"));
            if (BasePlayer.HasArmorSet(Mod, player, "Common Guard", true) || BasePlayer.HasArmorSet(Mod, player, "Common Guard", false))
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
            Player player = Main.LocalPlayer;
            WeightedRandom<string> chat = new(Main.rand);
            if ((RedeWorld.Alignment < 0 && !RedeBossDowned.downedTreebark) || (RedeWorld.Alignment < 2 && RedeBossDowned.downedTreebark))
                return Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.DialogueDistrust");

            if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue14"), 10);
            if (BasePlayer.HasHelmet(player, ItemType<ThornMask>(), true))
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.Dialogue15"));
            if (BasePlayer.HasArmorSet(Mod, player, "Living Wood", true) || BasePlayer.HasArmorSet(Mod, player, "Living Wood", false))
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
    public class OfferButton_ForestNymph : ChatButton
    {
        public override double Priority => 1.0;
        public override string Text(NPC npc, Player player)
        {
            return RedeQuest.forestNymphVar switch
            {
                1 => Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.2"),
                2 => Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.3"),
                3 => Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.4"),
                4 => npc.homeless ? Language.GetTextValue("Mods.Redemption.DialogueBox.HomeRequirements") : Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.5"),
                5 => Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Talk"),
                _ => Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.1"),
            };
        }
        public override bool IsActive(NPC npc, Player player)
        {
            if (npc.type == NPCType<ForestNymph_Friendly>())
                return RedeWorld.Alignment >= (RedeBossDowned.downedTreebark ? 2 : 0);
            return false;
        }
        public override Color? OverrideColor(NPC npc, Player player)
        {
            if (RedeQuest.forestNymphVar == 4 && !npc.homeless)
                return RedeColor.TextPositive;
            return null;
        }
        public override void OnClick(NPC npc, Player player)
        {
            if (npc.ModNPC is ForestNymph_Friendly nymph)
            {
                if (RedeQuest.forestNymphVar == 0)
                {
                    RedeQuest.adviceSeen[(int)RedeQuest.Advice.ForestNymph] = true;
                    RedeQuest.SyncData();

                    SoundEngine.PlaySound(SoundID.MenuTick);
                    if (player.ConsumeItem(ItemID.HerbBag))
                    {
                        string line = nymph.Personality switch
                        {
                            ForestNymph.PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueCalm"),
                            ForestNymph.PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueShy"),
                            ForestNymph.PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueJolly"),
                            ForestNymph.PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogueAggressive"),
                            _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HerbBagDialogue"),
                        };
                        Main.npcChatText = line;

                        RedeQuest.forestNymphVar++;
                        RedeQuest.SyncData();

                        player.QuickSpawnItem(npc.GetSource_Loot(), ItemType<ForestCore>());
                        Main.npcChatCornerItem = ItemType<ForestCore>();
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

                    if (player.ConsumeItem(ItemType<AnglonicMysticBlossom>()))
                    {
                        string line = nymph.Personality switch
                        {
                            ForestNymph.PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueCalm"),
                            ForestNymph.PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueShy"),
                            ForestNymph.PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueJolly"),
                            ForestNymph.PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueAggressive"),
                            _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogue"),
                        };
                        Main.npcChatText = line + Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.MysticBlossomDialogueCont");
                        npc.lifeMax += 500;
                        npc.life += 500;

                        RedeQuest.forestNymphVar++;
                        RedeQuest.SyncData();

                        player.QuickSpawnItem(npc.GetSource_Loot(), ItemType<ForestNymphsSickle>());
                        Main.npcChatCornerItem = ItemType<ForestNymphsSickle>();
                        SoundEngine.PlaySound(SoundID.Chat);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ItemType<AnglonicMysticBlossom>();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (RedeQuest.forestNymphVar == 2)
                {
                    SoundEngine.PlaySound(SoundID.Chat);
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeDialogue1");

                    RedeQuest.forestNymphVar++;
                    RedeQuest.SyncData();
                }
                else if (RedeQuest.forestNymphVar >= 3)
                {
                    SoundEngine.PlaySound(SoundID.Chat);
                    if (RedeQuest.forestNymphVar >= 5)
                    {
                        Main.npcChatText = nymph.ChitChat();
                        return;
                    }
                    if (RedeQuest.forestNymphVar == 3)
                    {
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeDialogue2");

                        RedeQuest.forestNymphVar++;
                        RedeQuest.SyncData();
                    }
                    else if (RedeQuest.forestNymphVar == 4)
                    {
                        if (npc.homeless)
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeDialogue3");
                        else
                        {
                            int score = 0;
                            for (int x = -40; x <= 40; x++)
                            {
                                for (int y = -25; y <= 25; y++)
                                {
                                    Tile tile = Framing.GetTileSafely(npc.homeTileX + x, npc.homeTileY + y);
                                    if (tile.LiquidAmount >= 255 && tile.LiquidType == LiquidID.Water)
                                        score++;
                                }
                            }
                            if (score < 20)
                            {
                                string line = nymph.Personality switch
                                {
                                    ForestNymph.PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueCalm"),
                                    ForestNymph.PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueShy"),
                                    ForestNymph.PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueJolly"),
                                    ForestNymph.PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueAggressive"),
                                    _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogue"),
                                };
                                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HomeNeedWaterDialogueStart") + line + Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.NeedWaterRequirement", score.ToString());
                            }
                            else
                            {
                                string line = nymph.Personality switch
                                {
                                    ForestNymph.PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueCalm"),
                                    ForestNymph.PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueShy"),
                                    ForestNymph.PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueJolly"),
                                    ForestNymph.PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueAggressive"),
                                    _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogue"),
                                };
                                Main.npcChatText = line + Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.HasHomeDialogueCont");

                                if (RedeQuest.forestNymphVar < 5)
                                {
                                    RedeWorld.Alignment++;
                                    ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromKey("Mods.Redemption.UI.Chalice.ForestNymphHoused"), 240, 30, 0, Color.DarkGoldenrod);

                                    RedeQuest.forestNymphVar = 5;
                                    RedeQuest.SyncData();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public class TradeButton_ForestNymph : ChatButton
    {
        public override double Priority => 2.0;
        public override string Text(NPC npc, Player player) => RedeQuest.forestNymphVar < 5 ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Trade");
        public override bool IsActive(NPC npc, Player player)
        {
            if (npc.type == NPCType<ForestNymph_Friendly>())
                return RedeWorld.Alignment >= (RedeBossDowned.downedTreebark ? 2 : 0);
            return false;
        }
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.forestNymphVar < 5 ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.forestNymphVar < 5)
                return;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            TradeUI.Visible = true;
        }
    }
    public class BlessingButton_ForestNymph : ChatButton
    {
        public override double Priority => 3.0;
        public override string Text(NPC npc, Player player) => RedeWorld.Alignment <= 0 ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Blessing");
        public override bool IsActive(NPC npc, Player player)
        {
            if (npc.type == NPCType<ForestNymph_Friendly>())
                return RedeWorld.Alignment >= (RedeBossDowned.downedTreebark ? 2 : 0);
            return false;
        }
        public override Color? OverrideColor(NPC npc, Player player) => RedeWorld.Alignment <= 0 ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeWorld.Alignment <= 0)
                return;
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Main.LocalPlayer.position.X, Main.LocalPlayer.Bottom.Y - 2), Main.LocalPlayer.width, 2, DustID.DryadsWard);
                Main.dust[dustIndex].velocity.Y = -Main.rand.Next(3, 7);
                Main.dust[dustIndex].velocity.X = 0;
                Main.dust[dustIndex].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, npc.position);
            Main.LocalPlayer.AddBuff(BuffID.Lucky, 10800 * ((RedeWorld.Alignment / 2) + 1));
        }
    }
    public class FollowButton_ForestNymph : ChatButton
    {
        public override double Priority => 4.0;
        public override string Text(NPC npc, Player player)
        {
            if (RedeQuest.forestNymphVar < 2)
                return "???";
            if (npc.ModNPC is ForestNymph_Friendly nymph && !nymph.following)
                return Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.Follow");
            return Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.SFollow");
        }
        public override bool IsActive(NPC npc, Player player)
        {
            if (npc.type == NPCType<ForestNymph_Friendly>())
                return RedeWorld.Alignment >= (RedeBossDowned.downedTreebark ? 2 : 0);
            return false;
        }
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.forestNymphVar < 2 ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.forestNymphVar < 2)
                return;
            if (npc.ModNPC is ForestNymph_Friendly nymph)
            {
                nymph.playerFollow = Main.LocalPlayer.whoAmI;
                nymph.following = !nymph.following;
                if (nymph.following)
                {
                    string line = nymph.Personality switch
                    {
                        ForestNymph.PersonalityState.Calm => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueCalm"),
                        ForestNymph.PersonalityState.Shy => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueShy"),
                        ForestNymph.PersonalityState.Jolly => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueJolly"),
                        ForestNymph.PersonalityState.Aggressive => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogueAggressive"),
                        _ => Language.GetTextValue("Mods.Redemption.Dialogue.ForestNymph.FollowDialogue"),
                    };
                    Main.npcChatText = line;
                }
            }
        }
    }
    public class DHairButton_ForestNymph : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => RedeQuest.forestNymphVar < 1 ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.DHair");
        public override bool IsActive(NPC npc, Player player)
        {
            if (npc.type == NPCType<ForestNymph_Friendly>())
                return RedeWorld.Alignment >= (RedeBossDowned.downedTreebark ? 2 : 0);
            return false;
        }
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.forestNymphVar < 1 ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.forestNymphVar < 1)
                return;
            if (npc.ModNPC is ForestNymph_Friendly nymph)
            {
                nymph.FlowerType++;
                if (nymph.FlowerType > 5)
                    nymph.FlowerType = 0;
            }
        }
    }
    public class SHairButton_ForestNymph : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => RedeQuest.forestNymphVar < 1 ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.ForestNymph.SHair");
        public override bool IsActive(NPC npc, Player player)
        {
            if (npc.type == NPCType<ForestNymph_Friendly>())
                return RedeWorld.Alignment >= (RedeBossDowned.downedTreebark ? 2 : 0);
            return false;
        }
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.forestNymphVar < 1 ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.forestNymphVar < 1)
                return;
            if (npc.ModNPC is ForestNymph_Friendly nymph)
            {
                nymph.HairExtType++;
                if (nymph.HairExtType > 2)
                    nymph.HairExtType = 0;
            }
        }
    }
}