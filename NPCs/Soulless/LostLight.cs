using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using Redemption.Base;
using Terraria.GameContent.Bestiary;
using Redemption.Biomes;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Globals.NPC;
using Redemption.Tiles.Tiles;
using Terraria.Audio;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Soulless
{
    public class LostLight : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 30;
            NPC.height = 30;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.rarity = 3;
            NPC.alpha = 255;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return NPC.GetBestiaryEntryColor();
            return null;
        }
        private float alpha;
        private bool canFade;
        private readonly Point offset = SoullessArea.Offset;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(NPC), RedeHelper.Spread(1) - NPC.velocity / 4, new SoulParticle(), Color.White * 0.02f, 2, 0, -0.9f);

            Vector2 vector;
            double angle2 = Main.rand.NextDouble() * 2d * Math.PI;
            vector.X = (float)(Math.Sin(angle2) * 90);
            vector.Y = (float)(Math.Cos(angle2) * 90);
            Dust dust3 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.AncientLight, 0f, 0f, 100, default, 1f)];
            dust3.noGravity = true;
            dust3.velocity = -NPC.DirectionTo(dust3.position) * 4f;
            Lighting.AddLight(NPC.Center, 1, 1, 1);

            if (++NPC.ai[3] % 180 == 0)
            {
                string s = "Dunn olv syht...";
                switch (Main.rand.Next(9))
                {
                    case 1:
                        s = "I il lyht ka wye...";
                        break;
                    case 2:
                        s = "Folke...";
                        break;
                    case 3:
                        s = "Folke ufe...";
                        break;
                    case 4:
                        s = "Dozmu...";
                        break;
                    case 5:
                        s = "Jugh...";
                        break;
                    case 6:
                        s = "Qua sudki uque ka senkar'ka...";
                        break;
                    case 7:
                        s = "I il lyht ka senkar'ka...";
                        break;
                    case 8:
                        s = "Roma sudki'nin...";
                        break;
                }
                CombatText.NewText(NPC.getRect(), Color.GhostWhite, s, true, true);
            }
            Vector2 v;
            switch (NPC.ai[0])
            {
                case 0:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            v = (new Vector2(430, 787) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 4, 50);
                            break;
                        case 1:
                            if (NPC.ai[2]++ < 120)
                                NPC.velocity *= 0.94f;
                            else
                            {
                                NPC.ai[2] += 2;
                                v = ((new Vector2(424, 798) + offset.ToVector2()) * 16) + (Vector2.One.RotatedBy(MathHelper.ToRadians(NPC.ai[2])) * 40);
                                NPC.Move(v, 4, 20);
                            }
                            if (NPC.ai[2] >= 360 * 2)
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[1]++;
                            }
                            break;
                        case 2:
                            v = (new Vector2(414, 802) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 4, 50);
                            break;
                        case 3:
                            canFade = true;
                            NPC.velocity *= 0.96f;
                            NPC.alpha += 2;
                            if (NPC.alpha >= 255)
                            {
                                Main.BestiaryTracker.Kills.RegisterKill(NPC);
                                CombatText.NewText(NPC.getRect(), Color.GhostWhite, "Kliq...", true, false);
                                NPC.active = false;
                            }
                            break;
                    }
                    break;
                case 1:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            v = (new Vector2(340, 1115) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 6, 50);
                            break;
                        case 1:
                            canFade = true;
                            NPC.velocity *= 0.96f;
                            NPC.alpha += 2;
                            if (NPC.alpha >= 255)
                            {
                                CombatText.NewText(NPC.getRect(), Color.GhostWhite, "Jugh...", true, false);
                                NPC.active = false;
                            }
                            break;
                    }
                    break;
                case 2:
                    if (SoullessArea.soullessInts[2] is 0 && NPC.ai[1] < 5)
                        NPC.active = false;

                    switch (NPC.ai[1])
                    {
                        case 0:
                            v = (new Vector2(327, 1084) + offset.ToVector2()) * 16;
                            if (Main.LocalPlayer.DistanceSQ(NPC.Center) < 200 * 200)
                                NPC.ai[1]++;

                            NPC.Move(v, 4, 30);
                            break;
                        case 1:
                            v = (new Vector2(339, 1083) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 5, 30);
                            break;
                        case 2:
                            v = (new Vector2(349, 1074) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 5, 30);
                            break;
                        case 3:
                            v = (new Vector2(378, 1053) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 60 * 60)
                                NPC.ai[1]++;

                            NPC.Move(v, 5, 30);
                            break;
                        case 4:
                            NPC.ai[2] += 3;
                            v = ((new Vector2(378, 1053) + offset.ToVector2()) * 16) + (Vector2.One.RotatedBy(MathHelper.ToRadians(NPC.ai[2])) * 80);
                            NPC.Move(v, 4, 20);
                            if (Main.LocalPlayer.DistanceSQ(NPC.Center) < 120 * 120)
                            {
                                SoundEngine.PlaySound(SoundID.AbigailCry, NPC.position);
                                if (SoullessArea.soullessInts[1] < 5)
                                    SoullessArea.soullessInts[1] = 5;
                                SoullessArea.soullessInts[2] = 0;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);

                                NPC.ai[2] = 0;
                                NPC.ai[1]++;
                            }
                            break;
                        case 5:
                            v = (new Vector2(378, 1053) + offset.ToVector2()) * 16;
                            NPC.Move(v, 3, 1);
                            NPC.velocity *= .9f;
                            NPC.scale += 0.2f;
                            canFade = true;
                            NPC.velocity *= 0.96f;
                            NPC.alpha += 2;
                            if (NPC.alpha >= 255)
                            {
                                for (int x = 573 + offset.X; x < 585 + offset.X; x++)
                                {
                                    for (int y = 1117 + offset.Y; y < 1127 + offset.Y; y++)
                                    {
                                        if (Framing.GetTileSafely(x, y).TileType == ModContent.TileType<ShadestoneTile>())
                                            WorldGen.KillTile(x, y, false, false, true);
                                    }
                                }
                                RedeHelper.SpawnNPC(new EntitySource_WorldGen(), (414 + offset.X) * 16, (1059 + offset.Y) * 16, ModContent.NPCType<LostLight>(), 3);
                                CombatText.NewText(NPC.getRect(), Color.GhostWhite, "Qua lyht'ned...", true, false);
                                NPC.active = false;
                            }
                            break;
                    }
                    break;
                case 3:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            v = (new Vector2(394, 1053) + offset.ToVector2()) * 16;
                            if (Main.LocalPlayer.DistanceSQ(NPC.Center) < 200 * 200)
                                NPC.ai[1]++;

                            NPC.Move(v, 2, 50);
                            break;
                        case 1:
                            v = (new Vector2(424, 1070) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 6, 50);
                            break;
                        case 2:
                            v = (new Vector2(477, 1071) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 6, 50);
                            break;
                        case 3:
                            v = (new Vector2(480, 1102) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 6, 50);
                            break;
                        case 4:
                            v = (new Vector2(563, 1105) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 6, 50);
                            break;
                        case 5:
                            v = (new Vector2(575, 1124) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 20 * 20)
                                NPC.ai[1]++;

                            NPC.Move(v, 6, 50);
                            break;
                        case 6:
                            canFade = true;
                            NPC.velocity *= 0.96f;
                            NPC.alpha += 2;
                            if (NPC.alpha >= 255)
                            {
                                CombatText.NewText(NPC.getRect(), Color.GhostWhite, "Jugh...", true, false);
                                NPC.active = false;
                            }
                            break;
                    }
                    break;
                case 4:
                    if (SoullessArea.soullessInts[2] == 0)
                        NPC.active = false;
                    switch (NPC.ai[1])
                    {
                        case 0:
                            if (Main.LocalPlayer.DistanceSQ(NPC.Center) < 100 * 100)
                                NPC.ai[1]++;
                            break;
                        case 1:
                            v = (new Vector2(568, 1242) + offset.ToVector2()) * 16;
                            if (NPC.DistanceSQ(v) <= 80 * 80)
                                NPC.ai[1]++;

                            NPC.Move(v, 8, 20);
                            break;
                        case 2:
                            NPC.velocity *= .9f;
                            Rectangle activeZone = new((567 + SoullessArea.Offset.X) * 16, (1233 + SoullessArea.Offset.Y) * 16, 28 * 16, 11 * 16);
                            if (Main.LocalPlayer.Hitbox.Intersects(activeZone))
                            {
                                SoullessArea.soullessInts[1] = 7;
                                SoullessArea.soullessInts[2] = 3;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.WorldData);

                                NPC.ai[1]++;
                            }
                            break;
                        case 3:
                            Vector2 stalkerPos = (new Vector2(552, 1242) + offset.ToVector2()) * 16;
                            NPC.velocity *= .9f;

                            if (NPC.ai[2]++ == 120)
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)stalkerPos.X, (int)stalkerPos.Y, ModContent.NPCType<TheStalker_Fake>(), 3, 0, 0, NPC.whoAmI);

                            if (NPC.ai[2] >= 160 && NPC.ai[2] < 365)
                            {
                                float aiOffset = NPC.ai[2] - 160;
                                v = stalkerPos + (Vector2.One.RotatedBy(MathHelper.ToRadians(NPC.ai[2] * (aiOffset / 100))) * (100 - (aiOffset / 4)));
                                NPC.Move(v, 5 + (aiOffset / 2), 30);
                                if (NPC.ai[2] == 220)
                                    SoundEngine.PlaySound(SoundID.AbigailCry with { Pitch = -.4f }, NPC.position);
                                if (NPC.ai[2] >= 220)
                                {
                                    Vector2 vector2;
                                    vector2.X = (float)(Math.Sin(angle2) * 200);
                                    vector2.Y = (float)(Math.Cos(angle2) * 200);
                                    Dust dust4 = Main.dust[Dust.NewDust(NPC.Center + vector2, 2, 2, DustID.AncientLight, 0f, 0f, 100, default, 2f)];
                                    dust4.noGravity = true;
                                    dust4.velocity = -NPC.DirectionTo(dust3.position) * 10f;
                                }
                            }
                            else if (NPC.ai[2] >= 365 && NPC.ai[2] < 500)
                            {
                                v = (new Vector2(552, 1242 - 18) + offset.ToVector2()) * 16;
                                if (NPC.DistanceSQ(v) <= 80 * 80)
                                    NPC.ai[2] = 500;
                                NPC.Move(v, 40, 40);
                            }
                            else if (NPC.ai[2] >= 500)
                            {
                                if (NPC.ai[2] == 500)
                                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse with { Pitch = .5f }, NPC.position);
                                NPC.velocity.X *= .7f;
                                if (NPC.ai[2] < 540)
                                {
                                    if (NPC.Center.X < stalkerPos.X)
                                        NPC.velocity.X += 1;
                                    else if (NPC.Center.X > stalkerPos.X)
                                        NPC.velocity.X -= 1;
                                }
                                else
                                    NPC.velocity.Y += 8;

                                if (NPC.Center.Y >= stalkerPos.Y + 16)
                                {
                                    SoundEngine.PlaySound(SoundID.AbigailCry with { Pitch = -.1f }, NPC.position);
                                    NPC.velocity *= 0;
                                    NPC.ai[1]++;
                                }
                            }
                            break;
                        case 4:
                            NPC.scale += 0.4f;
                            canFade = true;
                            NPC.alpha += 5;
                            if (NPC.alpha >= 255)
                                NPC.active = false;
                            break;
                    }
                    break;
            }
            NPC.LookByVelocity();
            NPC.rotation = NPC.velocity.X * 0.05f;
            alpha += Main.rand.NextFloat(-0.05f, 0.05f);
            alpha = MathHelper.Clamp(alpha, 0, 0.3f);
            if (NPC.alpha > 100 && !canFade)
                NPC.alpha -= 2;
            else if (!canFade)
            {
                NPC.alpha += Main.rand.Next(-10, 11);
                NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 50, 100);
            }
        }
        public override bool CheckActive() => false;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D LightGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 0.8f, 1.2f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.4f, 0.8f, 0.4f);
            Vector2 drawOrigin = new(LightGlow.Width / 2, LightGlow.Height / 2);

            if (NPC.IsABestiaryIconDummy)
            {
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, Color.White * alpha, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                return true;
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, Color.White * alpha, NPC.rotation, NPC.frame.Size() / 2, 1, effects, 0);
            spriteBatch.Draw(LightGlow, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, NPC.scale * 0.2f, effects, 0);
            spriteBatch.Draw(LightGlow, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White) * 0.6f, NPC.rotation, drawOrigin, NPC.scale * scale, effects, 0);
            spriteBatch.Draw(LightGlow, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White) * 0.8f, NPC.rotation, drawOrigin, NPC.scale * scale2, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CustomCollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], true, 1);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Spirits of those freed of sorrow. They offer light to the misplaced and misfortuned, a miracle in the darkest depths. Their guidance, however, is seldom understood for newcomers, for the ancient tongue of which they speak has been lost to the ages.")
            });
        }
    }
}