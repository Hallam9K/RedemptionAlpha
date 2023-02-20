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
using Redemption.Buffs.Debuffs;
using Redemption.Base;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Buffs.NPCBuffs;
using Redemption.Items.Accessories.PreHM;

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

        public static List<Item> shopItems = new();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                    BuffID.Poisoned,
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<InfestedDebuff>()
                }
            });

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
            NPC.friendly = true;
            NPC.lifeMax = 500;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;
            NPC.rarity = 1;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;

        private string setName;
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
                setName = name + " the Treebark Dryad";
            }
            else
                typeName = setName;
        }
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (TimerRand == 0)
            {
                shopItems = CreateNewShop();

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
        }
        public override bool CanChat() => true;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
        }

        public static List<Item> CreateNewShop()
        {
            var itemIds = new List<int>
            {
                ModContent.ItemType<LivingTwig>(),
                ItemID.GrassSeeds,
                ItemID.Acorn,
                ItemID.Mushroom
            };
            if (Main.rand.NextBool(2))
            {
                switch (Main.rand.Next(5))
                {
                    case 0:
                        itemIds.Add(ItemID.BlueMoss);
                        break;
                    case 1:
                        itemIds.Add(ItemID.BrownMoss);
                        break;
                    case 2:
                        itemIds.Add(ItemID.GreenMoss);
                        break;
                    case 3:
                        itemIds.Add(ItemID.PurpleMoss);
                        break;
                    case 4:
                        itemIds.Add(ItemID.RedMoss);
                        break;
                }
            }
            switch (Main.rand.Next(5))
            {
                case 0:
                    itemIds.Add(ItemID.Apple);
                    break;
                case 1:
                    itemIds.Add(ItemID.Apricot);
                    break;
                case 2:
                    itemIds.Add(ItemID.Grapefruit);
                    break;
                case 3:
                    itemIds.Add(ItemID.Lemon);
                    break;
                case 4:
                    itemIds.Add(ItemID.Peach);
                    break;
            }
            itemIds.Add(ItemID.WandofSparking);
            itemIds.Add(ItemID.BabyBirdStaff);
            if (Main.rand.NextBool(2))
                itemIds.Add(ItemID.Aglet);
            if (Main.rand.NextBool(4))
                itemIds.Add(ItemID.AnkletoftheWind);
            if (Main.rand.NextBool(8))
                itemIds.Add(ItemID.FlowerBoots);
            if (Main.rand.NextBool(8))
                itemIds.Add(ItemID.NaturesGift);
            else if (Main.rand.NextBool(4))
                itemIds.Add(ItemID.JungleRose);
            else if (Main.rand.NextBool(10))
                itemIds.Add(ModContent.ItemType<ForestCore>());

            var items = new List<Item>();
            foreach (int itemId in itemIds)
            {
                Item item = new();
                item.SetDefaults(itemId);
                items.Add(item);
            }
            return items;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            foreach (Item item in shopItems)
            {
                if (item == null || item.type == ItemID.None)
                    continue;

                shop.item[nextSlot].SetDefaults(item.type);
                nextSlot++;
            }
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new(Main.rand);

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
                    chat.Add("I think I have lingered here for long enough. I was told by the humans to guard this place, but it has been far too long an age... Dangers have come and gone, but none have ever sought to demolish this shrine. To Fairwood I return - I have missed its lush forests and bark'en friends. Mayhaps you be this shrine's new protector..? If it needs one, anyway.", 4);
                chat.Add("What's that shrine behind me..? It was built by a group of humans, long dead by now, as a place of worship to some sort of figure... I don't recall their name, but I know they were a good friend of nature. Some may consider her the Mother of Nature..? Hmmm... My memory grows distant in my old age.", 2);
                chat.Add("I have been here a long time, even for us. I was a bold sapling, far more than the others, and so I had come to this land far far earlier than the rest. Maybe I was unwise, but I have seen more of us coming from our lands in recent times, so perhaps the others grew adventurous too.");
            }

            if (score == 0)
                chat.Add("Where did all my tree friends go..? Perhaps they grew weary of me...", 2);

            if (score < 60)
                chat.Add("You aren't using that axe of yours on my tree friends, are you..?");

            if (RedeWorld.alignment < 0)
                chat.Add("You don't look like a very pleasant fellow. I hope you don't try to chop me down... Haha.. Ha.");

            if (AITimer != 1)
            {
                if (RedeBossDowned.downedThorn)
                    chat.Add("The forest we came from, Fairwood, has been freed of its curse. I was there to witness the forest's warden be tangled up by those cursed roots... But we toil with no humans, and our magic did nothing, so we roamed and roamed until we found this strange portal. It's what lead us here.");
                else
                    chat.Add("You wouldn't happen to see a brambly young man..? Poor thing was gulped up by the cursed forest we once lived in. I toil with no humans, but I do wonder if he's alright...");
                if (BasePlayer.HasHelmet(Main.LocalPlayer, ModContent.ItemType<ThornMask>()))
                    chat.Add("You remind me of that young warden, did the forest's curse get you too..?");
            }
            chat.Add("Are you friend, or foe? As long as you don't use your axe on me, I don't care...");
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
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
                NPC.frame.X = NPC.frame.Width * (int)WoodType;
                EyeFrameX = (int)WoodType;

                if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].whoAmI == NPC.whoAmI)
                {
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
                    if (Main.rand.NextBool(60))
                        Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X + Main.rand.Next(-12, 4), NPC.Center.Y + Main.rand.Next(6)), NPC.velocity, goreType);

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

            float baseChance = SpawnCondition.OverworldDay.Chance * (!NPC.AnyNPCs(NPC.type) ? 1 : 0);
            float multiplier = Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY).TileType == TileID.Grass ? (Main.raining ? 0.02f : 0.005f) : 0f;
            float trees = score >= 60 ? 1 : 0;

            return baseChance * multiplier * trees;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new TownNPCUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPC.type]);
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("These slow-thinking ents only appear in heavily forested areas. They have only recently arrived on this island, coming from the portal on the surface. Once every century, they find a shallow pond and hibernate in the centre. The water from the pond feeds the ent while hibernating.")
            });
        }
    }
}