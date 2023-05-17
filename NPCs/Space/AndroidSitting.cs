using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Redemption.Globals;
using Redemption.Base;
using Redemption.BaseExtension;
using Terraria.GameContent.UI;
using Redemption.Tiles.Furniture.SlayerShip;
using Redemption.Items.Armor.Vanity;

namespace Redemption.NPCs.Space
{
    public class AndroidSitting : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Android Mk.I");
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.width = 28;
            NPC.height = 42;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;
        }

        public override bool UsesPartyHat() => false;
        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override bool CanChat() => true;
        public override bool CheckActive() => false;
        public override void AI()
        {
            NPC.direction = (int)NPC.ai[0];
            NPC.spriteDirection = NPC.direction;
            if (Main.rand.NextBool(900))
            {
                bool chessTable = false;
                for (int x = -3; x <= 3; x++)
                {
                    for (int y = -3; y <= 3; y++)
                    {
                        Point tileToNPC = NPC.Center.ToTileCoordinates();
                        int type = Main.tile[tileToNPC.X + x, tileToNPC.Y + y].TileType;
                        if (type == ModContent.TileType<ChessTable4DTile>())
                            chessTable = true;
                    }
                }
                if (!chessTable)
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                else
                {
                    switch (Main.rand.Next(8))
                    {
                        case 0:
                            EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                            break;
                        case 1:
                            EmoteBubble.NewBubble(2, new WorldUIAnchor(NPC), 120);
                            break;
                        case 2:
                            EmoteBubble.NewBubble(16, new WorldUIAnchor(NPC), 120);
                            break;
                        case 3:
                            EmoteBubble.NewBubble(135, new WorldUIAnchor(NPC), 120);
                            break;
                        case 4:
                            EmoteBubble.NewBubble(138, new WorldUIAnchor(NPC), 120);
                            break;
                        case 5:
                            EmoteBubble.NewBubble(87, new WorldUIAnchor(NPC), 120);
                            break;
                        case 6:
                            EmoteBubble.NewBubble(93, new WorldUIAnchor(NPC), 120);
                            break;
                        case 7:
                            EmoteBubble.NewBubble(10, new WorldUIAnchor(NPC), 120);
                            break;
                    }
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }
        }
        private bool chessMove;
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);

            bool chessTable = false;
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    Point tileToNPC = NPC.Center.ToTileCoordinates();
                    int type = Main.tile[tileToNPC.X + x, tileToNPC.Y + y].TileType;
                    if (type == ModContent.TileType<ChessTable4DTile>())
                        chessTable = true;
                }
            }
            if (chessTable)
            {
                int r1 = Main.rand.Next(1, 9);
                int r2 = Main.rand.Next(1, 9);
                while (r2 == r1)
                    r2 = Main.rand.Next(1, 9);

                WeightedRandom<string> letter = new(Main.rand);
                letter.Add("A");
                letter.Add("B");
                letter.Add("C");
                letter.Add("D");
                letter.Add("E");
                letter.Add("F");
                letter.Add("G");
                letter.Add("H");
                string a1 = letter;
                string a2 = letter;
                if (chessMove)
                {
                    chat.Add(a1 + r1 + " to " + a2 + r2 + "...");
                    chat.Add("Paladin to " + a1 + r1, .2f);
                    chat.Add("Soldier to " + a1 + r1, .2f);
                    chat.Add("Slayer to " + a1 + r1, .2f);
                    chat.Add("Ship to " + a1 + r1, .2f);
                }
                else
                {
                    chat.Add("(They appear very focused)");
                    chat.Add("(It appears to be stressed)");
                    chat.Add("(It appears to be confident)");
                    chat.Add("(It appears to be winning)");
                    chat.Add("(It appears to be losing)");
                    chat.Add("Checkmate. Proceeding to next round.", .4f);
                }
                chessMove = !chessMove;
            }
            else
            {
                if (BasePlayer.HasHelmet(player, ModContent.ItemType<KingSlayerMask>(), true))
                    chat.Add("King Slayer imposter detected. Alerting King Slayer... Message failed to send.");
                else if (player.IsFullTBot())
                    chat.Add("I request our 4D chess table back, robot.");
                else if (player.RedemptionPlayerBuff().ChickenForm)
                    chat.Add("I request our 4D chess table back, chicken.");
                else
                    chat.Add("I request our 4D chess table back, human.");

                chat.Add("(They appear to be annoyed)");
            }
            return chat;
        }
    }
}