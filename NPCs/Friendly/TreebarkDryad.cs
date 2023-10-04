using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Utilities;
using Redemption.Base;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Accessories.PreHM;
using Redemption.BaseExtension;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Redemption.UI.ChatUI;
using Terraria.ModLoader.IO;
using System.Linq;

namespace Redemption.NPCs.Friendly
{
    public class TreebarkDryad : ModNPC
    {
        public enum ActionState
        {
            Idle
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public ref float WoodType => ref NPC.ai[3];

        private int EyeFrameY;
        private int EyeFrameX;

        public static TreebarkShop Shop;
        public List<Item> shopItems = new();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 20),
                PortraitPositionYOverride = 0
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 88;
            NPC.height = 92;
            NPC.lifeMax = Main.hardMode ? 2000 : 500;
            NPC.defense = 6;
            NPC.lifeRegen = 10;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.rarity = 1;
            NPC.lifeRegen = 1;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.NPCDeath27;
            NPC.chaseable = false;
        }
        public override void OnKill()
        {
            if (!RedeBossDowned.downedTreebark)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "-1", true, false);

                    if (!RedeWorld.alignmentGiven)
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.TreebarkFelled"), 300, 30, 0, Color.DarkGoldenrod);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    RedeWorld.alignment--;
                    RedeBossDowned.downedTreebark = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => item.axe > 0 ? null : false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => projectile.Redemption().IsAxe ? null : false;
        public override bool CanBeHitByNPC(NPC attacker) => false;
        public string setName;
        public override void ModifyTypeName(ref string typeName)
        {
            if (setName == null)
            {
                WeightedRandom<string> name = new(Main.rand);
                name.Add("Gentlewood");
                name.Add("Blandwood");
                name.Add("Elmshade");
                name.Add("Vinewood");
                name.Add("Bitterthorn");
                name.Add("Irontwig");
                name.Add("Tapio");
                if (WoodType == 1)
                    name.Add("Willowbark", 3);
                if (WoodType == 2)
                {
                    name.Add("Cherrysplinter", 3);
                    name.Add("Blossomwood", 3);
                }
                setName = name;
            }
            else
                typeName = setName + " the Treebark Dryad";
        }
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (TimerRand == 0)
            {
                int SakuraScore = 0;
                int WillowScore = 0;
                for (int x = -40; x <= 40; x++)
                {
                    for (int y = -40; y <= 40; y++)
                    {
                        Point tileToNPC = NPC.Center.ToTileCoordinates();
                        int type = Framing.GetTileSafely(tileToNPC.X + x, tileToNPC.Y + y).TileType;
                        if (type == TileID.VanityTreeSakura)
                            SakuraScore++;
                        if (type == TileID.VanityTreeYellowWillow)
                            WillowScore++;
                    }
                }

                if (WoodType == 0)
                {
                    WeightedRandom<int> choice = new(Main.rand);
                    choice.Add(0, 20);
                    choice.Add(1, WillowScore);
                    choice.Add(2, SakuraScore);

                    WoodType = choice;
                }
                TimerRand = 1;
            }
            else
            {
                if (TimerRand == 1)
                {
                    if (NPC.life < NPC.lifeMax - 10 && !Main.dedServ)
                    {
                        Texture2D bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra").Value;
                        SoundStyle voice = CustomSounds.Voice2 with { Pitch = -1f };

                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.1"), Color.LightGreen, Color.ForestGreen, voice, .06f, 2f, .5f, true, bubble: bubble, endID: 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                        TimerRand = 2;
                    }
                }
                else if (TimerRand == 3)
                {
                    if (NPC.life < NPC.lifeMax / 2 && !Main.dedServ)
                    {
                        Texture2D bubble = ModContent.Request<Texture2D>("Redemption/UI/TextBubble_Epidotra").Value;
                        SoundStyle voice = CustomSounds.Voice2 with { Pitch = -1f };

                        string gender = player.Male ? Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.3") : Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.4");
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.2") + gender + Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.5"), Color.LightGreen, Color.ForestGreen, voice, .06f, 2, .5f, true, bubble: bubble));
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                        TimerRand = 4;
                    }
                }
            }
            if (Main.LocalPlayer.talkNPC <= -1 || Main.npc[Main.LocalPlayer.talkNPC].whoAmI != NPC.whoAmI)
                return;
            int goreType = GoreID.TreeLeaf_Normal;
            switch (WoodType)
            {
                case 1:
                    goreType = GoreID.TreeLeaf_VanityTreeYellowWillow;
                    break;
                case 2:
                    goreType = GoreID.TreeLeaf_VanityTreeSakura;
                    break;
            }
            if (Main.rand.NextBool(60) && Main.netMode != NetmodeID.Server)
                Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X + Main.rand.Next(-12, 4), NPC.Center.Y + Main.rand.Next(6)), NPC.velocity, goreType);
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            TimerRand = 3;
        }
        public override bool CanChat() => true;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            if (!RedeBossDowned.downedTreebark)
                button = Language.GetTextValue("LegacyInterface.28");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
                shopName = Shop.Name;
        }
        public override void OnSpawn(IEntitySource source)
        {
            shopItems = Shop.GenerateNewInventoryList();
        }
        public override void SaveData(TagCompound tag)
        {
            tag["itemIds"] = shopItems;
        }
        public override void LoadData(TagCompound tag)
        {
            shopItems = tag.Get<List<Item>>("shopItems");
        }
        public override void AddShops()
        {
            Shop = new TreebarkShop(NPC.type);

            Shop.Add<LivingTwig>();
            Shop.Add(ItemID.GrassSeeds);
            Shop.Add(ItemID.Acorn);
            Shop.Add(ItemID.Mushroom);

            Shop.AddPool("Moss", 1)
                .Add(new Item(ItemID.BlueMoss) { shopCustomPrice = 20 })
                .Add(new Item(ItemID.BrownMoss) { shopCustomPrice = 20 })
                .Add(new Item(ItemID.GreenMoss) { shopCustomPrice = 20 })
                .Add(new Item(ItemID.PurpleMoss) { shopCustomPrice = 20 })
                .Add(new Item(ItemID.RedMoss) { shopCustomPrice = 20 });

            Shop.AddPool("Fruit", 1)
                .Add(ItemID.Apple)
                .Add(ItemID.Apricot)
                .Add(ItemID.Grapefruit)
                .Add(ItemID.Lemon)
                .Add(ItemID.Peach);

            Shop.Add(ItemID.WandofSparking);
            Shop.Add(ItemID.BabyBirdStaff);

            Shop.AddPool("Special", 1)
                .Add(ItemID.Aglet)
                .Add(ItemID.AnkletoftheWind)
                .Add(ItemID.FlowerBoots)
                .Add(ItemID.NaturesGift)
                .Add(ItemID.JungleRose)
                .Add<ForestCore>();

            Shop.Register();
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new(Main.rand);

            if (NPC.life < (int)(NPC.lifeMax * .8f) || RedeBossDowned.downedTreebark)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.FelledDialogue1"), 10);
            if (NPC.life < (int)(NPC.lifeMax * .5f) || RedeBossDowned.downedTreebark)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.FelledDialogue2"), 10);
            if (RedeBossDowned.downedTreebark)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.FelledDialogue3"), 10);
                return Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Felled") + chat;
            }

            int score = 0;
            for (int x = -40; x <= 40; x++)
            {
                for (int y = -40; y <= 40; y++)
                {
                    Point tileToNPC = NPC.Center.ToTileCoordinates();
                    int type = Main.tile[tileToNPC.X + x, tileToNPC.Y + y].TileType;
                    if (type == TileID.Trees || type == TileID.PalmTree)
                        score++;
                }
            }

            if (AITimer == 1)
            {
                if (RedeWorld.DayNightCount >= 3)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.ShrineDialogue1"), 4);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.ShrineDialogue2"), 2);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.ShrineDialogue3"));
            }

            if (score == 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.NoTreesDialogue1"), 2);

            if (score < 60)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.NoTreesDialogue2"));

            if (RedeWorld.alignment < 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Dialogue1"));

            if (AITimer != 1)
            {
                if (RedeBossDowned.downedThorn)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Dialogue2"));
                else
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Dialogue3"));
                if (BasePlayer.HasHelmet(Main.LocalPlayer, ModContent.ItemType<ThornMask>()))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Dialogue4"));
            }
            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Dialogue5"));
            return "Hmmmm... " + chat;
        }

        public override bool CheckActive()
        {
            if (AITimer == 1 && RedeWorld.DayNightCount < 4)
                return false;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            NPC.frame.X = NPC.frame.Width * (int)WoodType;
            EyeFrameX = (int)WoodType;

            if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].whoAmI == NPC.whoAmI)
            {
                if (NPC.frame.Y < 4 * frameHeight)
                    NPC.frame.Y = 4 * frameHeight;

                if (++NPC.frameCounter >= 15)
                {
                    EyeFrameY = 1;

                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 8 * frameHeight)
                        NPC.frame.Y = 4 * frameHeight;
                }
            }
            else
            {
                if (++NPC.frameCounter >= 15)
                {
                    EyeFrameY = 0;
                    if (Main.rand.NextBool(8))
                        EyeFrameY = 1;

                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D EyesTex = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/TreebarkDryad_Eyes").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            int Height = EyesTex.Height / 2;
            int Width = EyesTex.Width / 3;
            int y = Height * EyeFrameY;
            int x = Width * EyeFrameX;
            Rectangle rect = new(x, y, Width, Height);
            Vector2 origin = new(Width / 2f, Height / 2f);

            if (NPC.frame.Y < 400)
            {
                spriteBatch.Draw(EyesTex, NPC.Center - screenPos - new Vector2(6 * -NPC.spriteDirection, NPC.frame.Y >= 100 && NPC.frame.Y < 300 ? 12 : 14), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (NPC.AnyNPCs(Type))
                return 0;

            int score = 0;
            for (int x = -40; x <= 40; x++)
            {
                for (int y = -40; y <= 40; y++)
                {
                    int type = Framing.GetTileSafely(spawnInfo.SpawnTileX + x, spawnInfo.SpawnTileY + y).TileType;
                    if (type == TileID.Trees || type == TileID.PalmTree || type == TileID.VanityTreeSakura || type == TileID.VanityTreeYellowWillow)
                        score++;
                }
            }

            float baseChance = SpawnCondition.OverworldDay.Chance;
            float multiplier = Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY).TileType == TileID.Grass ? (Main.raining ? 0.025f : 0.008f) : 0f;
            float trees = score >= 60 ? 1 : 0;

            return baseChance * multiplier * trees;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 35; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture, Scale: 2f);

                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(0, 60), NPC.velocity, ModContent.Find<ModGore>("Redemption/TreebarkDryadGoreLeg" + (WoodType + 1)).Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/TreebarkDryadGoreAntler" + (WoodType + 1)).Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(40, 0), NPC.velocity, ModContent.Find<ModGore>("Redemption/TreebarkDryadGoreAntlerB" + (WoodType + 1)).Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(20, 10), NPC.velocity, ModContent.Find<ModGore>("Redemption/TreebarkDryadGoreHead" + (WoodType + 1)).Type, 1);
            }
            for (int i = 0; i < 3; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 40, 60));
            npcLoot.Add(ItemDropRule.Common(ItemID.Acorn, 1, 10, 20));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new TownNPCUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPC.type]);
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.TreebarkDryad"))
            });
        }
    }
    public class TreebarkShop : AbstractNPCShop
    {
        public new record Entry(Item Item, List<Condition> Conditions) : AbstractNPCShop.Entry
        {
            IEnumerable<Condition> AbstractNPCShop.Entry.Conditions => Conditions;

            public bool Disabled { get; private set; }

            public Entry Disable()
            {
                Disabled = true;
                return this;
            }

            public bool ConditionsMet() => Conditions.All(c => c.IsMet());
        }

        public record Pool(string Name, int Slots, List<Entry> Entries)
        {
            public Pool Add(Item item, params Condition[] conditions)
            {
                Entries.Add(new Entry(item, conditions.ToList()));
                return this;
            }

            public Pool Add<T>(params Condition[] conditions) where T : ModItem => Add(ModContent.ItemType<T>(), conditions);
            public Pool Add(int item, params Condition[] conditions) => Add(ContentSamples.ItemsByType[item], conditions);

            // Picks a number of items (up to Slots) from the entries list, provided conditions are met.
            public IEnumerable<Item> PickItems()
            {
                // This is not a fast way to pick items without replacement, but it's certainly easy. Be careful not to do this many many times per frame, or on huge lists of items.
                var list = Entries.Where(e => !e.Disabled && e.ConditionsMet()).ToList();
                for (int i = 0; i < Slots; i++)
                {
                    if (list.Count == 0)
                        break;

                    int k = Main.rand.Next(list.Count);
                    yield return list[k].Item;

                    // remove the entry from the list so it can't be selected again this pick
                    list.RemoveAt(k);
                }
            }
        }

        public List<Pool> Pools { get; } = new();

        public TreebarkShop(int npcType) : base(npcType) { }

        public override IEnumerable<Entry> ActiveEntries => Pools.SelectMany(p => p.Entries).Where(e => !e.Disabled);

        public Pool AddPool(string name, int slots)
        {
            var pool = new Pool(name, slots, new List<Entry>());
            Pools.Add(pool);
            return pool;
        }

        // Some methods to add a pool with a single item
        public void Add(Item item, params Condition[] conditions) => AddPool(item.ModItem?.FullName ?? $"Terraria/{item.type}", slots: 1).Add(item, conditions);
        public void Add<T>(params Condition[] conditions) where T : ModItem => Add(ModContent.ItemType<T>(), conditions);
        public void Add(int item, params Condition[] conditions) => Add(ContentSamples.ItemsByType[item], conditions);

        // Here is where we actually 'roll' the contents of the shop
        public List<Item> GenerateNewInventoryList()
        {
            var items = new List<Item>();
            foreach (var pool in Pools)
            {
                items.AddRange(pool.PickItems());
            }
            return items;
        }

        public override void FillShop(ICollection<Item> items, NPC npc)
        {
            // use the items which were selected when the NPC spawned.
            foreach (var item in ((TreebarkDryad)npc.ModNPC).shopItems)
            {
                // make sure to add a clone of the item, in case any ModifyActiveShop hooks adjust the item when the shop is opened
                items.Add(item.Clone());
            }
        }

        public override void FillShop(Item[] items, NPC npc, out bool overflow)
        {
            overflow = false;
            int i = 0;
            // use the items which were selected when the NPC spawned.
            foreach (var item in ((TreebarkDryad)npc.ModNPC).shopItems)
            {

                if (i == items.Length - 1)
                {
                    // leave the last slot empty for selling
                    overflow = true;
                    return;
                }

                // make sure to add a clone of the item, in case any ModifyActiveShop hooks adjust the item when the shop is opened
                items[i++] = item.Clone();
            }
        }
    }
}
