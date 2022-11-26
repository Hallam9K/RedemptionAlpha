using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Misc
{
    public class ToasterPet_Toast : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.timeLeft = 60;
        }
        public override bool Update(Gore gore)
        {
            gore.timeLeft = Math.Min(gore.timeLeft, 60);
            gore.timeLeft--;
            if (gore.timeLeft <= 0)
            {
                gore.alpha += 5;
                if (gore.alpha >= 255)
                    gore.active = false;
            }
            return true;
        }
    }
}