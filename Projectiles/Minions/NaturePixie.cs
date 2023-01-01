using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class NaturePixie : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => Projectile.velocity.Length() > 10;
        NPC target;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            CheckActive(owner);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.LookByVelocity();
            if (Projectile.ai[1] == 0)
                Projectile.rotation.SlowRotation(Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? 0 : MathHelper.Pi), (float)Math.PI / 20);
            Lighting.AddLight(Projectile.Center, .1f * Projectile.Opacity, .4f * Projectile.Opacity, .1f * Projectile.Opacity);

            if (RedeHelper.ClosestNPC(ref target, 600, Projectile.Center, true, owner.MinionAttackTargetNPC))
            {
                if (Projectile.ai[1] == 1)
                {
                    Projectile.rotation.SlowRotation((target.Center - Projectile.Center).ToRotation() + (Projectile.spriteDirection == -1 ? 0 : MathHelper.Pi), (float)Math.PI / 40);

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
                if (Projectile.velocity.Length() < 12)
                    Projectile.velocity *= 1.02f;
                if (Projectile.DistanceSQ(owner.Center) >= 100 * 100)
                    Projectile.Move(owner.Center, Projectile.DistanceSQ(owner.Center) > 700 * 700 ? 22 : 12, 20);
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
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.LightGreen) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

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
            DisplayName.SetDefault("Yell");
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
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool())
                target.AddBuff(BuffID.Confused, 60);
            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }
    }
}