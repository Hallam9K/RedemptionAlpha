using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BloodLetter : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Right-click to swap between a normal slash and blood letting\n" +
                "Holding left-click in blood letting will drain the player's Spirit Gauge in exchange for increased life regeneration"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 22;
            Item.knockBack = 3;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<BloodLetter_Slash>();
        }
        private bool specialMode;
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 5;
                player.itemTime = 5;
                player.itemAnimation = 5;

                specialMode = !specialMode;
                return false;
            }
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (specialMode && (player.GetModPlayer<RitualistPlayer>().SpiritLevel > 0 || player.GetModPlayer<RitualistPlayer>().SpiritGauge > 0))
            {
                type = ModContent.ProjectileType<BloodLetter_Proj>();
            }
            else
            {
                type = Item.shoot;
                velocity += velocity.RotatedByRandom(0.8f);
                Vector2 Offset = Vector2.Normalize(velocity) * 50f;
                position += Offset;
            }
        }
        public override bool CanUseItem(Player player)
        {
            if (specialMode && player.altFunctionUse != 2)
                return player.GetModPlayer<RitualistPlayer>().SpiritLevel > 0 || player.GetModPlayer<RitualistPlayer>().SpiritGauge > 0;

            return true;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            SpriteEffects effects = SpriteEffects.None;
            if (specialMode)
                effects = SpriteEffects.FlipVertically;

            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), drawColor, 0, origin, scale, effects, 0f);

            return false;
        }
        /*
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }*/
    }
}
