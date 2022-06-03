using Redemption.Biomes;
using Redemption.Items.Donator.Lizzy;
using Redemption.Items.Donator.Uncon;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.Lab;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameInput;
using Terraria.Audio;

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