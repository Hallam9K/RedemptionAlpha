using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals.Player;
using ReLogic.Content;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.NPCs.Bosses.Neb;
using Terraria.Graphics.Effects;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Bosses.Obliterator;

namespace Redemption.Globals
{
    public class SilenceEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/silence");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return RedeSystem.Silence || player.RedemptionRad().irradiatedEffect == 2;
        }
    }
    public class SkeletonInvasionMusic : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Spooky");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return RedeWorld.SkeletonInvasion && player.ZoneOverworldHeight;
        }
    }
    public class ChaliceIntroMusic : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/HallofHeroes");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<Chalice_Intro>());
        }
    }
    public class NukeMusic : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Warhead");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return RedeWorld.nukeCountdownActive;
        }
    }
    public class Rad1Music : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Rad1");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.RedemptionRad().irradiatedEffect == 3;
        }
    }
    public class Rad2Music : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Rad2");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.RedemptionRad().irradiatedEffect >= 4;
        }
    }
    public class DancingSkeletonEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Island");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Terraria.Player player, bool isActive)
        {
            if (isActive)
            {
                Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1.2f).UseIntensity(1f)
                    .UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
                player.ManageSpecialBiomeVisuals("MoR:FogOverlay", isActive);
            }
            player.ManageSpecialBiomeVisuals("MoR:IslandEffect", isActive);
        }
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.GetModPlayer<BuffPlayer>().island;
        }
    }
    public class SpiritRealmMusic : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/SpiritRealm");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.RedemptionAbility().SpiritwalkerActive;
        }
    }
    public class NebSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Terraria.Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("MoR:NebP1", isActive);
            if (isActive)
                SkyManager.Instance.Activate("MoR:NebP1");
            else
                SkyManager.Instance.Deactivate("MoR:NebP1");
        }
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<Nebuleus>()) || Terraria.NPC.AnyNPCs(ModContent.NPCType<Nebuleus_Clone>());
        }
    }
    public class OOSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Terraria.Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("MoR:OOSky", isActive);
            if (isActive)
                SkyManager.Instance.Activate("MoR:OOSky");
            else
                SkyManager.Instance.Deactivate("MoR:OOSky");
        }
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<OO>()) && Redemption.grooveTimer >= 824;
        }
    }
    public class NebSky2Scene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Terraria.Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("MoR:NebP2", isActive);
            if (isActive)
                SkyManager.Instance.Activate("MoR:NebP2");
            else
                SkyManager.Instance.Deactivate("MoR:NebP2");
        }
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<Nebuleus2>()) || Terraria.NPC.AnyNPCs(ModContent.NPCType<Nebuleus2_Clone>());
        }
    }
    public class UkkoSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Terraria.Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("MoR:Ukko", isActive);
            if (isActive)
                SkyManager.Instance.Activate("MoR:Ukko");
            else
                SkyManager.Instance.Deactivate("MoR:Ukko");
        }
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return Terraria.NPC.AnyNPCs(ModContent.NPCType<Ukko>());
        }
    }
}
