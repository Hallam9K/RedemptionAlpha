using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Redemption.Globals;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using Terraria.Utilities;
using Redemption.Items.Usable.Summons;
using Redemption.Buffs.Debuffs;
using Redemption.Base;
using Terraria.Localization;

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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treebark");
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                    BuffID.Poisoned,
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
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
                int SakuraScore = 0;
                int WillowScore = 0;
                for (int x = -40; x <= 40; x++)
                {
                    for (int y = -40; y <= 40; y++)
                    {
                        Point tileToNPC = NPC.Center.ToTileCoordinates();
                        int type = Main.tile[tileToNPC.X + x, tileToNPC.Y + y].type;
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
                if (WoodType == 1)
                    name.Add("Willowbark", 2);
                if (WoodType == 2)
                {
                    name.Add("Cherrysplinter", 2);
                    name.Add("Blossomwood", 2);
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
        public override string GetChat()
        {
            WeightedRandom<string> chat = new(Main.rand);

            int score = 0;
            for (int x = -40; x <= 40; x++)
            {
                for (int y = -40; y <= 40; y++)
                {
                    Point tileToNPC = NPC.Center.ToTileCoordinates();
                    int type = Main.tile[tileToNPC.X + x, tileToNPC.Y + y].type;
                    if (type == TileID.Trees || type == TileID.PalmTree)
                        score++;
                }
            }

            if (score == 0)
                chat.Add("Hmmmm... Where did all my tree friends go..? Perhaps they grew weary of me...", 2);

            if (score < 60)
                chat.Add("Hmmmm... You aren't using that axe of yours on my tree friends, are you..?");

            if (RedeWorld.alignment < 0)
                chat.Add("Hmmmm... You don't look like a very pleasant fellow. I hope you don't try to chop me down... Haha.. Ha.");

            chat.Add("Hmmmm... Are you friend, or foe. As long as you don't use your axe on me, I don't care...");
            return chat;
        }

        public override bool CheckActive()
        {
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 3;
            NPC.frame.X = NPC.frame.Width * WoodType;
            EyeFrameX = WoodType;

            if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].type == NPC.type)
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
            int score = 0;
            for (int x = -40; x <= 40; x++)
            {
                for (int y = -40; y <= 40; y++)
                {
                    int type = Main.tile[spawnInfo.spawnTileX + x, spawnInfo.spawnTileY + y].type;
                    if (type == TileID.Trees || type == TileID.PalmTree || type == TileID.VanityTreeSakura || type == TileID.VanityTreeYellowWillow)
                        score++;
                }
            }

            float baseChance = SpawnCondition.OverworldDay.Chance * (!NPC.AnyNPCs(NPC.type) ? 1 : 0);
            float multiplier = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass ? (Main.raining ? 0.01f : 0.005f) : 0f;
            float trees = score >= 60 ? 1 : 0;

            return baseChance * multiplier * trees;
        }
    }
}