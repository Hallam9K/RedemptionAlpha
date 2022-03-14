using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Melee;
using Terraria.GameContent.Creative;
using Redemption.Globals;
using Redemption.Base;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class Firebreak : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Firebreak");
            Tooltip.SetDefault("Rains down fire from the skies when hitting a target");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Orange;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawn = new(target.Center.X + Main.rand.Next(-300, 301), target.Center.Y - Main.rand.Next(800, 861));

                Projectile.NewProjectile(player.GetProjectileSource_Item(Item), spawn,
                    RedeHelper.PolarVector(30, (target.Center + target.velocity * 20f - spawn).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f)),
                    ModContent.ProjectileType<Firebreak_Proj>(), 13, knockBack, Main.myPlayer);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 4)
                .AddIngredient(ModContent.ItemType<GrimShard>(), 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
