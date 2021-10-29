using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Donator.Lantard
{
    public class LantardPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fluffy Boi");
			Description.SetDefault("Fluff.");
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<BuffPlayer>().lantardPet = true;
			bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<LantardPatreon_Pet>()] <= 0;
			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.position.X + player.width / 2, player.position.Y + player.height / 2, 0f, 0f, ModContent.ProjectileType<LantardPatreon_Pet>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}