using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Hostile;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Synthesizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Shoots a wave of notes along the ground");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 4, 50, 0);
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.LightRed;
            Item.damage = 52;
            Item.shoot = ModContent.ProjectileType<Synthesizer_Proj>();
            Item.shootSpeed = 11f;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ)
            {
                float cursorPosFromPlayer = player.Distance(Main.MouseWorld) / (Main.screenHeight / 2 / 24);
                if (cursorPosFromPlayer > 24) cursorPosFromPlayer = 1;
                else cursorPosFromPlayer = (cursorPosFromPlayer / 12) - 1;
                if (!Main.dedServ)
                {
                    SoundStyle s = CustomSounds.Synth with { Pitch = cursorPosFromPlayer };
                    SoundEngine.PlaySound(s, player.Center);
                }
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 16)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 4)
                .AddIngredient(ItemID.SoulofSight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}