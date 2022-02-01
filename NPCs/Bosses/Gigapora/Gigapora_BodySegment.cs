using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Redemption.Buffs.NPCBuffs;
using Redemption.Buffs.Debuffs;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.Base;
using Redemption.BaseExtension;
using Terraria.Audio;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_BodySegment : Gigapora
    {
        public ref float SegmentType => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Gigapora");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 92;
            NPC.height = 82;
            NPC.dontCountMe = true;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public ref float Host => ref NPC.ai[1];
        public ref float FrameState => ref NPC.ai[0];
        private float shieldAlpha;
        public override bool PreAI()
        {
            if (NPC.immortal)
                shieldAlpha += 0.04f;
            else
                shieldAlpha -= 0.04f;
            shieldAlpha = MathHelper.Clamp(shieldAlpha, 0, 2);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile target = Main.projectile[i];
                if (!target.active || target.minion || !target.friendly || target.damage <= 0)
                    continue;

                if (target.velocity.Length() == 0 || target.Redemption().TechnicallyMelee || !NPC.Hitbox.Intersects(target.Hitbox))
                    continue;

                SoundEngine.PlaySound(SoundID.NPCHit4, NPC.position);
                if (NPC.immortal)
                    DustHelper.DrawCircle(target.Center, DustID.LifeDrain, 1, 2, 2, nogravity: true);
                target.Kill();
            }

            Vector2 chasePosition = Main.npc[(int)NPC.ai[1]].Center;
            Vector2 directionVector = chasePosition - NPC.Center;
            NPC.spriteDirection = (directionVector.X > 0f) ? 1 : -1;
            if (NPC.ai[3] > 0)
                NPC.realLife = (int)NPC.ai[3];
            if (NPC.target < 0 || NPC.target == byte.MaxValue || Main.player[NPC.target].dead)
                NPC.TargetClosest(true);
            if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
                NPC.timeLeft = 300;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[3]].type != ModContent.NPCType<Gigapora>())
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0.0f, 0.0f, 0, 0, 0);
                }
            }

            if (SegmentType >= 1 && SegmentType <= 6)
            {
                NPC.width = 120;
                NPC.height = 92;
            }
            if (SegmentType == 7)
            {
                NPC.width = 124;
                NPC.height = 124;
            }
            if (NPC.ai[1] < (double)Main.npc.Length)
            {
                float dirX = Main.npc[(int)NPC.ai[1]].Center.X - NPC.Center.X;
                float dirY = Main.npc[(int)NPC.ai[1]].Center.Y - NPC.Center.Y;
                NPC.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                float dist = (length - NPC.width / (SegmentType >= 1 && SegmentType <= 6 ? 2f : 1.6f)) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                if (dirX < 0f)
                    NPC.spriteDirection = 1;
                else
                    NPC.spriteDirection = -1;

                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + posX;
                NPC.position.Y = NPC.position.Y + posY;
            }

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);
            NPC.netUpdate = true;
            return false;
        }
        private int CoreFrame;
        private int TailFrame;
        public override void FindFrame(int frameHeight)
        {
            if (SegmentType >= 1 && SegmentType <= 6 && FrameState == 1)
            {
                CoreFrame = 1;
            }
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 5 * frameHeight)
                    NPC.frame.Y = 0;

                TailFrame++;
                if (TailFrame > 1)
                    TailFrame = 0;
            }
        }

        public override bool CheckActive() => false;
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (NPC.immortal)
            {
                SoundEngine.PlaySound(SoundID.NPCHit4, NPC.position);
                damage = 0;
                return false;
            }
            int ai3 = (int)NPC.ai[3];
            if (ai3 > -1 && ai3 < Main.maxNPCs && Main.npc[ai3].active && Main.npc[ai3].type == ModContent.NPCType<Gigapora>())
            {
                if (Main.npc[ai3].immune[Main.myPlayer] > 0)
                {
                    damage = 1;
                    crit = false;
                    return false;
                }
            }
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D core = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Core").Value;
            Texture2D coreGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Core_Glow").Value;
            Texture2D tail = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Tail").Value;
            Texture2D thrusterBlue = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Gigapora_ThrusterBlue").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float thrusterScaleX = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.5f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f, 0.5f);
            float thrusterScaleY = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f, 0.5f, 1.5f);
            float pulse = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1, 0.2f, 1);
            Vector2 pos = NPC.Center + new Vector2(0, 0);

            if (SegmentType >= 1 && SegmentType <= 6)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Vector2 thrusterBOrigin = new(thrusterBlue.Width / 2f, thrusterBlue.Height / 2f - 20);
                spriteBatch.Draw(thrusterBlue, pos + RedeHelper.PolarVector(52, NPC.rotation) + RedeHelper.PolarVector(39, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White, NPC.rotation, thrusterBOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                spriteBatch.Draw(thrusterBlue, pos + RedeHelper.PolarVector(-52, NPC.rotation) + RedeHelper.PolarVector(39, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White, NPC.rotation, thrusterBOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                int height = core.Height / 3;
                int y = height * CoreFrame;
                Vector2 coreOrigin = new(core.Width / 2f, height / 2f);
                spriteBatch.Draw(core, pos - screenPos, new Rectangle?(new Rectangle(0, y, core.Width, height)), drawColor, NPC.rotation, coreOrigin, NPC.scale, effects, 0);
                spriteBatch.Draw(coreGlow, pos - screenPos, new Rectangle?(new Rectangle(0, y, core.Width, height)), RedeColor.RedPulse, NPC.rotation, coreOrigin, NPC.scale, effects, 0);
                if (!NPC.IsABestiaryIconDummy && NPC.immortal && !Main.dedServ && spriteBatch != null)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                    Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:ScanShader"]?.GetShader().Shader;
                    effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                    effect.Parameters["alpha"].SetValue(shieldAlpha * pulse);
                    effect.Parameters["red"].SetValue(new Color(223, 62, 55).ToVector4());
                    effect.Parameters["red2"].SetValue(new Color(223, 62, 55).ToVector4());

                    effect.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(core, pos - screenPos, new Rectangle?(new Rectangle(0, y, core.Width, height)), Color.White, NPC.rotation, coreOrigin, NPC.scale, effects, 0);

                    spriteBatch.End();
                    spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                }
            }
            switch (SegmentType)
            {
                case 0:
                    spriteBatch.Draw(texture, pos - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    if (!NPC.IsABestiaryIconDummy && NPC.immortal && !Main.dedServ && spriteBatch != null)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                        Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:ScanShader"]?.GetShader().Shader;
                        effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                        effect.Parameters["alpha"].SetValue(shieldAlpha * pulse);
                        effect.Parameters["red"].SetValue(new Color(223, 62, 55).ToVector4());
                        effect.Parameters["red2"].SetValue(new Color(223, 62, 55).ToVector4());

                        effect.CurrentTechnique.Passes[0].Apply();
                        spriteBatch.Draw(texture, pos - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

                        spriteBatch.End();
                        spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                    }
                    break;
                case 7:
                    int height2 = tail.Height / 2;
                    int y2 = height2 * TailFrame;
                    Vector2 tailOrigin = new(tail.Width / 2f, height2 / 2f);
                    spriteBatch.Draw(tail, pos - screenPos, new Rectangle?(new Rectangle(0, y2, tail.Width, height2)), drawColor, NPC.rotation, tailOrigin, NPC.scale, effects, 0);
                    if (!NPC.IsABestiaryIconDummy && NPC.immortal && !Main.dedServ && spriteBatch != null)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                        Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:ScanShader"]?.GetShader().Shader;
                        effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                        effect.Parameters["alpha"].SetValue(shieldAlpha * pulse);
                        effect.Parameters["red"].SetValue(new Color(223, 62, 55).ToVector4());
                        effect.Parameters["red2"].SetValue(new Color(223, 62, 55).ToVector4());

                        effect.CurrentTechnique.Passes[0].Apply();
                        spriteBatch.Draw(tail, pos - screenPos, new Rectangle?(new Rectangle(0, y2, tail.Width, height2)), Color.White, NPC.rotation, tailOrigin, NPC.scale, effects, 0);

                        spriteBatch.End();
                        spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                    }
                    break;
            }
            return false;
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            int ai3 = (int)NPC.ai[3];
            if (ai3 > -1 && ai3 < Main.maxNPCs && Main.npc[ai3].active && Main.npc[ai3].type == ModContent.NPCType<Gigapora>())
            {
                if (Main.npc[ai3].immune[Main.myPlayer] == 0)
                    Main.npc[ai3].immune[Main.myPlayer] = NPC.immune[Main.myPlayer];
            }
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            int ai3 = (int)NPC.ai[3];
            if (ai3 > -1 && ai3 < Main.maxNPCs && Main.npc[ai3].active && Main.npc[ai3].type == ModContent.NPCType<Gigapora>())
            {
                if (Main.npc[ai3].immune[Main.myPlayer] == 0)
                    Main.npc[ai3].immune[Main.myPlayer] = NPC.immune[Main.myPlayer];
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
    }
}
