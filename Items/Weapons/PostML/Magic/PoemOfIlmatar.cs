using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class PoemOfIlmatar : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Poem of Ilmatar");
            // Tooltip.SetDefault("Hold left-click to create a tornado that picks up enemies and juggles them");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 140;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.knockBack = 12;
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<PoemTornado_Proj>();
            Item.UseSound = CustomSounds.WindLong;
        }
        public override bool CanUseItem(Player player)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;

            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }
    }
}