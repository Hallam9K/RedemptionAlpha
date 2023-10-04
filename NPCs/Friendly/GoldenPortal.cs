using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Redemption.Globals;
using System;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Redemption.Base;

namespace Redemption.NPCs.Friendly
{
    public class GoldenPortal : ModNPC
    {
        public override string Texture => "Redemption/Textures/PortalTex";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Golden Gateway");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 188;
            NPC.height = 188;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 999;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.alpha = 20;
            NPC.npcSlots = 0;
            NPC.hide = true;
            NPC.behindTiles = true;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public override bool UsesPartyHat() => false;
        private float RotTime;
        public override void AI()
        {
            NPC.dontTakeDamage = true;
            NPC.rotation += .02f;
            if (NPC.alpha < 255)
            {
                if (Vector2.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), NPC.Center) <= Main.screenWidth / 2 + 100)
                {
                    RotTime += (float)Math.PI / 120;
                    RotTime *= 1.01f;
                    if (RotTime >= Math.PI) RotTime = 0;
                    float timer = RotTime;
                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoR:Shockwave", NPC.Center)?.GetShader().UseProgress(timer).UseOpacity(100f * (1 - timer / 1.3f)).UseColor(2, 8, 5).UseTargetPosition(NPC.Center);

                    if (RotTime > 0.5 && RotTime < 0.6 && !Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.PortalWub, NPC.position);
                }

                for (int i = 0; i < 30; i++)
                {
                    float distance = Main.rand.Next(20) * 4;
                    Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                    Vector2 dustPosition = NPC.Center + dustRotation;
                    Vector2 nextDustPosition = NPC.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                    Vector2 dustVelocity = dustPosition - nextDustPosition + NPC.velocity;
                    if (Main.rand.NextBool(5))
                    {
                        Dust dust = Dust.NewDustPerfect(dustPosition, DustID.GoldFlame, dustVelocity, Scale: 0.2f);
                        dust.scale = distance / 30;
                        dust.scale = MathHelper.Clamp(dust.scale, 0.2f, 2);
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.alpha += 10;
                        dust.rotation = dustRotation.ToRotation();
                    }
                }
                for (int i = 0; i < 30; i++)
                {
                    float distance = Main.rand.Next(25) * 4;
                    Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                    Vector2 dustPosition = NPC.Center + dustRotation;
                    Vector2 nextDustPosition = NPC.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                    Vector2 dustVelocity = (dustPosition - nextDustPosition + NPC.velocity) * -1;
                    if (Main.rand.NextBool(40))
                    {
                        Dust dust = Dust.NewDustPerfect(dustPosition, DustID.GoldFlame, dustVelocity);
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.alpha += 10;
                        dust.rotation = dustRotation.ToRotation();
                    }
                }
            }
        }

        public override bool CanChat() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.NegativeDye);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(255, 195, 0), new Color(255, 195, 0) * 0.5f, new Color(255, 195, 0));

            spriteBatch.End();
            spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition + new Vector2(0f, offset * 4f), NPC.frame, NPC.GetAlpha(new Color(255, 195, 0)) * 0.4f, -NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition + new Vector2(0f, offset * 4f), NPC.frame, NPC.GetAlpha(new Color(255, 195, 0)) * 0.4f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2.9f, SpriteEffects.None, 0f);

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition + new Vector2(0f, offset * 4f), NPC.frame, NPC.GetAlpha(color), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.8f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }
    }
}