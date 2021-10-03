using Microsoft.Xna.Framework;
using Redemption.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class RootTendril : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 0, 45, 0);
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.knockBack = 4f;
            Item.damage = 14;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<RootTendril_Proj>();
            Item.shootSpeed = 4;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item152;
            Item.autoReuse = false;
        }
    }
    public class RootTendril_Proj : WhipProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Root Tendril");
        }
        public override void WhipDefaults()
        {
            originalColor = Color.White;
            whipRangeMultiplier = 1f;
            fallOff = 0.3f;
        }
    }
}