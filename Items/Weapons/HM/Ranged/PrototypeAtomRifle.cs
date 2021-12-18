using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class PrototypeAtomRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Replaces normal bullets with plasma rounds");
        }

        public override void SetDefaults()
        {
            Item.damage = 108;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 30;
            Item.useTime = 68;
            Item.useAnimation = 68;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item122;

            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.GetGlobalItem<ItemUseGlow>().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 40f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PlasmaRound>(), damage, knockback, player.whoAmI);
            Main.projectile[proj].hostile = false;
            Main.projectile[proj].friendly = true;
            Main.projectile[proj].DamageType = DamageClass.Ranged;
            Main.projectile[proj].tileCollide = true;
            Main.projectile[proj].netUpdate2 = true;
            return false;
        }
    }
}