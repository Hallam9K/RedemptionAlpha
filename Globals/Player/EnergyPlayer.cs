using Redemption.Biomes;
using Redemption.Items.Donator.Lizzy;
using Redemption.Items.Donator.Uncon;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Redemption.Globals.Player
{
    public class EnergyPlayer : ModPlayer
    {
        public int energyMax = 100;
        public int statEnergy = 100;
        public int energyTimer;
        public override void ResetEffects()
        {
            energyMax = 100;
        }
        public override void UpdateDead()
        {
            statEnergy = energyMax;
        }
        public override void PostUpdate()
        {
            if (energyTimer++ % 30 == 0)
                statEnergy++;

            statEnergy = (int)MathHelper.Clamp(statEnergy, 0, energyMax);
        }
    }
}