using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Dusksong : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dusksong, Bond of Twilight");
            // Tooltip.SetDefault("Casts a large dusksong that splits upon hitting enemies");

            Item.ResearchUnlockCount = 1;
        }

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
            Item.shoot = ModContent.ProjectileType<Dusksong_Proj>();
            Item.UseSound = SoundID.NPCDeath6;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient<GrimShard>(6)
                .AddIngredient<LostSoul>(20)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}