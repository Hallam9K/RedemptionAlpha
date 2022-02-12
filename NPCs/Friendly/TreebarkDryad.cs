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

        private int WoodType;
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
                        int type = Framing.GetTileSafely(tileToNPC.X, tileToNPC.Y).TileType;
                        if (type == TileID.VanityTreeSakura)
                            SakuraScore++;
                        if (type == TileID.VanityTreeYellowWillow)
                            WillowScore++;
                    }
                }

                WeightedRandom<int> choice = new(Main.rand);
                choice.Add(0, 100);
                choice.Add(1, WillowScore);
                choice.Add(2, SakuraScore);

                WoodType = choice;

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
                NPC.GivenName = name + " the Treebark Dryad";
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

            if (score == 0)
                chat.Add("Where did all my tree friends go..? Perhaps they grew weary of me...", 2);

            if (score < 60)
                chat.Add("You aren't using that axe of yours on my tree friends, are you..?");

            if (RedeWorld.alignment < 0)
                chat.Add("You don't look like a very pleasant fellow. I hope you don't try to chop me down... Haha.. Ha.");

            if (RedeBossDowned.downedThorn)
                chat.Add("The forest we came from, Fairwood, has been freed of its curse. I was there to witness the forest's warden be tangled up by those cursed roots... But we toil with no humans, and our magic did nothing, so we roamed and roamed until we found this strange portal. It's what lead us here.");
            else
                chat.Add("You wouldn't happen to see a brambly old man..? Poor thing with gulped up by the cursed forest we once lived in. I toil with no humans, but I do wonder if he's alright...");

            if (BasePlayer.HasHelmet(Main.LocalPlayer, ModContent.ItemType<ThornMask>()))
                chat.Add("You remind me of that old warden, did the forest's curse get you too..?");

            chat.Add("Are you friend, or foe. As long as you don't use your axe on me, I don't care...");
            return "Hmmmm... " + chat;
        }

        public override bool CheckActive()
        {
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
                NPC.frame.X = NPC.frame.Width * WoodType;
                EyeFrameX = WoodType;

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
                        Gore.NewGore(new Vector2(NPC.Center.X + Main.rand.Next(-12, 4), NPC.Center.Y + Main.rand.Next(6)), NPC.velocity, goreType);

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
                    int type = Framing.GetTileSafely(spawnInfo.spawnTileX + x, spawnInfo.spawnTileY + y).TileType;
                    if (type == TileID.Trees || type == TileID.PalmTree || type == TileID.VanityTreeSakura || type == TileID.VanityTreeYellowWillow)
                        score++;
                }
            }

            float baseChance = SpawnCondition.OverworldDay.Chance * (!NPC.AnyNPCs(NPC.type) ? 1 : 0);
            float multiplier = Framing.GetTileSafely(spawnInfo.spawnTileX, spawnInfo.spawnTileY).TileType == TileID.Grass ? (Main.raining ? 0.01f : 0.005f) : 0f;
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