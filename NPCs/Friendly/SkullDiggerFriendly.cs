using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Redemption.Globals;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using Redemption.Items.Usable.Summons;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Items.Materials.PreHM;
using Terraria.Audio;
using Terraria.Localization;
using Redemption.Items.Usable;

namespace Redemption.NPCs.Friendly
{
    public class SkullDiggerFriendly : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger";
        public enum ActionState
        {
            Idle,
            Saved
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skull Digger");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 92;
            NPC.friendly = true;
            NPC.lifeMax = 2400;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override bool CheckActive() => !spoken;
        private bool floatTimer;
        private bool spoken;
        public override void AI()
        {
            if (spoken)
                NPC.DiscourageDespawn(60);
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<SkullDiggerFriendly_FlailBlade>()))
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<SkullDiggerFriendly_FlailBlade>(), NPC.damage, Vector2.Zero, NPC.whoAmI);

            if (!floatTimer)
            {
                NPC.velocity.Y += 0.03f;
                if (NPC.velocity.Y > .5f)
                    floatTimer = true;
            }
            else if (floatTimer)
            {
                NPC.velocity.Y -= 0.03f;
                if (NPC.velocity.Y < -.5f)
                    floatTimer = false;
            }

            switch (AIState)
            {
                case ActionState.Saved:
                    if (Main.LocalPlayer.talkNPC == -1 && AITimer == 0)
                        AITimer = 1;

                    if (AITimer > 0)
                    {
                        AITimer++;
                        NPC.alpha++;
                        if (Main.rand.NextBool(5))
                            Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.PurificationPowder);

                        if (AITimer == 60)
                            CombatText.NewText(NPC.getRect(), Color.GhostWhite, Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.1"), true, false);
                        if (AITimer == 120)
                            CombatText.NewText(NPC.getRect(), Color.GhostWhite, Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.2"), true, false);
                        if (NPC.alpha >= 255)
                        {
                            for (int i = 0; i < 50; i++)
                            {
                                int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.PurificationPowder, 0f, 0f, 100, default, 2.5f);
                                Main.dust[dustIndex].velocity *= 2.6f;
                            }
                            Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.SkullDigger"), Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);
                            if (!RedeBossDowned.skullDiggerSaved)
                            {
                                RedeWorld.alignment++;
                                for (int p = 0; p < Main.maxPlayers; p++)
                                {
                                    Player player2 = Main.player[p];
                                    if (!player2.active)
                                        continue;

                                    CombatText.NewText(player2.getRect(), Color.Gold, "+1", true, false);
                                }
                            }
                            NPC.netUpdate = true;
                            NPC.SetEventFlagCleared(ref RedeBossDowned.skullDiggerSaved, -1);

                            NPC.active = false;
                        }
                    }
                    break;
            }
        }

        public override bool CanChat() => true;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("Mods.Redemption.DialogueBox.SkullDigger.Talk");
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<WeddingRing>()))
                button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.SkullDigger.Give");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.npcChatText = ChitChat();
            }
            else
            {
                int ring = Main.LocalPlayer.FindItem(ModContent.ItemType<WeddingRing>());
                if (ring >= 0)
                {
                    Main.LocalPlayer.inventory[ring].stack--;
                    if (Main.LocalPlayer.inventory[ring].stack <= 0)
                        Main.LocalPlayer.inventory[ring] = new Item();
                }

                if ((BasePlayer.HasHelmet(Main.LocalPlayer, ItemID.TheBrideHat) && BasePlayer.HasChestplate(Main.LocalPlayer, ItemID.TheBrideDress)) || (BasePlayer.HasHelmet(Main.LocalPlayer, ItemID.TopHat) && BasePlayer.HasChestplate(Main.LocalPlayer, ItemID.TuxedoShirt) && BasePlayer.HasLeggings(Main.LocalPlayer, ItemID.TuxedoPants)))
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.WeddingRingDialogue2");
                else
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.WeddingRingDialogue1");
                AIState = ActionState.Saved;
            }
        }
        public static string ChitChat()
        {
            return Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.Talk");
        }
        public override string GetChat()
        {
            spoken = true;
            return Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.Dialogue");
        }

        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D HandsTex = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Hands").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.3f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Rectangle rect = new(0, 0, HandsTex.Width, HandsTex.Height);
            spriteBatch.Draw(HandsTex, NPC.Center - screenPos - new Vector2(14, -32), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (RedeBossDowned.skullDiggerSaved || !RedeBossDowned.keeperSaved || NPC.AnyNPCs(NPC.type))
                return 0;
            bool sorrow = spawnInfo.Player.HasItem(ModContent.ItemType<SorrowfulEssence>());
            return SpawnCondition.Cavern.Chance * (sorrow ? 0.08f : 0.03f);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
    }
    public class SkullDiggerFriendly_Spirit : SkullDiggerFriendly
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Skull Digger's Spirit");
        }
        public override bool CheckActive() => false;
        private bool floatTimer;
        public override bool PreAI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();
            if (!Main.player[Main.myPlayer].RedemptionAbility().SpiritwalkerActive)
            {
                NPC.alpha += 10;
                if (NPC.alpha >= 255)
                {
                    bool oneActive = false;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player2 = Main.player[i];
                        if (!player2.active || player2.dead || !player.RedemptionAbility().SpiritwalkerActive)
                            continue;

                        oneActive = true;
                    }
                    if (!oneActive)
                        NPC.active = false;
                }
                return false;
            }
            else
            {
                if (NPC.ai[0] >= 2 && NPC.alpha > 0)
                    NPC.alpha -= 10;
            }

            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<SkullDiggerFriendly_FlailBlade>()))
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<SkullDiggerFriendly_FlailBlade>(), NPC.damage, Vector2.Zero, NPC.whoAmI, 1);

            if (!floatTimer)
            {
                NPC.velocity.Y += 0.03f;
                if (NPC.velocity.Y > .5f)
                    floatTimer = true;
            }
            else if (floatTimer)
            {
                NPC.velocity.Y -= 0.03f;
                if (NPC.velocity.Y < -.5f)
                    floatTimer = false;
            }
            NPC.alpha += Main.rand.Next(-10, 11);
            NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
            return false;
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("Mods.Redemption.DialogueBox.SkullDigger.Talk");
            if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive && !Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardSkullDigger>()))
                button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.SkullDigger.Crux");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.SpiritTalk");
            else
            {
                int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                if (card >= 0)
                {
                    Main.LocalPlayer.inventory[card].stack--;
                    if (Main.LocalPlayer.inventory[card].stack <= 0)
                        Main.LocalPlayer.inventory[card] = new Item();

                    Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardSkullDigger>());
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.CruxSpiritDialogue");
                    Main.npcChatCornerItem = ModContent.ItemType<CruxCardSkullDigger>();
                    SoundEngine.PlaySound(SoundID.Chat);
                }
                else
                {
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.NoCruxSpiritDialogue");
                    Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                }
            }
        }
        public override string GetChat()
        {
            return Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.SpiritDialogue");
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D HandsTex = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Hands").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.3f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Rectangle rect = new(0, 0, HandsTex.Width, HandsTex.Height);
            spriteBatch.Draw(HandsTex, NPC.Center - screenPos - new Vector2(14, -32), new Rectangle?(rect), NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!RedeBossDowned.skullDiggerSaved || NPC.AnyNPCs(NPC.type))
                return 0;

            if (spawnInfo.Player.RedemptionAbility().SpiritwalkerActive && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                bool sorrow = spawnInfo.Player.HasItem(ModContent.ItemType<SorrowfulEssence>());
                return sorrow ? 0.8f : 0.4f;
            }
            return 0;
        }
    }
}
