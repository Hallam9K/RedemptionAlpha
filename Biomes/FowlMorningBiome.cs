using Redemption.Globals.World;
using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class FowlMorningBiome : ModBiome
    {
        public override string BestiaryIcon => "Redemption/Textures/Bestiary/FowlMorningIcon";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fowl Morning");
        }
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/FowlMorning");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("MoR:FowlMorningSky", isActive);
        }
        public override bool IsBiomeActive(Player player) => FowlMorningWorld.FowlMorningActive && player.ZoneOverworldHeight;
    }
}