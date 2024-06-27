using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Dusksong2 : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 66;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.width = 32;
            Item.height = 34;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<DusksongWeak_Proj>();
            Item.UseSound = SoundID.NPCDeath6;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient<GrimShard>(6)
                .AddIngredient<LostSoul>(20)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}