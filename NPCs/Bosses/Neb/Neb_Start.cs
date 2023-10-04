using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.Audio;
using Redemption.Globals;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Effects.PrimitiveTrails;
using ReLogic.Content;
using Redemption.Base;

namespace Redemption.NPCs.Bosses.Neb
{
    public class Neb_Start : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 62;
            NPC.height = 84;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
            NPC.behindTiles = true;
        }
        Vector2 origin;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest(true);

            if (NPC.ai[0] < 5)
            {
                player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                player.RedemptionScreen().ScreenFocusPosition.Y = MathHelper.Max(NPC.Center.Y, 1200);
                player.RedemptionScreen().lockScreen = true;
            }
            player.RedemptionScreen().cutscene = true;

            NPC.rotation += 0.2f;
            switch (NPC.ai[0])
            {
                case 0:
                    player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
                    origin = player.Center + new Vector2(200, 0);
                    NPC.ai[0] = 1;
                    break;
                case 1:
                    NPC.Center = new Vector2(origin.X, 1200);
                    if (NPC.ai[2]++ < 80)
                        Main.BlackFadeIn += 50;
                    else
                        Main.BlackFadeIn += 20;

                    if (NPC.ai[2] == 80)
                        Main.BlackFadeIn = 255;

                    if (NPC.ai[2] >= 180)
                    {
                        Main.BlackFadeIn = 0;
                        NPC.scale = 0.1f;
                        NPC.ai[2] = 0;
                        NPC.ai[0] = 2;
                        NPC.netUpdate = true;
                    }
                    break;
                case 2:
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.scale >= 1.8f)
                    {
                        SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 1.2f, Volume = 0.2f }, NPC.position);
                        NPC.ai[0] = 3;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        NPC.alpha -= 20;
                        NPC.scale += 0.075f;
                    }
                    break;
                case 3:
                    if (NPC.scale > 0.1f)
                    {
                        NPC.alpha += 20;
                        NPC.scale -= 0.05f;
                    }
                    else if (NPC.ai[2]++ >= 80)
                    {
                        NPC.velocity.Y = 0f;
                        NPC.Center = new Vector2(origin.X, 100);
                        NPC.velocity.Y += 90;

                        NPC.ai[2] = 0;
                        NPC.ai[0] = 4;
                    }
                    break;
                case 4:
                    if (NPC.ai[2]++ == 5)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Neb_Start_Visual>(), 0, new Vector2(0, 90), SoundID.Item163, NPC.whoAmI);
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Neb_Start_Visual2>(), 0, new Vector2(0, 90), NPC.whoAmI);
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Neb_Start_Visual3>(), 0, new Vector2(0, 90), NPC.whoAmI);
                    }
                    if (NPC.ai[2] >= 5)
                        player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity, 10);

                    if (NPC.position.Y > player.position.Y - 80f)
                    {
                        NPC.ai[0] = 5;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case 5:
                    NPC.Center = new Vector2(origin.X, player.position.Y - 80f);
                    NPC.velocity *= 0;

                    SoundEngine.PlaySound(CustomSounds.NebSound3, NPC.position);
                    SoundEngine.PlaySound(CustomSounds.Teleport2, NPC.position);
                    DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.IndianRed, 5, 4, 3, 1, 2, 0, ai0: .05f, ai1: Main.rand.Next(50, 60));
                    DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Pink, 5, 5, 3, 1, 2, 0, ai0: .05f, ai1: Main.rand.Next(50, 60));
                    DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Purple, 5, 6, 3, 1, 2, 0, ai0: .05f, ai1: Main.rand.Next(50, 60));
                    DustHelper.DrawParticleStar<GlowParticle2>(NPC.Center, Color.Blue, 5, 7, 1, 3, 2, 0, ai0: .05f, ai1: Main.rand.Next(50, 60));
                    for (int d = 0; d < 16; d++)
                        ParticleManager.NewParticle(NPC.Center, RedeHelper.Spread(6), new RainbowParticle(), Color.White, 1);

                    RedeDraw.SpawnExplosion(NPC.Center, Color.White * 0.4f, scale: 6, noDust: true);
                    RedeDraw.SpawnExplosion(NPC.Center, Color.White * 0.2f, shakeAmount: 150, scale: 12, noDust: true);

                    NPC.netUpdate = true;
                    NPC.SetDefaults(ModContent.NPCType<Nebuleus>());
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Vector2 drawOrigin = new(glow.Width / 2, glow.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, NPC.Center - screenPos, null, Color.MediumPurple * NPC.Opacity, NPC.rotation, drawOrigin, NPC.scale * 0.4f, 0, 0);
            spriteBatch.Draw(glow, NPC.Center - screenPos, null, Color.MediumPurple * NPC.Opacity, -NPC.rotation, drawOrigin, NPC.scale * 0.4f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CheckDead()
        {
            NPC.life = 1;
            return false;
        }
        public override bool CheckActive() => false;
    }
    public class Neb_Start_Visual : ModProjectile, ITrailProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public int proType = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus");
        }
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 84;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 100;
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            if (proType == 0)
            {
                tManager.CreateTrail(Projectile, new RainbowTrail(5f, 0.002f, 1f, .75f), new RoundCap(), new DefaultTrailPosition(), 260f, 1600f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_1", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
                tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 170, 242, 0), new Color(255, 255, 255, 0)), new RoundCap(), new DefaultTrailPosition(), 130f, 600f);
            }
            else
                tManager.CreateTrail(Projectile, new RainbowTrail(4f, 0.002f, 1f, .8f), new RoundCap(), new DefaultTrailPosition(), 200f, 800f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_6", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }
        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.ai[0] == 5 || npc.type != ModContent.NPCType<Neb_Start>())
                Projectile.Kill();

            Projectile.rotation += 0.1f;
            if (proType != 0)
            {
                if (originalVelocity == Vector2.Zero)
                    originalVelocity = Projectile.velocity;

                if (offsetLeft)
                {
                    vectorOffset -= 0.26f;
                    if (vectorOffset <= -0.6f)
                    {
                        vectorOffset = -0.6f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset += 0.26f;
                    if (vectorOffset >= 0.6f)
                    {
                        vectorOffset = 0.6f;
                        offsetLeft = true;
                    }
                }
                float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
                Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));

                Projectile.position.Y = npc.position.Y - 100;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            if (proType == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Neb/GiantStar_Proj").Value;
                Vector2 position = Projectile.Center - Main.screenPosition;
                Rectangle rect = new(0, 0, texture.Width, texture.Height);
                Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Color color = new(Main.DiscoR * 6, Main.DiscoG * 6, Main.DiscoB * 6);
                Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), color * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.5f, 0, 0);
                Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), color * 0.7f * Projectile.Opacity, -Projectile.rotation, origin, Projectile.scale * 0.5f, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
    public class Neb_Start_Visual2 : Neb_Start_Visual
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 1;
            offsetLeft = false;
        }
    }
    public class Neb_Start_Visual3 : Neb_Start_Visual
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 2;
            offsetLeft = true;
        }
    }
}