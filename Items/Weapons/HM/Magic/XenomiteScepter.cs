using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class XenomiteScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Helix Scepter");
            /* Tooltip.SetDefault("Casts infectious helix bolts\n" +
                "Every consecutive shot increases the velocity of the bolts"); */
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 4, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item117;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<XenomiteScepter_Proj>();
            Item.shootSpeed = 5f;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        private int CastCount;
        public override void HoldItem(Player player)
        {
            if (!player.channel)
                CastCount = 0;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 70f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }

            if (CastCount++ > 0)
                velocity *= (CastCount / 20f) + 1;
            CastCount = (int)MathHelper.Min(CastCount, 20);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int m = 0; m < 2; m++)
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<XenomiteScepter_Proj>(), damage, knockback, player.whoAmI, m);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ContagionSpreader>())
                .AddIngredient(ModContent.ItemType<Xenomite>(), 6)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 7)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}