using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using Redemption.Base;
using Redemption.Particles;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class LightningRod_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Magic/LightningRod";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Self-Sufficient Lighting Rod");
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
        }
        public float glow;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            if (Projectile.owner == Main.myPlayer)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Projectile.ai[1]++ >= 60)
                            glow += 0.01f;
                        if (Projectile.ai[1] == 30 && !Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Spark1, Projectile.position);

                        if (Projectile.ai[1] >= 30 && (Projectile.ai[1] >= 60 ? Main.rand.NextBool(6) : Main.rand.NextBool(10)))
                            DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center + new Vector2(32 * player.direction, -32), Projectile.Center + new Vector2(32 * player.direction, -32) + RedeHelper.PolarVector(30 * (glow + 1), RedeHelper.RandomRotation()), .2f, 5, 0.1f);

                        if (!player.channel || glow >= 0.8f)
                        {
                            Projectile.ai[0]++;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        if (Projectile.ai[1] < 30)
                        {
                            Projectile.Kill();
                            break;
                        }

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.ElectricNoise, Projectile.position);
                        DustHelper.DrawCircle(Projectile.Center + new Vector2(36 * player.direction, -36), DustID.Electric, 3, 2, 1, 1, 1, nogravity: true);
                        int dmg = (int)Projectile.ai[1] * 2;
                        DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center + new Vector2(36 * player.direction, -36), Main.MouseWorld, 1.3f, 30, 0.05f);
                        DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center + new Vector2(36 * player.direction, -36), Main.MouseWorld, 1.3f, 30, 0.05f);
                        if (glow < 0.8f)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];
                                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                                    continue;

                                if (npc.DistanceSQ(Main.MouseWorld) > 60 * 60)
                                    continue;

                                int hitDirection = npc.RightOfDir(Projectile);
                                BaseAI.DamageNPC(npc, Projectile.damage + dmg * 2, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                            }
                        }
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (!npc.active || npc.friendly || npc.dontTakeDamage)
                                continue;

                            if (glow >= 0.8f)
                            {
                                if (Projectile.DistanceSQ(npc.Center) > 600 * 600)
                                    continue;
                            }
                            else
                            {
                                if (Projectile.DistanceSQ(npc.Center) > 400 * 400 || Main.rand.NextBool((int)Projectile.ai[1] / 10))
                                    continue;
                            }
                            DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center + new Vector2(36 * player.direction, -36), npc.Center, 1.3f, 30, 0.05f);
                            DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center + new Vector2(36 * player.direction, -36), npc.Center, 1.3f, 30, 0.05f);
                            int hitDirection = npc.RightOfDir(Projectile);
                            if (glow >= 0.8f)
                                BaseAI.DamageNPC(npc, Projectile.damage * 2, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                            else
                                BaseAI.DamageNPC(npc, Projectile.damage + dmg * 2, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                        }
                        Projectile.ai[1] = 0;
                        Projectile.ai[0]++;
                        break;
                    case 2:
                        if (Projectile.localAI[1]++ >= 30)
                            Projectile.Kill();
                        break;
                }
            }

            Projectile.spriteDirection = player.direction;
            Projectile.Center = playerCenter + new Vector2(30 * player.direction, -30);

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(Color.Orange) * glow, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}