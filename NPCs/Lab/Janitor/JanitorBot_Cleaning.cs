using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.Textures;
using Redemption.UI.ChatUI;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBot_Cleaning : ModNPC
    {
        public ref float State => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Janitor");
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
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.2f };
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();
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
                    if (AITimer == 30 && !Main.dedServ)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Janitor.Start.R1"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false)) // 224
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Janitor.Start.R2"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false)) // 252
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Janitor.Start.R3"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, .5f, true)); // 244
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 2000)
                    {
                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);

                        if (!LabArea.labAccess[0])
                            Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel1>());

                        NPC nPC = new();
                        nPC.SetDefaults(ModContent.NPCType<JanitorBot>());
                        Main.BestiaryTracker.Kills.RegisterKill(nPC);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeBossDowned.downedJanitor = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        NPC.SetDefaults(ModContent.NPCType<JanitorBot_Defeated>());
                        NPC.ai[0] = 1;
                        NPC.netUpdate = true;
                    }
                    break;
                case 2:
                    AITimer++;
                    if (AITimer == 30 && !Main.dedServ)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Janitor.Start.1"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false))
                             .Add(new(NPC, ".[0.1].[0.1].[0.1]", Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, 0, false))
                             .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.Janitor.Start.2"), Colors.RarityYellow, new Color(100, 86, 0), voice, .03f, 2f, .5f, true, endID: 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 2000)
                    {
                        NPC.SetDefaults(ModContent.NPCType<JanitorBot>());
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            switch (signature)
            {
                case "a":
                    AITimer = 3000;
                    break;
                case "b":
                    EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 216);
                    break;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 3000;
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
                if ((AITimer >= 30 && AITimer < 226) || AITimer >= 380)
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
        public override bool CanHitNPC(NPC target) => false;
    }
}