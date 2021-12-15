using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class DragonSlayersStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Slayer's Staff");
            Tooltip.SetDefault("Casts a molten dragon skull to spews out flames at cursor point" +
                "\nHold down left click long enough to change the flames into a heat ray");
            Item.staff[Item.type] = true;
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.reuseDelay = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.channel = true;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<DragonSkull_Proj>();
            if (!Main.dedServ)
                Item.GetGlobalItem<ItemUseGlow>().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }

        public override bool CanUseItem(Player player)
        {
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if (tile.IsActiveUnactuated && Main.tileSolid[tile.type] && !Main.tileCut[tile.type])
                return false;

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 10)
            .AddIngredient(ItemID.Bone, 5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}