using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    public class EggCracker : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Stuns the panicking Fowl Emperor\n" +
                "Disappears if the Fowl Emperor isn't alive"); */
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 24;
            Item.damage = 40;
            Item.knockBack = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Default;
            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<EggCracker_Proj>();
        }
        public override void UpdateInventory(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<FowlEmperor>()))
            {
                int egg = player.FindItem(Type);
                if (egg >= 0)
                {
                    player.inventory[egg].stack = 0;
                    if (player.inventory[egg].stack <= 0)
                        player.inventory[egg] = new Item();
                }
            }
        }
        private float glowRot = 0;
        public override void PostUpdate()
        {
            glowRot += 0.03f;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.BeginAdditive();

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), Color.IndianRed, glowRot, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), Color.IndianRed, -glowRot, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.BeginDefault();
            return true;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            glowRot += 0.03f;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Vector2 origin2 = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            spriteBatch.Draw(glow, position, new Rectangle(0, 0, glow.Width, glow.Height), Color.IndianRed, glowRot, origin2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, position, new Rectangle(0, 0, glow.Width, glow.Height), Color.IndianRed, -glowRot, origin2, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            return true;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<FowlEmperor>()))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Item.active = false;
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
                }
            }
        }
    }
}
