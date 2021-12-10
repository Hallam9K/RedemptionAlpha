using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using Redemption.Items.Usable;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Audio;
using Redemption.Base;
using ReLogic.Content;
using Redemption.WorldGeneration;

namespace Redemption.NPCs.Lab.Volt
{
    public class ProtectorVolt_Start : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Lab/Volt/ProtectorVolt";
        public ref float State => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Protector Volt");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 70;
            NPC.friendly = false;
            NPC.lifeMax = 140000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.knockBackResist = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            NPC.netAlways = true;
            NPC.aiStyle = -1;
        }
        public override bool CheckActive()
        {
            return !LabArea.Active;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);

            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (RedeBossDowned.downedVolt)
                NPC.Transform(ModContent.NPCType<ProtectorVolt_NPC>());
            else
            {
                switch (State)
                {
                    case 0:
                        if (NPC.DistanceSQ(player.Center) < 300 * 300 && player.Center.X > NPC.Center.X)
                        {
                            if (player.IsFullTBot())
                                State = 2;
                            else
                                State = 1;
                            NPC.netUpdate = true;
                        }
                        break;
                    case 1:
                        if (RedeBossDowned.voltBegin)
                        {
                            AITimer++;
                            if (AITimer >= 60)
                            {
                                NPC.Transform(ModContent.NPCType<ProtectorVolt>());
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            AITimer++;
                            if (AITimer == 40)
                                CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Halt!", true, false);
                            if (AITimer == 120)
                                CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "You aren't supposed to be here!", true, false);
                            if (AITimer == 260)
                            {
                                NPC.velocity.Y = -8;
                            }
                            if ((NPC.collideY || NPC.velocity.Y == 0) && AITimer >= 270)
                            {
                                Mod mod = Redemption.Instance;

                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/EarthBoom"), NPC.position);

                                for (int i = 0; i < 40; i++)
                                    Dust.NewDust(NPC.BottomLeft, Main.rand.Next(NPC.width), 1, DustID.Smoke, 0, 0, 0, default, 2f);

                                Dictionary<Color, int> colorToTile = new()
                                {
                                    [new Color(150, 150, 150)] = -2,
                                    [Color.Black] = -1
                                };

                                Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/VoltDestroy", AssetRequestMode.ImmediateLoad).Value;
                                Point origin = RedeGen.LabVector.ToPoint();
                                Main.QueueMainThreadAction(() =>
                                {
                                    TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                                    gen.Generate(origin.X, origin.Y, true, true);
                                });

                                State = 3;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                    case 2:
                        AITimer++;
                        if (AITimer == 40)
                            CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Hm? Are you supposed to be let through?", true, false);
                        if (AITimer == 220)
                            CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "One second...", true, false);
                        if (AITimer == 340)
                            CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "...", true, false);
                        if (AITimer == 500)
                            CombatText.NewText(NPC.getRect(), Colors.RarityYellow, "Everything seems to be in order. Move along.", true, false);
                        if (AITimer > 560)
                        {
                            if (!LabArea.labAccess[3])
                                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ModContent.ItemType<ZoneAccessPanel4>());

                            RedeBossDowned.downedVolt = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);

                            NPC.Transform(ModContent.NPCType<ProtectorVolt_NPC>());
                            NPC.netUpdate = true;
                        }
                        break;
                    case 3:
                        AITimer++;
                        if (AITimer >= 60)
                        {
                            RedeBossDowned.voltBegin = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);

                            NPC.Transform(ModContent.NPCType<ProtectorVolt>());
                        }
                        break;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Length() == 0)
            {
                if (NPC.frameCounter++ >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
                NPC.frame.Y = 4 * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D GunTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Gun").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            Vector2 gunCenter = new(NPC.Center.X, NPC.Center.Y + 6);
            int height = GunTex.Height / 4;
            spriteBatch.Draw(GunTex, gunCenter - screenPos, new Rectangle?(new Rectangle(0, 0, GunTex.Width, height)), drawColor, NPC.rotation, new Vector2(GunTex.Width / 2f, height / 2f), NPC.scale, effects, 0f);
            return false;
        }
    }
}