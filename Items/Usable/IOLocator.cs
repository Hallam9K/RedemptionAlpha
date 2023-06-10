using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class IOLocator : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("IO-Locator");
            // Tooltip.SetDefault("Holding this in your hand will point to the Abandoned Laboratory");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 15, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.width = 34;
            Item.height = 26;
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<LabPointer>()] < 1)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<LabPointer>(), 0, 0, player.whoAmI);
        }
    }
    public class LabPointer : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pointer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 LabPos = new((RedeGen.LabVector.X + 104) * 16, (RedeGen.LabVector.Y + 14) * 16);
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<IOLocator>())
                Projectile.timeLeft = 10;

            Projectile.Center = player.Center;
            Projectile.rotation = (LabPos - player.Center).ToRotation();
            Projectile.direction = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.1f, 0.3f, 0.1f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.05f, 0.15f, 0.05f);
            float alpha = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.5f, 1f, 0.5f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * alpha, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale + scale2, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale + scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
