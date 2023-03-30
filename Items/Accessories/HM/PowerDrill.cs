using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Projectiles.Pets;
using Redemption.Buffs.Pets;

namespace Redemption.Items.Accessories.HM
{
    public class PowerDrill : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("Summons a Mini Gigapora");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToVanitypet(ModContent.ProjectileType<MiniGigapora>(), ModContent.BuffType<MiniGigaporaPetBuff>());
			Item.width = 40;
			Item.height = 42;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.sellPrice(0, 5);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);
			return false;
		}
	}
}