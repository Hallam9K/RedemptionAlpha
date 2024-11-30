using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.UI.ChatUI;
using Redemption.UI.Dialect;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.UI;

namespace Redemption.NPCs.Friendly
{
    public class SkullDiggerFriendly : ModRedeNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger";

        private static Asset<Texture2D> HeadIcon;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            HeadIcon = Request<Texture2D>(Texture + "_Head_Boss");
        }

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

            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
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
            NPC.rarity = 1;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;

            DialogueBoxStyle = CAVERN;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override bool CheckActive() => !spoken;
        private bool spoken;
        public override void AI()
        {
            if (spoken)
                NPC.DiscourageDespawn(60);
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            if (!RedeHelper.AnyProjectiles(ProjectileType<SkullDiggerFriendly_FlailBlade>()))
                NPC.Shoot(NPC.Center, ProjectileType<SkullDiggerFriendly_FlailBlade>(), NPC.damage, Vector2.Zero, NPC.whoAmI);

            NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 15) / 3;

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
                            Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemType<SkullDiggerTrophy>());

                            for (int i = 0; i < 50; i++)
                            {
                                int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.PurificationPowder, 0f, 0f, 100, default, 2.5f);
                                Main.dust[dustIndex].velocity *= 2.6f;
                            }
                            Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Other.SkullDigger"), Colors.RarityPurple.R, Colors.RarityPurple.G, Colors.RarityPurple.B);

                            if (!RedeBossDowned.skullDiggerSaved)
                                RedeWorld.Alignment++;

                            NPC.netUpdate = true;
                            NPC.SetEventFlagCleared(ref RedeBossDowned.skullDiggerSaved, -1);

                            NPC.active = false;
                        }
                    }
                    break;
            }
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;
        }

        public override bool CanChat() => true;
        public override string GetChat()
        {
            spoken = true;
            return Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.Dialogue");
        }

        public override void FindFrame(int frameHeight)
        {
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
            Texture2D HandsTex = Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Hands").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            spriteBatch.End();
            spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.3f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
            }

            spriteBatch.End();
            spriteBatch.BeginDefault();

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Rectangle rect = new(0, 0, HandsTex.Width, HandsTex.Height);
            spriteBatch.Draw(HandsTex, NPC.Center - screenPos - new Vector2(14, -32), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (RedeBossDowned.skullDiggerSaved || !RedeBossDowned.keeperSaved || NPC.AnyNPCs(NPC.type))
                return 0;
            bool sorrow = spawnInfo.Player.HasItem(ItemType<SorrowfulEssence>());
            return SpawnCondition.Cavern.Chance * (sorrow ? 0.08f : 0.03f);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public sealed class SkullDiggerMapIconLayer : ModMapLayer
        {
            public override void Draw(ref MapOverlayDrawContext context, ref string text)
            {
                const float scale = 1f;

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.ModNPC is not SkullDiggerFriendly skullDiggerNPC)
                        continue;

                    Vector2 position = skullDiggerNPC.NPC.Center.ToTileCoordinates().ToVector2();
                    if (context.Draw(HeadIcon.Value, position, Color.White, new SpriteFrame(1, 1, 0, 0), scale, scale, Alignment.Center).IsMouseOver)
                        text = npc.TypeName;
                }
            }
        }
    }
    public class TalkButton_SkullDigger : ChatButton
    {
        public override double Priority => 3.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Talk");
        public override string Description(NPC npc, Player player) => string.Empty;
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SkullDiggerFriendly>() || npc.type == NPCType<SkullDiggerFriendly_Spirit>();
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            if (npc.type == NPCType<SkullDiggerFriendly_Spirit>())
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.SpiritTalk");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.Talk");
        }
    }
    public class GiveWeddingRingButton : ChatButton
    {
        public override double Priority => 4.0;
        public override string Text(NPC npc, Player player) => !Main.LocalPlayer.HasItem(ItemType<WeddingRing>()) ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SkullDigger.Give");
        public override string Description(NPC npc, Player player) => string.Empty;
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SkullDiggerFriendly>() && npc.ai[0] != 1;
        public override Color? OverrideColor(NPC npc, Player player)
        {
            if (!Main.LocalPlayer.HasItem(ItemType<WeddingRing>()))
                return Color.Gray;
            return RedeColor.TextPositive;
        }

        public override void OnClick(NPC npc, Player player)
        {
            if (!Main.LocalPlayer.HasItem(ItemType<WeddingRing>()))
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            int ring = Main.LocalPlayer.FindItem(ItemType<WeddingRing>());
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
            npc.ai[0] = 1;
            NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
        }
    }
    public class SkullDiggerFriendly_Spirit : SkullDiggerFriendly
    {
        public override bool HasCruxButton(Player player) => !player.HasItem(ItemType<CruxCardSkullDigger>());
        public override void CruxButton(Player player)
        {
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardSkullDigger>(), "SkullDigger.NoCruxSpiritDialogue", "SkullDigger.CruxSpiritDialogue");
        }

        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger";
        public override bool CheckActive() => false;
        public override bool PreAI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
            {
                NPC.alpha += 10;
                if (NPC.alpha >= 255)
                {
                    bool oneActive = false;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player2 = Main.player[i];
                        if (!player2.active || player2.dead || !player2.RedemptionAbility().SpiritwalkerActive)
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
                if (NPC.alpha > 30)
                    NPC.alpha -= 10;
            }

            if (!RedeHelper.AnyProjectiles(ProjectileType<SkullDiggerFriendly_FlailBlade>()))
                NPC.Shoot(NPC.Center, ProjectileType<SkullDiggerFriendly_FlailBlade>(), NPC.damage, Vector2.Zero, NPC.whoAmI, 1);

            NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 15) / 3;
            if (NPC.alpha <= 30)
            {
                NPC.alpha += Main.rand.Next(-10, 11);
                NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 0, 30);
            }
            return false;
        }
        public override string GetChat()
        {
            return Language.GetTextValue("Mods.Redemption.Dialogue.SkullDigger.SpiritDialogue");
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D HandsTex = Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Hands").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
            spriteBatch.End();
            spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.3f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Rectangle rect = new(0, 0, HandsTex.Width, HandsTex.Height);
            spriteBatch.Draw(HandsTex, NPC.Center - screenPos - new Vector2(14, -32), new Rectangle?(rect), NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!RedeBossDowned.skullDiggerSaved || NPC.AnyNPCs(NPC.type))
                return 0;

            if (spawnInfo.Player.RedemptionAbility().SpiritwalkerActive && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                bool sorrow = spawnInfo.Player.HasItem(ItemType<SorrowfulEssence>());
                return sorrow ? 0.8f : 0.4f;
            }
            return 0;
        }
    }
}