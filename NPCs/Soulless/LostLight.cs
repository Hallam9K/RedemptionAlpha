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
                            v = new Vector2(430, 787) * 16;
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
                                v = (new Vector2(424, 798) * 16) + (Vector2.One.RotatedBy(MathHelper.ToRadians(NPC.ai[2])) * 40);
                                NPC.Move(v, 4, 20);
                            }
                            if (NPC.ai[2] >= 360 * 2)
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[1]++;
                            }
                            break;
                        case 2:
                            v = new Vector2(414, 802) * 16;
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
                                for (int i = 0; i < 50; i++)
                                    Main.BestiaryTracker.Kills.RegisterKill(NPC);
                                CombatText.NewText(NPC.getRect(), Color.GhostWhite, "Kliq...", true, false);
                                NPC.active = false;
                            }
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

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, Color.White * alpha, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(LightGlow, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, 0.2f, effects, 0);
            spriteBatch.Draw(LightGlow, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White) * 0.6f, NPC.rotation, drawOrigin, scale, effects, 0);
            spriteBatch.Draw(LightGlow, NPC.Center - screenPos, null, NPC.GetAlpha(Color.White) * 0.8f, NPC.rotation, drawOrigin, scale2, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Spirits of those freed of sorrow. They offer light to the misplaced and misfortuned, a miracle in the darkest depths. Their guidance, however, is seldom understood for newcomers, for the ancient tongue of which they speak has been lost to the ages.")
            });
        }
    }
}