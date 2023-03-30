using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class SoulScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Casts controllable soul-charges that orbit the cursor" +
                "\nMore soul-charges are cast the longer you hold" +
                "\n'Her soul was not my target...'"); */
            Item.staff[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 38;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.value = 2000;
            Item.channel = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item9 with { Volume = 0 };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SoulScepterChargeS>();
            Item.shootSpeed = 10f;
        }
        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, scale, scale * 0.8f, scale);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(46, 32, 54), new Color(111, 83, 188), new Color(46, 32, 54));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(-32, 34, glow.Width, glow.Height), color * 0.4f, rotation, origin, scale * 0.3f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;

        }
    }
}