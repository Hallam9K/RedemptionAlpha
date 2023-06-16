using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Redemption.Rarities;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class AncientSigil : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Ancient Sigil");
            /* Tooltip.SetDefault("Reawakens an ancient power..."
                + "\nOnly usable at day"
                + "\nNot consumable"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
		{
			Item.width = 26;
            Item.height = 42;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.consumable = false;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }
        public override bool CanUseItem(Player player)
        {
            return !SubworldSystem.IsActive<SoullessSub>() && Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<EaglecrestGolem2>()) && !NPC.AnyNPCs(ModContent.NPCType<EaglecrestGolem>());
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<EaglecrestGolem2>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }
            return true;
        }
        private float glowRot = 0;
        public override void PostUpdate()
        {
            glowRot += 0.03f;

            if (!Main.rand.NextBool(30))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.TreasureSparkle, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(241, 215, 108), new Color(255, 255, 255), new Color(241, 215, 108));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), color, glowRot, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), color, -glowRot, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}