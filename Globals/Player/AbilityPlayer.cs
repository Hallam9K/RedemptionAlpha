using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameInput;
using Terraria.Audio;
using Terraria.DataStructures;
using System;

namespace Redemption.Globals.Player
{
    public class AbilityPlayer : ModPlayer
    {
        public bool Spiritwalker;
        public bool SpiritwalkerActive;
        public int SpiritwalkerTimer;
        public int SpiritwalkerCooldown;
        public override void Initialize()
        {
            Spiritwalker = false;
        }
        public override void SaveData(TagCompound tag)
        {
            var abilityS = new List<string>();
            if (Spiritwalker) abilityS.Add("Spiritwalker");

            tag["abilityS"] = abilityS;
        }
        public override void LoadData(TagCompound tag)
        {
            var abilityS = tag.GetList<string>("abilityS");
            Spiritwalker = abilityS.Contains("Spiritwalker");
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Spiritwalker && SpiritwalkerCooldown <= 0 && Redemption.RedeSpiritwalkerAbility.Current && Player.active && !Player.dead)
            {
                if (SpiritwalkerTimer++ >= 60)
                {
                    SoundEngine.PlaySound(CustomSounds.PortalWub with { Volume = 2f, Pitch = -0.5f }, Player.position);
                    SpiritwalkerActive = !SpiritwalkerActive;
                    SpiritwalkerCooldown = 60;
                    SpiritwalkerTimer = 0;
                }
            }
            else
                SpiritwalkerTimer--;

            SpiritwalkerCooldown--;
            SpiritwalkerCooldown = (int)MathHelper.Clamp(SpiritwalkerCooldown, 0, 60);
            SpiritwalkerTimer = (int)MathHelper.Clamp(SpiritwalkerTimer, 0, 60);
        }
        public override void PreUpdate()
        {
            Player.ManageSpecialBiomeVisuals("MoR:SpiritSky", false, Player.Center);
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (SpiritwalkerTimer > 0)
            {
                if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Vector2 vector;
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        vector.X = (float)(Math.Sin(angle) * (130 - (SpiritwalkerTimer * 2)));
                        vector.Y = (float)(Math.Cos(angle) * (130 - (SpiritwalkerTimer * 2)));
                        Dust dust2 = Main.dust[Dust.NewDust(drawInfo.Center + vector, 2, 2, DustID.DungeonSpirit, Scale: 2)];
                        dust2.noGravity = true;
                        dust2.noLight = true;
                        Color dustColor = new(180, 255, 255) { A = 0 };
                        dust2.color = dustColor;
                        dust2.velocity = dust2.position.DirectionTo(drawInfo.Center) * 8f;
                    }
                }
            }
        }
        public override void PostUpdateBuffs()
        {
            if (SpiritwalkerActive)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(20))
                    ParticleManager.NewParticle(Player.position + RedeHelper.Spread(1100), RedeHelper.SpreadUp(2), new SpiritParticle(), Color.White, 3);

                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.5f).UseIntensity(1f)
                    .UseColor(new Color(180, 255, 255)).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/SwirlyPerly", AssetRequestMode.ImmediateLoad).Value);
                Player.ManageSpecialBiomeVisuals("MoR:FogOverlay", SpiritwalkerActive);
                Player.ManageSpecialBiomeVisuals("MoR:SpiritSky", SpiritwalkerActive, Player.Center);
            }
        }
    }
}