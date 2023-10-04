using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using System.Collections.Generic;
using Redemption.Walls;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Lore;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Materials.HM;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Redemption.WorldGeneration;
using Redemption.Tiles.Tiles;
using Redemption.Biomes;
using Redemption.BaseExtension;
using Redemption.Items.Donator.Lizzy;
using Terraria.GameContent.UI;
using Terraria.Localization;

namespace Redemption.NPCs.Friendly
{
    public class KS3Sitting : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.width = 56;
            NPC.height = 82;
            NPC.lifeMax = 999;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;
        }

        public override bool UsesPartyHat() => true;
        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override bool CanChat() => true;
        public override bool CheckActive()
        {
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Terraria.GameContent.Events.BirthdayParty.PartyIsUp)
            {
                Asset<Texture2D> hat = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.PartyHat);
                var offset = (NPC.frame.Y / 88) switch
                {
                    3 => 2,
                    4 => 2,
                    _ => 0,
                };
                var hatEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 origin = new(hat.Value.Width / 2f, hat.Value.Height / 2f);
                spriteBatch.Draw(hat.Value, NPC.Center - new Vector2(1 - offset * NPC.spriteDirection, 48) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, hatEffects, 0);
            }
        }
        public override void AI()
        {
            if (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus)
                NPC.active = false;

            NPC.direction = 1;
            if (NPC.AnyNPCs(ModContent.NPCType<KS3>()))
            {
                for (int i = 0; i < 15; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Frost, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                NPC.active = false;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile lizzy = Main.projectile[i];
                if (!lizzy.active || lizzy.type != ModContent.ProjectileType<LizzyPet>())
                    continue;

                if (lizzy.frame != 8)
                    continue;

                if (NPC.localAI[0]++ % 500 == 0)
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            AITimer++;
            if (AITimer >= 300 && AITimer <= 370)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 6 * frameHeight)
                        NPC.frame.Y = 0;
                }
                if (AITimer >= 370)
                {
                    NPC.frame.Y = 0;
                    AITimer = 0;
                }
                if (AITimer == 330)
                    NPC.Shoot(new Vector2(NPC.Center.X - 80, NPC.Center.Y - 50), ModContent.ProjectileType<KS3Sitting_Hologram>(), 0, Vector2.Zero, Main.rand.Next(4));
            }
        }

        private static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.CycleD");

            switch (ChatNumber)
            {
                case 0:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.0");
                    break;
                case 1:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.1");
                    break;
                case 2:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.2");
                    break;
                case 3:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.3");
                    break;
                case 4:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.4");
                    break;
                case 5:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.5");
                    break;
                case 6:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.6");
                    break;
                case 7:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.7");
                    break;
                case 8:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.8");
                    break;
                case 9:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.9");
                    break;
                case 10:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.Quest");
                    break;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.LocalPlayer;
            Main.npcChatCornerItem = 0;
            if (firstButton)
            {
                if (ChatNumber == 2 && RedeWorld.slayerRep == 0)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int Urani = player.FindItem(ModContent.ItemType<Uranium>());
                    if (Urani >= 0)
                    {
                        player.inventory[Urani].stack--;
                        if (player.inventory[Urani].stack <= 0)
                            player.inventory[Urani] = new Item();

                        WeightedRandom<string> chat = new(Main.rand);
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.UraniumDialogue1"));
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.UraniumDialogue2"));
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.UraniumDialogue3"));
                        Main.npcChatText = chat;

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.SilverCoin, 20);
                        ChatNumber++;
                        RedeWorld.slayerRep++;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);

                        CombatText.NewText(NPC.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);

                        SoundEngine.PlaySound(SoundID.Chat);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<Uranium>();
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.NoUraniumDialogue3");
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep == 1)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int WiringKit = player.FindItem(ModContent.ItemType<SlayerWiringKit>());
                    if (WiringKit >= 0)
                    {
                        player.inventory[WiringKit].stack--;
                        if (player.inventory[WiringKit].stack <= 0)
                            player.inventory[WiringKit] = new Item();

                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest1CompleteDialogue");

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.GoldCoin, 4);
                        RedeWorld.slayerRep++;
                        CombatText.NewText(NPC.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);

                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipFix1", AssetRequestMode.ImmediateLoad).Value;

                        Point origin = RedeGen.slayerShipVector.ToPoint();

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<SlayerWiringKit>();
                        Main.npcChatText = QuestChat();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep == 2)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int HullPlating = player.FindItem(ModContent.ItemType<SlayerHullPlating>());
                    if (HullPlating >= 0)
                    {
                        player.inventory[HullPlating].stack--;
                        if (player.inventory[HullPlating].stack <= 0)
                            player.inventory[HullPlating] = new Item();

                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest2CompleteDialogue");

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.GoldCoin, 8);
                        RedeWorld.slayerRep++;
                        CombatText.NewText(NPC.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);

                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(0, 255, 255)] = ModContent.TileType<SlayerShipPanelTile>(),
                            [new Color(255, 0, 255)] = ModContent.TileType<ShipGlassTile>(),
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipFix2", AssetRequestMode.ImmediateLoad).Value;

                        Point origin = RedeGen.slayerShipVector.ToPoint();

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<SlayerHullPlating>();
                        Main.npcChatText = QuestChat();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep == 3)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    int ShipEngine = player.FindItem(ModContent.ItemType<SlayerShipEngine>());
                    if (ShipEngine >= 0)
                    {
                        player.inventory[ShipEngine].stack--;
                        if (player.inventory[ShipEngine].stack <= 0)
                            player.inventory[ShipEngine] = new Item();

                        Main.npcChatCornerItem = ModContent.ItemType<MemoryChip>();
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest3CompleteDialogue");

                        player.QuickSpawnItem(NPC.GetSource_Loot(), ItemID.GoldCoin, 12);
                        player.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<MemoryChip>());
                        RedeWorld.slayerRep++;
                        RedeWorld.alignment += 2;

                        CombatText.NewText(NPC.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);
                        CombatText.NewText(player.getRect(), Color.Gold, "+2", true, false);

                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(0, 255, 255)] = ModContent.TileType<SlayerShipPanelTile>(),
                            [new Color(255, 0, 255)] = ModContent.TileType<ShipGlassTile>(),
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };

                        Dictionary<Color, int> colorToWall = new()
                        {
                            [new Color(0, 255, 0)] = ModContent.WallType<SlayerShipPanelWallTile>(),
                            [Color.Black] = -1
                        };

                        Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipFix2", AssetRequestMode.ImmediateLoad).Value;
                        Texture2D texWalls = ModContent.Request<Texture2D>("Redemption/WorldGeneration/SlayerShipWallsFix", AssetRequestMode.ImmediateLoad).Value;

                        Point origin = RedeGen.slayerShipVector.ToPoint();

                        GenUtils.InvokeOnMainThread(() =>
                        {
                            TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile, texWalls, colorToWall);
                            gen.Generate(origin.X, origin.Y, true, true);
                        });
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ModContent.ItemType<SlayerShipEngine>();
                        Main.npcChatText = QuestChat();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
                else if (ChatNumber == 10 && RedeWorld.slayerRep >= 4)
                {
                    Main.npcChatText = QuestChat();
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
                else if (ChatNumber == 9 && RedeWorld.slayerRep >= 4)
                    shopName = "Shop";
                else
                    Main.npcChatText = ChitChat();
            }
            else
            {
                ChatNumber++;
                if (ChatNumber > 10)
                    ChatNumber = 0;
                if (RedeWorld.slayerRep == 0 && ChatNumber > 2)
                    ChatNumber = 0;
                if (RedeWorld.slayerRep >= 1 && ChatNumber == 2)
                    ChatNumber++;
                if (!Main.LocalPlayer.Redemption().foundHall && ChatNumber == 5)
                    ChatNumber++;
            }
        }
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add<Datalog>()
                .Add<Datalog2>()
                .Add<Datalog3>()
                .Add<Datalog4>()
                .Add<Datalog5>()
                .Add<Datalog6>()
                .Add<Datalog7>()
                .Add<Datalog8>()
                .Add<Datalog9>()
                .Add<Datalog10>()
                .Add<Datalog11>()
                .Add<Datalog12>()
                .Add<Datalog13>()
                .Add<Datalog14>()
                .Add<Datalog15>()
                .Add<Datalog16>()
                .Add<Datalog17>()
                .Add<Datalog18>()
                .Add<Datalog19>()
                .Add<Datalog20>()
                .Add<Datalog21>()
                .Add<Datalog22>()
                .Add<Datalog24>()
                .Add<Datalog25>()
                .Add<Datalog26>()
                .Add<Datalog27>()
                .Add<Datalog28>();

            npcShop.Register();
        }

        public static string QuestChat()
        {
            return RedeWorld.slayerRep switch
            {
                2 => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest2Dialogue"),
                3 => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest3Dialogue"),
                4 => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest4Dialogue"),
                _ => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest1Dialogue"),
            };
        }
        public static string ChitChat()
        {
            switch (ChatNumber)
            {
                case 0:
                    if (RedeWorld.slayerRep >= 1 && RedeWorld.slayerRep < 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat1B");
                    else if (RedeWorld.slayerRep >= 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat1C");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat1");
                case 1:
                    if (RedeWorld.slayerRep == 1)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2B");
                    else if (RedeWorld.slayerRep >= 2 && RedeWorld.slayerRep < 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2C");
                    else if (RedeWorld.slayerRep >= 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2D");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2");
                case 3:
                    if (RedeWorld.slayerRep == 3)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3C");
                    else if (RedeWorld.slayerRep >= 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3D");
                    else
                        return Main.rand.NextBool(2) ? Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3") : Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3B");
                case 4:
                    if (RedeWorld.slayerRep == 2)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat4B");
                    else if (RedeWorld.slayerRep >= 3)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat4C");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat4");
                case 5:
                    if (RedeWorld.slayerRep == 2)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5B");
                    else if (RedeWorld.slayerRep == 3)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5C");
                    else if (RedeWorld.slayerRep >= 4)
                        return Main.rand.NextBool(3) ? Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5E") : Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5D");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5");
                case 6:
                    if (RedeWorld.slayerRep >= 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat6B");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat6");
                case 7:
                    if (RedeWorld.slayerRep >= 2 && RedeWorld.slayerRep < 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat7B");
                    else if (RedeWorld.slayerRep >= 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat7C");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat7");
                case 8:
                    if (RedeWorld.slayerRep >= 2 && RedeWorld.slayerRep < 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat8B");
                    else if (RedeWorld.slayerRep >= 4)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat8C");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat8");
                case 9:
                    if (RedeWorld.slayerRep == 2)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat9B");
                    else if (RedeWorld.slayerRep == 3)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat9C");
                    else
                        return Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat9");
            }
            return "...";
        }
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);

            if (RedeBossDowned.downedSlayer && !Main.LocalPlayer.InModBiome<SlayerShipBiome>())
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.NotInShipDialogue"));
            else
            {
                if (RedeWorld.slayerRep >= 2)
                {
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue1"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue2"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue3"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue4"));
                }
                else if (RedeWorld.slayerRep >= 4)
                {
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue5"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue6"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue7"));
                }
                else
                {
                    if (player.IsFullTBot())
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue8TBot"));
                    else
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue8"));

                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue9"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue10"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue11"));
                }
                if (NPC.downedMoonlord)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue12"), 2);

                if (BasePlayer.HasHelmet(player, ModContent.ItemType<KingSlayerMask>(), true))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue13"));

                if (player.wellFed && RedeWorld.slayerRep < 2)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue14"));
                if (player.RedemptionPlayerBuff().ChickenForm)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue15"));
            }
            return chat;
        }
    }
}
