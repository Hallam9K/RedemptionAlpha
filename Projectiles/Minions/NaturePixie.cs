using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Redemption.NPCs.PreHM;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class NaturePixie : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            ElementID.ProjNature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 28;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanHitNPC(NPC target) => !target.friendly && target.type != ModContent.NPCType<ForestNymph>() ? null : false;
        public override bool MinionContactDamage() => Projectile.velocity.Length() > 10;
        NPC target;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            CheckActive(owner);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
            Projectile.LookByVelocity();
            if (Projectile.ai[1] == 0)
                Projectile.rotation = Projectile.velocity.X * 0.05f;
            Lighting.AddLight(Projectile.Center, .1f * Projectile.Opacity, .4f * Projectile.Opacity, .1f * Projectile.Opacity);

            if (Main.rand.NextBool() && Projectile.velocity.Length() > 10)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DryadsWard, Scale: 1.5f);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }

            if (Projectile.ai[1] == 2)
            {
                int nymphID = NPC.FindFirstNPC(ModContent.NPCType<ForestNymph>());
                if (nymphID != -1)
                {
                    NPC nymph = Main.npc[nymphID];
                    if (nymph.active && nymph.ai[0] >= 8)
                    {
                        if (Projectile.DistanceSQ(nymph.Center) >= 150 * 150)
                            Projectile.Move(nymph.Center, 18, 20);
                        else
                            Projectile.velocity *= .9f;

                        if (Projectile.localAI[0]++ == 30)
                        {
                            SoundEngine.PlaySound(CustomSounds.Pixie1, Projectile.position);
                            CombatText.NewText(Projectile.getRect(), Color.LightGreen, "Hello!", false, true);
                            nymph.LookAtEntity(Projectile);
                        }
                        if (Projectile.localAI[0] == 180)
                        {
                            Projectile.velocity = Projectile.Center.DirectionTo(owner.Center) * 6;
                            SoundEngine.PlaySound(CustomSounds.Pixie3, Projectile.position);
                            CombatText.NewText(Projectile.getRect(), Color.LightGreen, "Listen!", false, true);
                            EmoteBubble.NewBubble(0, new WorldUIAnchor(Projectile), 120);
                        }
                        if (Projectile.localAI[0] >= 120 && Projectile.localAI[0] < 280)
                        {
                            Projectile.LookAtEntity(owner);
                            Projectile.rotation.SlowRotation((owner.Center - Projectile.Center).ToRotation() + (Projectile.spriteDirection == 1 ? 0 : MathHelper.Pi), (float)Math.PI / 40);
                        }
                        else
                        {
                            Projectile.LookAtEntity(nymph);
                            Projectile.rotation.SlowRotation((nymph.Center - Projectile.Center).ToRotation() + (Projectile.spriteDirection == 1 ? 0 : MathHelper.Pi), (float)Math.PI / 40);
                        }
                        if (Projectile.localAI[0] >= 340)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.localAI[0] = 0;
                        }
                    }
                    else
                    {
                        Projectile.ai[1] = 0;
                        Projectile.localAI[0] = 0;
                    }
                }
                return;
            }
            if (RedeHelper.ClosestNPC(ref target, 600, Projectile.Center, true, owner.MinionAttackTargetNPC) && target.type != ModContent.NPCType<ForestNymph>())
            {
                if (Projectile.ai[1] == 1)
                {
                    Projectile.rotation.SlowRotation((target.Center - Projectile.Center).ToRotation() + (Projectile.spriteDirection == 1 ? 0 : MathHelper.Pi), (float)Math.PI / 40);

                    if (Projectile.DistanceSQ(target.Center) >= 40 * 40)
                        Projectile.Move(target.Center, 18, 20);
                    else
                        Projectile.velocity *= .9f;
                    if (Projectile.localAI[0]++ == 40)
                    {
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                SoundEngine.PlaySound(CustomSounds.Pixie1, Projectile.position);
                                CombatText.NewText(Projectile.getRect(), Color.LightGreen, "Hello!", false, true);
                                break;
                            case 1:
                                SoundEngine.PlaySound(CustomSounds.Pixie2, Projectile.position);
                                CombatText.NewText(Projectile.getRect(), Color.LightGreen, "Hey!", false, true);
                                break;
                            case 2:
                                SoundEngine.PlaySound(CustomSounds.Pixie3, Projectile.position);
                                CombatText.NewText(Projectile.getRect(), Color.LightGreen, "Listen!", false, true);
                                break;
                        }
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NaturePixie_Yell>(), Projectile.damage, 0, owner.whoAmI);
                        }
                    }
                    if (Projectile.localAI[0] >= 80)
                    {
                        Projectile.velocity += Projectile.Center.DirectionTo(target.Center);
                        Projectile.ai[1] = 0;
                    }
                }
                else
                {
                    if (Projectile.localAI[0] == 0)
                        Projectile.localAI[0] = Main.rand.Next(20, 41);

                    Projectile.ai[0]++;
                    if (Projectile.DistanceSQ(target.Center) >= 180 * 180)
                        Projectile.Move(target.Center, 12, 30);
                    else
                    {
                        if (Projectile.ai[0] % Projectile.localAI[0] == 0)
                        {
                            Projectile.localAI[0] = 0;
                            if (!Main.rand.NextBool(4))
                                Projectile.velocity = Projectile.Center.DirectionTo(target.Center) * 18;
                            else
                            {
                                Projectile.localAI[0] = 0;
                                Projectile.ai[1] = 1;
                            }
                        }
                    }
                }
            }
            else
            {
                Projectile.ai[1] = 0;
                if (((RedeWorld.alignment >= 1 && !RedeBossDowned.downedTreebark) || (RedeWorld.alignment >= 3 && RedeBossDowned.downedTreebark)) && Projectile.localAI[1]++ % 60 == 0)
                {
                    int nymphID = NPC.FindFirstNPC(ModContent.NPCType<ForestNymph>());
                    if (nymphID != -1)
                    {
                        NPC nymph = Main.npc[nymphID];
                        if (nymph.active && nymph.ai[0] <= 2 && Projectile.DistanceSQ(nymph.Center) <= 500 * 500)
                        {
                            if (Projectile.DistanceSQ(nymph.Center) < 150 * 150)
                            {
                                Projectile.localAI[1] = 0;
                                Projectile.localAI[0] = 0;
                                Projectile.ai[1] = 2;
                                if (nymph.ai[0] < 8)
                                {
                                    nymph.ai[1] = 0;
                                    nymph.ai[2] = 0;
                                    nymph.ai[0] = 8;
                                }
                            }
                            else
                                Projectile.Move(nymph.Center, 10, 20);
                            return;
                        }
                    }
                }
                if (Projectile.velocity.Length() < 6)
                    Projectile.velocity *= 1.02f;
                if (Projectile.DistanceSQ(owner.Center) >= 100 * 100)
                    Projectile.Move(owner.Center, Projectile.DistanceSQ(owner.Center) > 700 * 700 ? 22 : 12, 20);
                else
                    Projectile.velocity *= 0.98f;
            }
            if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
            {
                Projectile.position = owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(glow, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<NaturePixieBuff>()))
                Projectile.timeLeft = 2;
        }
    }
    public class NaturePixie_Yell : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Yell");
            ElementID.ProjPsychic[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 4;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override bool? CanCutTiles() => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
                target.AddBuff(BuffID.Confused, 60);
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }
    }
}