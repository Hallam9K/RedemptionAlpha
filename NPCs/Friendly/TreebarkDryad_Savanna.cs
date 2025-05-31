using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Textures;
using Redemption.UI.ChatUI;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class TreebarkDryad_Savanna : TreebarkDryad
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return CrossMod.CrossMod.Reforged.Enabled;
        }

        private int EyeFrameY;
        private int EyeFrameX;

        public override void SetStaticDefaults()
        {
            BetterDialogue.BetterDialogue.SupportedNPCs.Add(Type);
            BetterDialogue.BetterDialogue.RegisterShoppableNPC(Type);
            NPCLists.Inorganic.Add(Type);
            NPCLists.Plantlike.Add(Type);

            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (setName == null)
            {
                WeightedRandom<string> name = new(Main.rand);
                name.Add(Language.GetTextValue("Mods.Redemption.NPCs.TreebarkDryad.Name1Acacia"));
                name.Add(Language.GetTextValue("Mods.Redemption.NPCs.TreebarkDryad.Name2Acacia"));
                name.Add(Language.GetTextValue("Mods.Redemption.NPCs.TreebarkDryad.Name3Acacia"));
                name.Add(Language.GetTextValue("Mods.Redemption.NPCs.TreebarkDryad.Name4Acacia"));
                name.Add(Language.GetTextValue("Mods.Redemption.NPCs.TreebarkDryad.Name5Acacia"));
                setName = name;
            }
            else
                typeName = setName + Language.GetTextValue("Mods.Redemption.NPCs.TreebarkDryad.Title");
        }
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (TimerRand == 0)
            {
                WoodType = 0;
                TimerRand = 1;
            }
            else
            {
                if (TimerRand == 1)
                {
                    if (NPC.life < NPC.lifeMax - 10 && !Main.dedServ)
                    {
                        Texture2D bubble = !Main.dedServ ? CommonTextures.TextBubble_Epidotra.Value : null;
                        SoundStyle voice = CustomSounds.Voice2 with { Pitch = -1f };

                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.1"), Color.LightGreen, Color.ForestGreen, voice, .06f, 2f, .5f, true, bubble: bubble, endID: 1));
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                        TimerRand = 2;
                    }
                }
                else if (TimerRand == 3)
                {
                    if (NPC.life < NPC.lifeMax / 2 && !Main.dedServ)
                    {
                        Texture2D bubble = !Main.dedServ ? CommonTextures.TextBubble_Epidotra.Value : null;
                        SoundStyle voice = CustomSounds.Voice2 with { Pitch = -1f };

                        string gender = player.Male ? Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.3") : Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.4");
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.2") + gender + Language.GetTextValue("Mods.Redemption.Cutscene.TreebarkDryad.5"), Color.LightGreen, Color.ForestGreen, voice, .06f, 2, .5f, true, bubble: bubble));
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                        TimerRand = 4;
                    }
                }
            }
            if (Main.LocalPlayer.talkNPC <= -1 || Main.npc[Main.LocalPlayer.talkNPC].whoAmI != NPC.whoAmI)
                return;

            int goreType = GoreID.TreeLeaf_Palm;

            if (Main.rand.NextBool(60) && Main.netMode != NetmodeID.Server)
                Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X + Main.rand.Next(-12, 4), NPC.Center.Y + Main.rand.Next(6)), NPC.velocity, goreType);
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            TimerRand = 3;
        }
        public override bool CanChat() => true;
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
                shopName = Shop.Name;
        }

        public override bool CheckActive()
        {
            return !Main.dayTime;
        }

        public override void FindFrame(int frameHeight)
        {
            EyeFrameX = 0;

            if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].whoAmI == NPC.whoAmI)
            {
                if (NPC.frame.Y < 4 * frameHeight)
                    NPC.frame.Y = 4 * frameHeight;

                if (++NPC.frameCounter >= 15)
                {
                    EyeFrameY = 1;

                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 8 * frameHeight)
                        NPC.frame.Y = 4 * frameHeight;
                }
            }
            else
            {
                if (++NPC.frameCounter >= 15)
                {
                    EyeFrameY = 0;
                    if (Main.rand.NextBool(8))
                        EyeFrameY = 1;

                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                        NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

        public override string GetChat()
        {
            Main.BestiaryTracker.Chats.SetWasChatWithDirectly(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<TreebarkDryad>()]);
            WeightedRandom<string> chat = new(Main.rand);

            if (NPC.life < (int)(NPC.lifeMax * .8f) || RedeBossDowned.downedTreebark)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.FelledDialogue1"), 10);
            if (NPC.life < (int)(NPC.lifeMax * .5f) || RedeBossDowned.downedTreebark)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.FelledDialogue2"), 10);
            if (RedeBossDowned.downedTreebark)
            {
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.FelledDialogue3"), 10);
                return Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Felled") + chat;
            }

            if (RedeWorld.Alignment < 0)
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Dialogue1"));

            chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.TreebarkDryad.Dialogue5"));
            return "Hmmmm... " + chat;
        }

        Asset<Texture2D> EyesTex;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            EyesTex ??= Request<Texture2D>("Redemption/NPCs/Friendly/TreebarkDryad_Savanna_Eyes");
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            int Height = EyesTex.Height() / 2;
            int Width = EyesTex.Width();
            int y = Height * EyeFrameY;
            int x = Width * EyeFrameX;
            Rectangle rect = new(x, y, Width, Height);
            Vector2 origin = new(Width / 2f, Height / 2f);

            if (NPC.frame.Y < 400)
            {
                spriteBatch.Draw(EyesTex.Value, NPC.Center - screenPos - new Vector2(6 * -NPC.spriteDirection, NPC.frame.Y >= 100 && NPC.frame.Y < 300 ? 12 : 14), new Rectangle?(rect), NPC.ColorTintedAndOpacity(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (NPC.AnyNPCs(Type))
                return 0;

            int score = 0;
            for (int x = -40; x <= 40; x++)
            {
                for (int y = -40; y <= 40; y++)
                {
                    int type = Framing.GetTileSafely(spawnInfo.SpawnTileX + x, spawnInfo.SpawnTileY + y).TileType;
                    if (CrossMod.CrossMod.Reforged.TryFind("AcaciaTree", out ModTile acaciaTree))
                    {
                        if (type == acaciaTree.Type)
                            score++;
                    }
                }
            }

            if (CrossMod.CrossMod.Reforged.TryFind("SavannaGrass", out ModTile savannaGrass))
            {
                float baseChance = SpawnCondition.OverworldDay.Chance;
                float multiplier = Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY).TileType == savannaGrass.Type ? (Main.raining ? 0.025f : 0.008f) : 0f;
                float trees = score >= 5 ? 1 : 0;
                float nymphIncrease = RedeQuest.forestNymphVar > 0 ? 2 : 1;

                return baseChance * multiplier * trees * nymphIncrease;
            }
            return 0;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 35; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture, Scale: 2f);

                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(0, 60), NPC.velocity, Find<ModGore>("Redemption/TreebarkDryadGoreLeg_Savanna").Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, Find<ModGore>("Redemption/TreebarkDryadGoreAntler_Savanna").Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(40, 0), NPC.velocity, Find<ModGore>("Redemption/TreebarkDryadGoreAntlerB_Savanna").Type, 1);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(20, 10), NPC.velocity, Find<ModGore>("Redemption/TreebarkDryadGoreHead_Savanna").Type, 1);
            }
            for (int i = 0; i < 3; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            if (CrossMod.CrossMod.Reforged.TryFind("DrywoodItem", out ModItem drywood))
                npcLoot.Add(ItemDropRule.Common(drywood.Type, 1, 40, 60));
            npcLoot.Add(ItemDropRule.Common(ItemID.Acorn, 1, 10, 20));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }
    }
}