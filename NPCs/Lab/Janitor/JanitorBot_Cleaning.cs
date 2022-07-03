using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.UI;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBot_Cleaning : ModNPC
    {
        public ref float State => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.width = 34;
            NPC.height = 44;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.npcSlots = 0;
            NPC.netAlways = true;
        }

        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();
            SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.2f };
            switch (State)
            {
                case 0:
                    if (player.active && !player.dead && NPC.DistanceSQ(player.Center) <= 200 * 200 && player.Center.X < NPC.Center.X)
                    {
                        if (player.IsFullTBot())
                            State = 1;
                        else
                            State = 2;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer == 30)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, "...Why did you have to barge in through the ventilation shaft?", Colors.RarityYellow, new Color(100, 86, 0), voice, 2, 100, 0, false)) // 224
                             .Add(new(NPC, "Lost your access card huh?[30] Have mine and get out of my sight.", Colors.RarityYellow, new Color(100, 86, 0), voice, 2, 100, 0, false)) // 252
                             .Add(new(NPC, "*Grumbles* Those darn careless bots losing their cards...", Colors.RarityYellow, new Color(100, 86, 0), voice, 2, 100, 30, true)); // 244
                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(chain);
                    }
                    if (AITimer >= 506)
                    {
                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);

                        if (!LabArea.labAccess[0])
                            Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel1>());

                        NPC.SetEventFlagCleared(ref RedeBossDowned.downedJanitor, -1);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);

                        NPC.SetDefaults(ModContent.NPCType<JanitorBot_Defeated>());
                        NPC.ai[0] = 1;
                        NPC.netUpdate = true;
                    }
                    break;
                case 2:
                    AITimer++;
                    if (AITimer == 30)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, "Oi![10] Don't go there,[10] the floor's wet.", Colors.RarityYellow, new Color(100, 86, 0), voice, 2, 100, 0, false)) // 172
                             .Add(new(NPC, ".[10].[10].[10]", Colors.RarityYellow, new Color(100, 86, 0), voice, 2, 100, 0, false)) // 136
                             .Add(new(NPC, "Wait...[30] You're a trespasser!", Colors.RarityYellow, new Color(100, 86, 0), voice, 2, 100, 30, true)); // 216
                        TextBubbleUI.Visible = true;
                        TextBubbleUI.Add(chain);
                    }
                    if (AITimer == 338)
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 216);
                    if (AITimer >= 544)
                    {
                        NPC.SetDefaults(ModContent.NPCType<JanitorBot>());
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (State == 1 && AITimer >= 30)
            {
                NPC.frame.Y = 4 * frameHeight;
                return;
            }
            if (State == 2)
            {
                if ((AITimer >= 30 && AITimer < 202) || AITimer >= 338)
                {
                    NPC.frame.Y = 4 * frameHeight;
                    return;
                }
            }
            if (NPC.frameCounter++ >= 20)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
    }
}