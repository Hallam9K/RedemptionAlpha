using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorBot_Defeated : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Lab/Janitor/JanitorBot";

        public ref float State => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 19;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 40;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
            NPC.dontTakeDamage = true;
            NPC.npcSlots = 0;
        }

        private Vector2 moveTo;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();
            NPC.LookByVelocity();

            moveTo = new((RedeGen.LabVector.X + 190) * 16, (RedeGen.LabVector.Y + 21) * 16);
            switch (State)
            {
                case 0:
                    if (AITimer++ == 10)
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Okay, okay!", true, false);
                    if (AITimer == 100)
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Alright fine, you probably can handle yourself here.", true, false);
                    if (AITimer == 260)
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Here, have this Lab Access thing and get lost!", true, false);
                    if (AITimer == 390)
                        CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "I got moppin' to do.", true, false);
                    if (AITimer >= 390)
                    {
                        if (NPC.DistanceSQ(moveTo) < 16 * 16)
                        {
                            AITimer = 0;
                            NPC.velocity.X = 0;
                            State = 2;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            bool jumpDownPlatforms = false;
                            NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                            if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                            else { NPC.noTileCollide = false; }
                            RedeHelper.HorizontallyMove(NPC, moveTo, 0.4f, 1, 12, 8, NPC.Center.Y > moveTo.Y);
                        }
                    }
                    break;
                case 1:
                    if (NPC.DistanceSQ(moveTo) < 16 * 16)
                    {
                        AITimer = 0;
                        NPC.velocity.X = 0;
                        State = 2;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        bool jumpDownPlatforms = false;
                        NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                        if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                        else { NPC.noTileCollide = false; }
                        RedeHelper.HorizontallyMove(NPC, moveTo, 0.4f, 1, 12, 8, NPC.Center.Y > moveTo.Y);
                    }
                    break;
                case 2:
                    if (AITimer++ >= 60)
                    {
                        for (int g = 0; g < 4; g++)
                        {
                            int goreIndex = Gore.NewGore(NPC.Center, default, Main.rand.Next(61, 64), 2f);
                            Main.gore[goreIndex].velocity.X += 1.5f;
                            Main.gore[goreIndex].velocity.Y += 1.5f;
                        }
                        NPC.alpha = 255;
                        NPC.active = false;
                    }
                    break;
            }
        }
        private int WalkFrame;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.X == 0)
            {
                if (NPC.frameCounter++ >= 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.frameCounter += NPC.velocity.X * 0.5f;
                if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                {
                    NPC.frameCounter = 0;
                    WalkFrame++;
                    if (WalkFrame >= 7)
                        WalkFrame = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D WalkAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_WalkAway").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (NPC.velocity.X != 0)
            {
                int Height = WalkAni.Height / 7;
                int y = Height * WalkFrame;
                Rectangle rect = new(0, y, WalkAni.Width, Height);
                Vector2 origin = new(WalkAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(WalkAni, NPC.Center - screenPos - new Vector2(0, 3), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
                spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
    }
}