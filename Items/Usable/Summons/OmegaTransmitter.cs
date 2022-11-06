using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.NPCs.Bosses.Obliterator;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class OmegaTransmitter : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Transmitter");
            Tooltip.SetDefault("Summons an Omega Prototype"
                + "\nRight-click to switch which prototype to summon"
                + "\nOnly usable at night" +
                "\nOnly usable after Plantera has been defeated"
                + "\nNot consumable");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
		{
			Item.width = 18;
            Item.height = 40;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.consumable = false;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.UseSound = CustomSounds.ShootChange;
            else
                Item.UseSound = SoundID.Item44;

            return player.altFunctionUse == 2 || (!Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()) && !NPC.AnyNPCs(ModContent.NPCType<Wielder>()) && !NPC.AnyNPCs(ModContent.NPCType<Gigapora>()) && !NPC.AnyNPCs(ModContent.NPCType<Porakone>()) && !NPC.AnyNPCs(ModContent.NPCType<OO>()));
		}
        private int Choice;
        public override bool? UseItem(Player player)
        {
            int limit = -1;
            if (NPC.downedPlantBoss)
                limit = 0;
            if (NPC.downedGolemBoss)
                limit = 1;
            if (NPC.downedMoonlord)
                limit = 2;
            if (RedeBossDowned.downedNebuleus)
                limit = 3;

            if (player.altFunctionUse == 2)
            {
                Choice++;
                if (Choice >= limit + 1)
                    Choice = 0;

                switch (Choice)
                {
                    case 0:
                        if (limit == -1)
                            CombatText.NewText(player.getRect(), Color.Red, "Nothing happens...", true, false);
                        else if (limit == 0)
                            CombatText.NewText(player.getRect(), Color.Red, "No other Prototypes available...", true, false);
                        else
                            CombatText.NewText(player.getRect(), Color.Red, "1st Omega Prototype", true, false);
                        break;
                    case 1:
                        CombatText.NewText(player.getRect(), Color.Red, "2nd Omega Prototype", true, false);
                        break;
                    case 2:
                        CombatText.NewText(player.getRect(), Color.Red, "3rd Omega Prototype", true, false);
                        break;
                    case 3:
                        CombatText.NewText(player.getRect(), Color.Red, "4th Omega Prototype", true, false);
                        break;
                }
            }
            else
            {
                switch (Choice)
                {
                    case 0:
                        if (limit == -1)
                            CombatText.NewText(player.getRect(), Color.Red, "Nothing happens...", true, false);
                        else
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                SoundEngine.PlaySound(SoundID.Roar, player.position);

                                int type2 = ModContent.NPCType<Wielder>();

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y + 500, type2);
                                else
                                    NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type2);
                            }
                        }
                        break;
                    case 1:
                        if (player.whoAmI == Main.myPlayer)
                        {
                            SoundEngine.PlaySound(SoundID.Roar, player.position);

                            int type2 = ModContent.NPCType<Porakone>();

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y - 500, type2);
                            else
                                NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type2);
                        }
                        break;
                    case 2:
                        SoundEngine.PlaySound(SoundID.Roar, player.position);

                        int type = ModContent.NPCType<OO>();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 350, (int)player.position.Y - 800, type);
                        else
                            NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
                        break;
                }
            }
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string omegaType = "";
            switch (Choice)
            {
                case 0:
                    if (!NPC.downedPlantBoss)
                        omegaType = "No Omega Prototypes can be called right now";
                    else
                        omegaType = "1st Omega Prototype\n" +
                            "'The corrupted blade draws near the power, thus beginning the final hour'";
                    break;
                case 1:
                    omegaType = "2nd Omega Prototype\n" +
                        "'Mechanical whirls beneath the ground, be wary of the deadly sound'";
                    break;
                case 2:
                    omegaType = "3rd Omega Prototype\n" +
                        "'Feel the sense of frustration, prepare for obliteration'\n" +
                        "Only usable after the first 2 Omega Prototypes have been defeated";
                    break;
                case 3:
                    omegaType = "4th Omega Prototype\n" +
                        "'As the noise dies down, the end draws near; ******* **** **** ******, *** ****** ** ****'\n" +
                        "Only usable after Nebuleus has been defeated";
                    break;
            }
            TooltipLine line = new(Mod, "OmegaName", omegaType)
            {
                OverrideColor = Color.Red,
            };
            tooltips.Add(line);
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            bool glowing = false;
            if ((NPC.downedPlantBoss && !RedeBossDowned.downedOmega1) || (NPC.downedGolemBoss && !RedeBossDowned.downedOmega2) || (NPC.downedMoonlord && !RedeBossDowned.downedOmega3))
                glowing = true;
            if (glowing)
            {
                Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
                BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, scale, scale * 0.8f, scale);
                Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.Red, Color.DarkRed, Color.Red);
                Vector2 origin2 = new(glow.Width / 2, glow.Height / 2);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                spriteBatch.Draw(glow, position + new Vector2(6, 14), new Rectangle(0, 0, glow.Width, glow.Height), color * 0.7f, 0, origin2, scale * 0.8f, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            }
            return true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            bool glowing = false;
            if ((NPC.downedPlantBoss && !RedeBossDowned.downedOmega1) || (NPC.downedGolemBoss && !RedeBossDowned.downedOmega2) || (NPC.downedMoonlord && !RedeBossDowned.downedOmega3))
                glowing = true;
            if (glowing)
            {
                Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
                BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, scale, scale * 0.8f, scale);
                Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.Red, Color.DarkRed, Color.Red);
                Vector2 origin = new(glow.Width / 2, glow.Height / 2);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), color * 0.7f, rotation, origin, scale * 0.8f, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return true;
        }
    }
}