using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class EaglecrestGlove : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ThunderS);
        public override void SetStaticDefaults()
        {
            ElementID.ItemEarth[Type] = true;
            ElementID.ItemThunder[Type] = true;
            ElementID.ItemArcane[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 28;
            Item.height = 34;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 0, 75);
            Item.UseSound = SoundID.Item88;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType<EaglecrestBoulder_Proj>();
            Item.shootSpeed = 5f;
            Item.rare = ItemRarityID.Orange;

            Item.Redemption().HideElementTooltip[ElementID.Thunder] = true;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Flat += player.velocity.Length() * 4;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position.Y -= 20;
            if ((player.velocity.Y >= 0 || velocity.Y < 0) && (player.velocity.Y < 0 || velocity.Y >= 0))
                velocity.Y += player.velocity.Y;

            if ((player.velocity.X >= 0 || velocity.X < 0) && (player.velocity.X < 0 || velocity.X >= 0))
                velocity.X += player.velocity.X;
        }
    }
}