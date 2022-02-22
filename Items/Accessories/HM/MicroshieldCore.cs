using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class MicroshieldCore : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Microshield Core");
            Tooltip.SetDefault("Summons a small Shield Core that occasionally shoots lasers at enemies");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
		{
            Item.DefaultToVanitypet(ModContent.ProjectileType<MicroshieldCore_Proj>(), ModContent.BuffType<MicroshieldCoreBuff>());
            Item.width = 18;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
        }
        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
