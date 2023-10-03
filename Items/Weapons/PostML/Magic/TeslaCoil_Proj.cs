using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Redemption.Base;
using Redemption.Particles;
using Redemption.Buffs.NPCBuffs;
using System.Collections.Generic;
using ReLogic.Utilities;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class TeslaCoil_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Magic/TeslaCoil";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tesla Coil");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 86;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        private readonly List<int> targets = new();

        public float glow;
        private NPC target2;
        private SlotId loop;
        private ActiveSound sound;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active || !player.channel)
                Projectile.Kill();
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 coilPos = Projectile.Center + new Vector2(3 * Projectile.spriteDirection, -36);

            if (Projectile.owner == Main.myPlayer)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Projectile.localAI[0]++ % 3 == 0)
                        {
                            if (BasePlayer.ReduceMana(player, 3))
                            {
                                if (sound == null)
                                    loop = SoundEngine.PlaySound(CustomSounds.ElectricLoop, Projectile.position);
                                for (int k = 0; k < Main.rand.Next(2, 4); k++)
                                {
                                    Vector2 lightningArc = coilPos + RedeHelper.PolarVector(Main.rand.Next(40, 71), -MathHelper.Pi + Main.rand.NextFloat(0, MathHelper.Pi));
                                    DustHelper.DrawParticleElectricity<LightningParticle>(lightningArc, coilPos, 1f, 20, 0.05f);
                                    int side = -1;
                                    if (lightningArc.X > coilPos.X)
                                        side = 1;
                                    Vector2 lightningArc2 = Projectile.Center + new Vector2(Main.rand.Next(0, 261) * side, Main.rand.Next(24, 45));
                                    if (Projectile.ai[1] == 0)
                                    {
                                        targets.Clear();
                                        int target = -1;
                                        for (int i = 0; i < Main.maxNPCs; i++)
                                        {
                                            NPC npc = Main.npc[i];
                                            if (!npc.active || npc.friendly || npc.dontTakeDamage)
                                                continue;

                                            if (npc.DistanceSQ(player.Center) > 400 * 400)
                                                continue;

                                            targets.Add(npc.whoAmI);
                                            int[] targetsArr = targets.ToArray();
                                            target = Utils.Next(Main.rand, targetsArr);
                                        }
                                        if (target != -1 && Main.rand.NextBool(3))
                                            lightningArc2 = Main.npc[target].Center;
                                    }
                                    else
                                    {
                                        if (RedeHelper.ClosestNPC(ref target2, 80, Main.MouseWorld, true))
                                            lightningArc2 = target2.Center;
                                        else
                                            lightningArc2 = Main.MouseWorld;
                                    }
                                    float lagReduce = 0.05f;
                                    if (lightningArc.DistanceSQ(lightningArc2) > 400 * 400)
                                        lagReduce = 0.2f;
                                    DustHelper.DrawParticleElectricity<LightningParticle>(lightningArc, lightningArc2, 1f, 20, lagReduce);
                                    for (int i = 0; i < Main.maxNPCs; i++)
                                    {
                                        NPC npc = Main.npc[i];
                                        if (!npc.active || npc.friendly || npc.dontTakeDamage)
                                            continue;

                                        if (target2 != null)
                                        {
                                            int whoAmI = target2.whoAmI;
                                            if (whoAmI != -1)
                                            {
                                                if (Projectile.ai[1] == 1)
                                                {
                                                    if (npc.whoAmI != whoAmI || !Main.rand.NextBool(2))
                                                        continue;
                                                }
                                            }
                                        }

                                        if (npc.DistanceSQ(lightningArc2) > 60 * 60)
                                            continue;

                                        int hitDirection = npc.RightOfDir(Projectile);
                                        BaseAI.DamageNPC(npc, Projectile.damage + (npc.defense / 2), Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                                    }
                                }
                                glow += Main.rand.Next(-5, 6);
                                glow = (int)MathHelper.Clamp(glow, 0, 20);
                            }
                            else
                            {
                                if (sound != null)
                                {
                                    sound.Stop();
                                    loop = SlotId.Invalid;
                                }
                                glow = 0;
                            }
                        }
                        break;
                }
            }
            SoundEngine.TryGetActiveSound(loop, out sound);
            if (sound != null)
                sound.Position = Projectile.position;

            Projectile.spriteDirection = player.direction;
            Projectile.Center = playerCenter + new Vector2(6 * player.direction, -20);

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }
        public override void OnKill(int timeLeft)
        {
            if (sound != null)
            {
                sound.Stop();
                loop = SlotId.Invalid;
            }
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

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/Star").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center + new Vector2(3 * Projectile.spriteDirection, -36) - Main.screenPosition;
            Color colour = Color.Lerp(Color.White, Color.LightCyan, 1f / glow * 10f) * (1f / glow * 10f);

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.5f, Projectile.rotation, origin, 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}