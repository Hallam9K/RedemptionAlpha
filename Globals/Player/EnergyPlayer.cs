using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Redemption.Globals.Player
{
    public class EnergyPlayer : ModPlayer
    {
        public int energyMax;
        public int statEnergy;
        public int energyTimer;
        public int energyRegen;
        public override void ResetEffects()
        {
            energyMax = 0;
            energyRegen = 0;
        }
        public override void UpdateDead()
        {
            statEnergy = energyMax;
        }
        public override void PostUpdate()
        {
            if (energyTimer++ % (60 - energyRegen) == 0)
                statEnergy++;

            statEnergy = (int)MathHelper.Clamp(statEnergy, 0, energyMax);
            energyMax = (int)MathHelper.Clamp(energyMax, 0, 300);
            energyRegen = (int)MathHelper.Max(0, energyRegen);
        }
    }
}