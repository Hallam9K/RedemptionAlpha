using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Items.Weapons.PreHM.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Divinity : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.height = 30;
            Item.width = 36;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.reuseDelay = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.channel = true;
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 15, 50, 0);
            Item.UseSound = SoundID.Item123;
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<Divinity_Sun>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<HolyBible>())
                .AddIngredient(ItemID.BeetleHusk, 2)
                .AddIngredient(ItemID.LunarTabletFragment, 5)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.Bookcases)
                .DisableDecraft()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient(ItemID.BeetleHusk, 4)
                .AddIngredient(ItemID.LunarTabletFragment, 10)
                .AddIngredient(ItemID.SoulofLight, 15)
                .AddIngredient(ItemID.SoulofSight, 15)
                .AddTile(TileID.Bookcases)
                .DisableDecraft()
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<Divinity_Proj>();
        }
    }
}
