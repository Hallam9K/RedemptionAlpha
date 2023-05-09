using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Tiles.Tiles;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Soulless
{
    public class TheStalker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 74;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }
        public bool RAAGH;
        public override void AI()
        {
            if (!SubworldSystem.IsActive<SoullessSub>() || SoullessArea.soullessInts[2] > 1)
                NPC.active = false;
            Player player = Main.player[Main.myPlayer];
            if (!player.Hitbox.Intersects(NPC.Hitbox))
                RAAGH = false;
            switch (NPC.ai[0])
            {
                case 0:
                    Lighting.AddLight(NPC.Center, .55f, .55f, .55f);
                    if (SoullessArea.soullessInts[1] > 1)
                        NPC.active = false;
                    break;
                case 1:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.spriteDirection = 1;
                            Rectangle activeZone = new((473 + SoullessArea.Offset.X) * 16, (1097 + SoullessArea.Offset.Y) * 16, 17 * 16, 9 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                            {
                                SoullessArea.soullessInts[2] = 1;
                                NPC.ai[1] = 1;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            break;
                        case 1:
                            if (SoullessArea.soullessInts[1] > 4)
                                NPC.active = false;

                            if (NPC.ai[2]++ == 0)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Vector2 eyesPos = RedeHelper.RandomPosition(new Vector2(510 + SoullessArea.Offset.X, 1118 + SoullessArea.Offset.Y), new Vector2(554 + SoullessArea.Offset.X, 1125 + SoullessArea.Offset.Y)) * 16;
                                    NPC.NewNPC(new EntitySource_SpawnNPC(), (int)eyesPos.X, (int)eyesPos.Y, ModContent.NPCType<StalkingEyes>());
                                }
                                NPC.Shoot(NPC.Center + new Vector2(18, 0), ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI);
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (284 + SoullessArea.Offset.X) * 16, (1084 + SoullessArea.Offset.Y) * 16, ModContent.NPCType<TheStalker_Fake>());
                            }
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[1] = 0;
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            if (NPC.ai[2]++ == 0)
                            {
                                NPC.Shoot(new Vector2(459 + SoullessArea.Offset.X, 1175 + SoullessArea.Offset.Y) * 16, ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI, 1);
                                NPC.Shoot(new Vector2(518 + SoullessArea.Offset.X, 1190 + SoullessArea.Offset.Y) * 16, ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI, 4);
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (434 + SoullessArea.Offset.X) * 16, (1169 + SoullessArea.Offset.Y) * 16, ModContent.NPCType<TheStalker_Fake>(), 1);
                            }

                            NPC.spriteDirection = 1;
                            if (SoullessArea.soullessInts[1] <= 5 && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                                NPC.ai[2] = 0;

                            Rectangle activeZone = new((376 + SoullessArea.Offset.X) * 16, (1201 + SoullessArea.Offset.Y) * 16, 40 * 16, 12 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                            {
                                NPC.ai[2] = 0;
                                SoullessArea.soullessInts[2] = 1;
                                NPC.ai[1] = 1;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            break;
                        case 1:
                            if (SoullessArea.soullessInts[1] > 6)
                                NPC.active = false;

                            if (NPC.ai[2]++ == 0)
                            {
                                for (int i = 0; i < 15; i++)
                                {
                                    Vector2 eyesPos = RedeHelper.RandomPosition(new Vector2(411 + SoullessArea.Offset.X, 1202 + SoullessArea.Offset.Y), new Vector2(448 + SoullessArea.Offset.X, 1210 + SoullessArea.Offset.Y)) * 16;
                                    RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)eyesPos.X, (int)eyesPos.Y, ModContent.NPCType<StalkingEyes>(), 1);
                                }
                                NPC.Shoot(NPC.Center + new Vector2(18, 0), ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI, 10);
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (439 + SoullessArea.Offset.X) * 16, (1234 + SoullessArea.Offset.Y) * 16, ModContent.NPCType<TheStalker_Fake>(), 2);
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (520 + SoullessArea.Offset.X) * 16, (1238 + SoullessArea.Offset.Y) * 16, ModContent.NPCType<LostLight>(), 4);
                            }
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[1] = 0;
                            }
                            break;
                    }
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D angy = ModContent.Request<Texture2D>(Texture + "2").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.NegativeDye);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            if (RAAGH)
                spriteBatch.Draw(angy, NPC.Center - new Vector2(76, 36) + RedeHelper.Spread(2) - screenPos, null, NPC.GetAlpha(drawColor) * .5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            else
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + RedeHelper.Spread(2) - screenPos, NPC.frame, NPC.GetAlpha(drawColor) * .5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (RAAGH)
                spriteBatch.Draw(angy, NPC.Center - new Vector2(76, 36) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            else
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override bool CheckActive() => false;
    }
    public class TheStalker_Fake : TheStalker
    {
        public override string Texture => "Redemption/NPCs/Soulless/TheStalker";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool PreAI()
        {
            Player player = Main.player[Main.myPlayer];
            if (NPC.ai[0] != 3 && !player.Hitbox.Intersects(NPC.Hitbox))
                RAAGH = false;
            if ((NPC.ai[0] != 1 && NPC.ai[0] != 3 && SoullessArea.soullessInts[2] != 1) || !SubworldSystem.IsActive<SoullessSub>())
                NPC.active = false;
            switch (NPC.ai[0])
            {
                case 0:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.spriteDirection = 1;
                            Rectangle activeZone = new((300 + SoullessArea.Offset.X) * 16, (1071 + SoullessArea.Offset.Y) * 16, 36 * 16, 21 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                                NPC.ai[1] = 1;
                            break;
                        case 1:
                            if (NPC.ai[2]++ == 0)
                            {
                                NPC.Shoot(NPC.Center + new Vector2(18, 0), ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI);
                            }
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[1] = 0;
                            }
                            break;
                    }
                    break;
                case 1:
                    if (SoullessArea.soullessInts[2] != 0)
                        NPC.active = false;
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.spriteDirection = -1;
                            Rectangle activeZone = new((379 + SoullessArea.Offset.X) * 16, (1167 + SoullessArea.Offset.Y) * 16, 24 * 16, 12 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                                NPC.ai[1] = 1;
                            break;
                        case 1:
                            if (NPC.ai[2]++ == 0)
                            {
                                NPC.Shoot(NPC.Center + new Vector2(18, 0), ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI, 10);
                            }
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[1] = 0;
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.spriteDirection = 1;
                            Rectangle activeZone = new((436 + SoullessArea.Offset.X) * 16, (1232 + SoullessArea.Offset.Y) * 16, 67 * 16, 19 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                                NPC.ai[1] = 1;
                            break;
                        case 1:
                            if (NPC.ai[2]++ == 0)
                            {
                                NPC.Shoot(NPC.Center + new Vector2(18, 0), ModContent.ProjectileType<TheStalker_Hand>(), 0, Vector2.Zero, NPC.whoAmI, 10);
                            }
                            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<TheStalker_Hand>()))
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.ai[1] = 0;
                            }
                            break;
                    }
                    break;
                case 3:
                    Main.LocalPlayer.RedemptionScreen().ScreenFocusPosition = (new Vector2(552, 1242) + SoullessArea.Offset.ToVector2()) * 16;
                    Main.LocalPlayer.RedemptionScreen().lockScreen = true;
                    Main.LocalPlayer.RedemptionScreen().cutscene = true;
                    switch (NPC.ai[1])
                    {
                        case 0:
                            RAAGH = true;
                            NPC.spriteDirection = 1;
                            NPC.alpha = 255;
                            NPC.ai[1] = 1;
                            SoundEngine.PlaySound(CustomSounds.SpookyNoise with { Pitch = -.3f });
                            break;
                        case 1:
                            NPC.alpha -= 2;
                            if (NPC.alpha <= 0)
                                NPC.ai[1] = 2;
                            break;
                        case 2:
                            if (NPC.ai[2]++ == 200)
                                SoundEngine.PlaySound(new("Redemption/Sounds/Ambient/SoullessNoise3"), NPC.position);
                            NPC light = Main.npc[(int)NPC.ai[3]];
                            if (light.ai[1] is 4)
                            {
                                for (int i = 0; i < 40; i++)
                                {
                                    Vector2 particlePos = RedeHelper.RandomPosition(new Vector2(545 + SoullessArea.Offset.X, 1244 + SoullessArea.Offset.Y), new Vector2(560 + SoullessArea.Offset.X, 1245 + SoullessArea.Offset.Y)) * 16;
                                    ParticleManager.NewParticle(particlePos, new Vector2(Main.rand.NextFloat(-1, 1), -Main.rand.NextFloat(1, 6)), new SoulParticle(), Color.White, 0.5f);
                                }
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    for (int x = 544 + SoullessArea.Offset.X; x < 559 + SoullessArea.Offset.X; x += 3)
                                    {
                                        Gore.NewGore(NPC.GetSource_OnHit(NPC), new Vector2(x, 1244) * 16, NPC.velocity, ModContent.Find<ModGore>("Redemption/ShadestoneSlabGore").Type, 1);
                                    }
                                    for (int x = 546 + SoullessArea.Offset.X; x < 558 + SoullessArea.Offset.X; x += 3)
                                    {
                                        Gore.NewGore(NPC.GetSource_OnHit(NPC), new Vector2(x, 1244) * 16, NPC.velocity, ModContent.Find<ModGore>("Redemption/ShadestoneSlabGore").Type, 1);
                                    }
                                }
                                for (int x = 545 + SoullessArea.Offset.X; x < 560 + SoullessArea.Offset.X; x++)
                                {
                                    for (int y = 1242 + SoullessArea.Offset.Y; y < 1247 + SoullessArea.Offset.Y; y++)
                                        WorldGen.KillTile(x, y, false, false, true);
                                }
                                Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity += 30;
                                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/ElevatorImpact"), NPC.position);
                                NPC.velocity = new(-2, -2);
                                NPC.ai[2] = 0;
                                NPC.ai[1] = 3;
                            }
                            break;
                        case 3:
                            int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Wraith, 0f, 0f, 100, default, 2f);
                            Main.dust[dustIndex].noGravity = true;
                            Main.dust[dustIndex].velocity.X = 0f;
                            Main.dust[dustIndex].velocity.Y = -2f;

                            NPC.velocity.X *= .97f;
                            NPC.rotation -= .01f;
                            if (NPC.ai[2]++ == 20)
                                SoundEngine.PlaySound(new("Redemption/Sounds/Ambient/SoullessNoise4") { Pitch = .2f }, NPC.position);
                            if (NPC.ai[2] >= 120)
                            {
                                NPC.active = false;
                                SoullessArea.soullessInts[1] = 8;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);
                            }
                            break;
                    }
                    break;
            }
            return false;
        }
    }
}