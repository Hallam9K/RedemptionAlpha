using Microsoft.Xna.Framework;
using Redemption.Items.Accessories.PostML;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Mounts
{
	public class JyrinaMountBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
            player.mount.SetMount(ModContent.MountType<JyrinaMount_Chariot>(), player);
            player.buffTime[buffIndex] = 10;

            int projType = ModContent.ProjectileType<JyrinaMount_Horse>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
        }
    }
}