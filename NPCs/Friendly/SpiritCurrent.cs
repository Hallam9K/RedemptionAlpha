using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Redemption.WorldGeneration;
using Terraria.DataStructures;
using Redemption.Globals;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.Friendly
{
    public class SpiritCurrent : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/SoullessPortal";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 188;
            NPC.height = 188;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 999;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
            NPC.npcSlots = 0;
        }
        public override bool CanHitNPC(NPC target) => false;
        public Point16 point;
        public Point16 origPoint;
        private Player zoomer;
        public override void AI()
        {
            if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
            {
                NPC.alpha += 10;
                if (NPC.alpha >= 255)
                {
                    bool oneActive = false;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[i];
                        if (!player.active || player.dead || !player.RedemptionAbility().SpiritwalkerActive)
                            continue;

                        oneActive = true;
                    }
                    if (!oneActive)
                        NPC.active = false;
                }
                return;
            }
            else
            {
                if (NPC.ai[0] >= 2 && NPC.alpha > 0)
                    NPC.alpha -= 10;
            }
            NPC.rotation += .02f;
            switch (NPC.ai[0])
            {
                case 0:
                    if (origPoint != Point16.Zero)
                    {
                        NPC.scale = 1;
                        NPC.alpha = 0;
                        if (NPC.ai[1]++ >= 120)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 2;
                        }
                        break;
                    }
                    origPoint = new((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16);
                    int attempts = 0;
                    while (attempts++ < 10 && point == Point16.Zero)
                    {
                        point = Main.rand.Next(6) switch
                        {
                            1 => RedeGen.SpiritAssassinPoint,
                            2 => RedeGen.SpiritCommonGuardPoint,
                            3 => RedeGen.HangingTiedPoint,
                            4 => RedeGen.SpiritOldLadyPoint,
                            5 => RedeGen.SpiritDruidPoint,
                            _ => RedeGen.SpiritOldManPoint,
                        };
                    }
                    if (attempts >= 10 || NPC.Center.DistanceSQ(point.ToVector2() * 16) < 8000 * 8000)
                        NPC.active = false;

                    int attempts2 = 0;
                    bool set = false;
                    while (attempts2++ < 1000 && !set)
                    {
                        Point16 offset = RedeHelper.PolarVector(Main.rand.Next(80, 131), RedeHelper.RandomRotation()).ToPoint16();
                        if (WorldGen.InWorld((point + offset).X, (point + offset).Y) && Framing.GetTileSafely((point + offset).X, (point + offset).Y).WallType is 0 && !Collision.SolidCollision(new Vector2(((point + offset).X * 16) - 50, ((point + offset).Y * 16) - 50), 100, 100))
                        {
                            point += offset;
                            set = true;
                        }
                    }
                    if (!set)
                        NPC.active = false;

                    NPC.scale = 0.1f;
                    NPC.ai[0] = 1;
                    break;
                case 1:
                    if (NPC.scale < 1)
                        NPC.scale += 0.02f;
                    NPC.alpha -= 3;
                    NPC.velocity.Y = -0.3f;
                    if (NPC.alpha <= 0)
                    {
                        NPC.velocity.Y = 0f;
                        NPC.ai[0] = 2;
                        NPC.scale = 1;
                        NPC.alpha = 0;
                    }
                    break;
                case 2:
                    NPC.velocity *= 0f;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[i];
                        if (!player.active || player.dead || NPC.DistanceSQ(player.Center) > 40 * 40)
                            continue;

                        for (int m = 0; m < 16; m++)
                        {
                            int dust = Dust.NewDust(player.Center - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 4f);
                            Main.dust[dust].noGravity = true;
                            Color dustColor = new(188, 244, 227) { A = 0 };
                            Main.dust[dust].color = dustColor;
                        }
                        zoomer = player;
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.PortalWub with { Volume = 1 });
                        NPC.ai[0] = 3;
                    }
                    break;
                case 3:
                    if (NPC.ai[1]++ < 40 && Main.myPlayer == zoomer.whoAmI)
                        Main.BlackFadeIn += 50;

                    zoomer.controlJump = false;
                    zoomer.controlDown = false;
                    zoomer.controlLeft = false;
                    zoomer.controlRight = false;
                    zoomer.controlUp = false;
                    zoomer.controlUseItem = false;
                    zoomer.controlUseTile = false;
                    zoomer.controlThrow = false;
                    zoomer.velocity *= 0.01f;
                    zoomer.noFallDmg = true;
                    zoomer.position = zoomer.oldPosition;
                    if (NPC.ai[1]++ >= 40)
                    {
                        zoomer.AddBuff(BuffID.Featherfall, 60);
                        if (Main.myPlayer == zoomer.whoAmI)
                            Main.BlackFadeIn = 500;

                        zoomer.Center = point.ToVector2() * 16 - new Vector2(0, 94);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int index = NPC.NewNPC(NPC.GetSource_FromAI(), point.X * 16, point.Y * 16, Type);
                            if (Main.npc[index].ModNPC is SpiritCurrent newCurrent)
                            {
                                newCurrent.origPoint = point;
                                newCurrent.point = origPoint;
                            }
                            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: index);
                        }
                        for (int m = 0; m < 16; m++)
                        {
                            int dust = Dust.NewDust(zoomer.Center - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 4f);
                            Main.dust[dust].noGravity = true;
                            Color dustColor = new(188, 244, 227) { A = 0 };
                            Main.dust[dust].color = dustColor;
                        }
                        NPC.active = false;
                    }
                    break;
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.RedemptionAbility().SpiritwalkerActive && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
                return SpawnCondition.Cavern.Chance * 0.3f;
            return 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D extra = ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            spriteBatch.End();
            spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * NPC.Opacity, -NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.2f, effects, 0f);
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.BeginDefault();

            spriteBatch.Draw(extra, NPC.Center - screenPos, null, Color.White * NPC.Opacity, -NPC.rotation, new Vector2(extra.Width / 2, extra.Height / 2), NPC.scale, effects, 0f);
            spriteBatch.Draw(extra, NPC.Center - screenPos, null, Color.White * NPC.Opacity * .4f, NPC.rotation, new Vector2(extra.Width / 2, extra.Height / 2), NPC.scale * 2, effects, 0f);
            spriteBatch.Draw(extra, NPC.Center - screenPos, null, Color.White * NPC.Opacity * .2f, -NPC.rotation, new Vector2(extra.Width / 2, extra.Height / 2), NPC.scale * 4, effects, 0f);
            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }
    }
}