using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Projectiles.Pets;
using Redemption.Buffs.Pets;

namespace Redemption.Items.Accessories.HM
{
    public class SlayerProjector : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("Summons a hologram of King Slayer to judge you");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToVanitypet(ModContent.ProjectileType<KS3Pet_Proj>(), ModContent.BuffType<KS3PetBuff>());
			Item.width = 28;
			Item.height = 18;
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