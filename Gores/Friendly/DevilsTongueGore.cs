using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Gores.Friendly
{
    public class DevilsTongueGore : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.Frame = new SpriteFrame(1, 12, 0, (byte)Main.rand.Next(12));
            gore.timeLeft = 60;
        }
        public override bool Update(Gore gore)
        {
            gore.frameCounter++;
            if (gore.frameCounter >= 3)
            {
                gore.frameCounter = 0;
                gore.Frame.CurrentRow++;
                if (gore.Frame.CurrentRow > 11)
                {
                    gore.Frame.CurrentRow = 0;
                }
            }
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