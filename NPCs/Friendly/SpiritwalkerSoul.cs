using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.DataStructures;
using Redemption.Particles;
using ParticleLibrary;
using Redemption.BaseExtension;
using System;
using Redemption.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;

namespace Redemption.NPCs.Friendly
{
    public class SpiritwalkerSoul : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Friendly/LostSoulNPC";
        public ref float AITimer => ref NPC.ai[0];
        public ref float TimerRand => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lost Soul");
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 10;
            NPC.height = 10;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.lavaImmune = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.alpha = 200;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }
        public override void AI()
        {
            ParticleManager.NewParticle(NPC.RandAreaInEntity() + (NPC.velocity * 2), Vector2.Zero, new SpiritParticle(), Color.White, 0.6f * NPC.scale, 0, 1);
            if (Main.rand.NextBool(3))
                ParticleManager.NewParticle(NPC.Center, RedeHelper.Spread(2), new SpiritParticle(), Color.White, 1);

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            RedeSystem.Silence = true;
            switch (TimerRand)
            {
                case 0:
                    player.RedemptionScreen().cutscene = true;
                    player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                    player.RedemptionScreen().lockScreen = true;

                    if (AITimer++ == 0)
                            NPC.velocity.Y = -6;

                    NPC.velocity *= 0.98f;
                    if (AITimer >= 120)
                    {
                        NPC.MoveToVector2(player.Center, 10);
                        if (NPC.Hitbox.Intersects(player.Hitbox))
                        {
                            AITimer = 0;
                            TimerRand = 1;
                            NPC.velocity *= 0;
                        }
                    }
                    break;
                case 1:
                    player.RedemptionScreen().cutscene = true;
                    NPC.MoveToVector2(player.Center, 10);
                    player.position = player.oldPosition;
                    player.velocity *= 0;

                    if (AITimer++ == 60)
                        SoundEngine.PlaySound(CustomSounds.EnergyCharge, player.position);

                    if (AITimer >= 60)
                    {
                        player.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(player.RedemptionScreen().ScreenShakeIntensity, AITimer / 30);
                        for (int k = 0; k < 3; k++)
                        {
                            Vector2 vector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            vector.X = (float)(Math.Sin(angle) * 1000);
                            vector.Y = (float)(Math.Cos(angle) * 1000);
                            Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector, 2, 2, ModContent.DustType<GlowDust>())];
                            dust2.noGravity = true;
                            dust2.noLight = true;
                            Color dustColor = new(180, 255, 255) { A = 0 };
                            dust2.color = dustColor;
                            dust2.velocity = dust2.position.DirectionTo(player.Center) * 40f;
                        }
                        player.RedemptionAbility().SpiritwalkerActive = true;
                    }
                    if (AITimer >= 260)
                    {
                        string s = Language.GetTextValue("Mods.Redemption.UI.SpiritWalker.Keybind");
                        foreach (string key in Redemption.RedeSpiritwalkerAbility.GetAssignedKeys())
                        {
                            s = Language.GetTextValue("Mods.Redemption.UI.SpiritWalker.Hold") + key + Language.GetTextValue("Mods.Redemption.UI.SpiritWalker.Context");
                        }
                        RedeSystem.Instance.TitleCardUIElement.DisplayTitle(Language.GetTextValue("Mods.Redemption.UI.SpiritWalker.Name"), 300, 90, 1f, 0, Color.White, s);

                        SoundEngine.PlaySound(CustomSounds.NewLocation, player.position);
                        player.RedemptionAbility().Spiritwalker = true;
                        player.RedemptionAbility().SpiritwalkerActive = false;

                        NPC.Shoot(player.Center, ModContent.ProjectileType<SpiritwalkerIconFade>(), 0, Vector2.Zero, player.whoAmI);
                        for (int i = 0; i < 20; i++)
                        {
                            ParticleManager.NewParticle(player.Center, RedeHelper.Spread(10), new SpiritParticle(), Color.White, 2);

                            int dust2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.DungeonSpirit,
                                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                            Main.dust[dust2].velocity *= 10f;
                            Main.dust[dust2].noGravity = true;
                        }
                        NPC.active = false;
                    }
                    break;
            }
        }
        public override bool CanHitNPC(NPC target) => false;
    }
    public class SpiritwalkerIconFade : ModProjectile
    {
        public override string Texture => "Redemption/Textures/Abilities/Spiritwalker";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Walker");
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            Projectile.Center = new Vector2(player.Center.X, player.Center.Y - 180);
            Projectile.timeLeft = 10;
            Projectile.scale += 0.01f;
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.alpha -= 10;
                    if (Projectile.alpha <= 0)
                        Projectile.localAI[0] = 1;
                    break;
                case 1:
                    Projectile.alpha++;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                    break;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            float fluctuate = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f)) * 0.1f;
            float modifiedScale = Projectile.scale * (1 + fluctuate);

            Color godrayColor = Color.Lerp(new Color(180, 255, 255), Color.White * Projectile.Opacity, 0.5f);
            godrayColor.A = 0;
            RedeDraw.DrawGodrays(Main.spriteBatch, Projectile.Center - Main.screenPosition, godrayColor, 100 * modifiedScale * Projectile.Opacity, 30 * modifiedScale * Projectile.Opacity, 16);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
