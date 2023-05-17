using Microsoft.Xna.Framework;
using Redemption.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Pets
{
    public class BasanPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<BasanPet_Proj>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }
}
