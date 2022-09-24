using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Items.Usable;
using Terraria;
using Terraria.Audio;
using Redemption.Buffs;

namespace Redemption.Globals.Player
{
    public class EnergyPlayer : ModPlayer
    {
        public int energyMax;
        public int statEnergy;
        public int energyTimer;
        public int energyRegen;
        public bool stopEnergyRegen;
        public override void ResetEffects()
        {
            energyMax = 0;
            energyRegen = 0;
            stopEnergyRegen = false;
        }
        public override void UpdateDead()
        {
            statEnergy = energyMax;
        }
        public override void PostUpdate()
        {
            if (statEnergy <= 15 && energyMax > 0)
            {
                int cell = Player.FindItem(ModContent.ItemType<EnergyCell>());
                if (cell >= 0)
                {
                    SoundEngine.PlaySound(CustomSounds.Spark1 with { Pitch = 0.5f }, Player.position);
                    Player.GetModPlayer<EnergyPlayer>().statEnergy += 20;
                    Player.inventory[cell].stack--;
                    if (Player.inventory[cell].stack <= 0)
                        Player.inventory[cell] = new Item();
                }
            }
            energyRegen = (int)MathHelper.Min(59, energyRegen);
            if (!stopEnergyRegen && energyTimer++ % (60 - energyRegen) == 0)
            {
                if (Player.HasBuff<EnergyStationBuff>())
                    statEnergy += 3;
                else
                    statEnergy++;
            }

            statEnergy = (int)MathHelper.Clamp(statEnergy, 0, energyMax);
            energyMax = (int)MathHelper.Clamp(energyMax, 0, 300);
        }
    }
}