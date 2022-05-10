using Redemption.Globals;
using Redemption.Items.Usable;
using Terraria;
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
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            switch (State)
            {
                case 0:
                    if (NPC.DistanceSQ(player.Center) <= 200 * 200 && player.Center.X < NPC.Center.X)
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
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "...Why did you have to barge in through the ventilation shaft?", true, false);
                    if (AITimer == 240)
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Lost your access card huh? Have mine and get out of my sight.", true, false);
                    if (AITimer >= 400)
                    {
                        EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "*Grumbles* Those darn careless bots losing their cards...", true, false);

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
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Oi! Don't go there, the floor's wet.", true, false);
                    if (AITimer == 180)
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "...", false, false);
                    if (AITimer == 280)
                    {
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(NPC), 120);
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Wait... You're a trespasser!", true, false);
                    }
                    if (AITimer >= 400)
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
                if ((AITimer >= 30 && AITimer < 120) || AITimer >= 240)
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