using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.UI;
using Redemption.WorldGeneration;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly
{
    public class Chalice_Intro : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override string Texture => "Redemption/Items/Usable/AlignmentTeller";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chalice of Alignment");
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
        }
        public override bool CheckActive() => false;
        public Vector2[] extraPos = new Vector2[4];
        public float extraAlpha;
        public float extraAlpha2;
        public float rotSpeed;

        private bool spawned = false;
        private bool skipIntro = false;
        private List<Player> askedPlayers;

        public override void AI()
        {
            if (!spawned)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Choir with { Pitch = -.9f }, NPC.position);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    askedPlayers = Main.player.Where(p => p.active && !p.dead).ToList();

                Main.LocalPlayer.Redemption().yesChoice = false;
                Main.LocalPlayer.Redemption().noChoice = false;
                ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue1"), 260, 30, 0, Color.DarkGoldenrod);
                NPC.velocity.Y = -5;
                spawned = true;
            }

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Lighting.AddLight(NPC.Center, Color.Lime.ToVector3() * 0.6f * Main.essScale);

            if (NPC.alpha > 0)
                NPC.alpha -= 2;
            switch (TimerRand)
            {
                case 0:
                    NPC.velocity.Y *= 0.97f;
                    if (AITimer++ >= 320)
                    {
                        ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue2"), 800, 30, 0, Color.DarkGoldenrod);
                        YesNoUI.DisplayYesNoButtons(Main.LocalPlayer, Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.Yes"), Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.No"), new Vector2(0, 28), new Vector2(0, 28), .6f, .6f);
                        AITimer = 0;
                        TimerRand = 1;
                    }
                    break;
                case 1:
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (Main.LocalPlayer.Redemption().noChoice)
                        {
                            if (!skipIntro)
                            {
                                ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue3"), 60, 30, 0, Color.DarkGoldenrod);
                                skipIntro = true;
                            }
                        }
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (askedPlayers.All(p => p.Redemption().noChoice || p.Redemption().yesChoice))
                        {
                            if (askedPlayers.Any(p => p.Redemption().yesChoice))
                            {
                                rotSpeed = 0.005f;
                                AITimer = 0;
                                TimerRand = 2;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                AITimer = 0;
                                TimerRand = 3;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                    break;
                case 2:
                    if (extraAlpha < 1)
                        extraAlpha += 0.02f;
                    if (AITimer <= 80)
                        rotSpeed += 0.1f;
                    else if (AITimer > 120)
                        rotSpeed -= 0.01f;
                    NPC.localAI[0] += rotSpeed;
                    rotSpeed = MathHelper.Clamp(rotSpeed, 0.5f, 2f);

                    if (NPC.localAI[1] < 50)
                        NPC.localAI[1] += 1;
                    for (int i = 0; i < 3; i++)
                    {
                        extraPos[i] = NPC.Center + Vector2.One.RotatedBy(MathHelper.ToRadians((360 / 3 * i) + NPC.localAI[0])) * NPC.localAI[1];
                    }
                    AITimer++;
                    if (!skipIntro)
                    {
                        if (AITimer == 30 + 60)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue4"), 300, 20, 0, Color.DarkGoldenrod);
                        if (AITimer == 340 + 60)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue5"), 400, 20, 0, Color.Green);
                        if (AITimer == 780 + 60)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue6"), 400, 20, 0, Color.Red);
                        if (AITimer == 1220 + 60)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue7"), 300, 20, 0, Color.DarkGoldenrod);
                        if (AITimer == 1560 + 60)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue8"), 300, 20, 0, Color.DarkGoldenrod);
                    }
                    if (AITimer >= 1900 + 60)
                    {
                        if (extraAlpha2 < 1)
                            extraAlpha2 += 0.02f;

                        extraPos[3] = NPC.Center + new Vector2(0, -170);
                    }
                    if (!skipIntro)
                    {
                        if (AITimer == 1900 + 120)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue9"), 300, 20, 0, Color.Goldenrod);
                        if (AITimer == 2240 + 120)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Dialogue.Chalice.Dialogue10"), 180, 20, 0, Color.DarkGoldenrod);
                    }
                    if (AITimer >= 2240 + 120)
                    {
                        extraAlpha -= 0.04f;
                        extraAlpha2 -= 0.04f;
                    }
                    if (AITimer >= 2460 + 120)
                    {
                        Despawn();
                    }
                    break;
                case 3:
                    if (AITimer++ >= 62)
                    {
                        Despawn();
                    }
                    break;
            }
            if (RedeConfigClient.Instance.CameraLockDisable || skipIntro)
                return;
            ScreenPlayer.CutsceneLock(Main.LocalPlayer, NPC, ScreenPlayer.CutscenePriority.Max, 0, 0, 0);
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(2f).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            Main.LocalPlayer.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);
        }

        private void Despawn()
        {
            SoundEngine.PlaySound(SoundID.Item68, NPC.position);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                RedeDraw.SpawnExplosion(NPC.Center, Color.White, scale: 1, noDust: true, tex: "Redemption/Textures/HolyGlow2");

                RedeWorld.alignmentGiven = true;
                RedeWorld.SyncData();

                NPC.active = false;
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        private float drawTimer;
        public float flareOpacity;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (flareOpacity > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive();

                Texture2D bigFlareAni = ModContent.Request<Texture2D>("Redemption/Textures/BigFlare").Value;
                Vector2 origin2 = new(bigFlareAni.Width / 2f, bigFlareAni.Height / 2f);
                spriteBatch.Draw(bigFlareAni, NPC.Center - screenPos, null, Color.LightGoldenrodYellow, 0, origin2, flareOpacity, 0, 0);
                spriteBatch.Draw(bigFlareAni, NPC.Center - screenPos, null, Color.LightGoldenrodYellow * .7f, 0, origin2, flareOpacity * 1.5f, 0, 0);
                spriteBatch.Draw(bigFlareAni, NPC.Center - screenPos, null, Color.White * .5f, 0, origin2, flareOpacity * 2, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
            if (TimerRand >= 4)
                return false;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = NPC.frame.Size() / 2;

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale);
            Main.spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();
            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteEyeFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 flareOrigin = new(flare.Width / 2, flare.Height / 2);
            for (int i = 0; i < 4; i++)
            {
                Color colour = new(54, 193, 59);
                switch (i)
                {
                    case 1:
                        colour = new(226, 45, 28);
                        break;
                    case 2:
                        colour = new(203, 189, 99);
                        break;
                    case 3:
                        flare = ModContent.Request<Texture2D>("Redemption/Textures/BadRedeEyeFlare").Value;
                        colour = Color.White;
                        break;
                }

                Main.EntitySpriteDraw(flare, extraPos[i] - screenPos, new Rectangle?(rect), colour * (i == 3 ? extraAlpha2 : extraAlpha) * Main.rand.NextFloat(0.7f, 0.8f), NPC.rotation, flareOrigin, 1 * Main.rand.NextFloat(0.95f, 1f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(flare, extraPos[i] - screenPos, new Rectangle?(rect), colour * (i == 3 ? extraAlpha2 : extraAlpha) * Main.rand.NextFloat(0.7f, 0.8f) * 0.4f, NPC.rotation, flareOrigin, 1.5f * Main.rand.NextFloat(0.95f, 1f), SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            for (int i = 0; i < 4; i++)
            {
                Texture2D routeTex = ModContent.Request<Texture2D>("Redemption/Items/GoodRoute").Value;
                switch (i)
                {
                    case 1:
                        routeTex = ModContent.Request<Texture2D>("Redemption/Items/BadRoute").Value;
                        break;
                    case 2:
                        routeTex = ModContent.Request<Texture2D>("Redemption/Items/RedemptionRoute").Value;
                        break;
                    case 3:
                        routeTex = ModContent.Request<Texture2D>("Redemption/Items/BadRedemptionRoute").Value;
                        break;
                }
                Vector2 origin2 = new(routeTex.Width / 2, routeTex.Height / 2);
                Main.spriteBatch.Draw(routeTex, extraPos[i] - screenPos, null, Color.White * (i == 3 ? extraAlpha2 : extraAlpha), NPC.rotation, origin2, 1, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}