using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System;
using Terraria.Audio;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Dusts;
using Redemption.Globals;

namespace Redemption.NPCs.Friendly
{
    public class GathuramPortal : ModNPC
    {
        public override string Texture => "Redemption/Textures/PortalTex";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mysterious Gateway");
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
            NPC.ShowNameOnHover = false;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
        public override bool CheckActive() => false;
        public override bool UsesPartyHat() => false;
        private float RotTime;
        public override void AI()
        {
            NPC.dontTakeDamage = true;
            NPC.rotation += .02f;
            NPC.velocity *= 0f;
            if (Vector2.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), NPC.Center) <= Main.screenWidth / 2 + 100)
            {
                if (NPC.ai[0]++ % 20 == 0)
                {
                    Vector2 spawnPos = new Vector2(0f, -50f).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360f)));

                    ParticleManager.NewParticle(NPC.Center + spawnPos, spawnPos.RotatedBy(Main.rand.NextFloat(-10f, 10f)), new GathuramPortal_EnergyGather(), Color.White, 1f, NPC.Center.X, NPC.Center.Y);
                }

                RotTime += (float)Math.PI / 120;
                RotTime *= 1.01f;
                if (RotTime >= Math.PI) RotTime = 0;
                float timer = RotTime;
                Terraria.Graphics.Effects.Filters.Scene.Activate("MoR:Shockwave", NPC.Center)?.GetShader().UseProgress(timer).UseOpacity(100f * (1 - timer / 1.3f)).UseColor(2, 8, 5).UseTargetPosition(NPC.Center);

                if (RotTime > 0.5 && RotTime < 0.6 && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.PortalWub, NPC.position);
            }
        }

        public override bool CanChat() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            spriteBatch.End();
            spriteBatch.BeginAdditive();

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(Color.CornflowerBlue), -NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.8f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.BeginDefault();

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(Color.CornflowerBlue), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.1f, SpriteEffects.None, 0f);


            Texture2D extra = ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value;
            spriteBatch.Draw(extra, NPC.Center - screenPos, null, Color.LightBlue * NPC.Opacity * .4f, -NPC.rotation * 1.5f, new Vector2(extra.Width / 2, extra.Height / 2), NPC.scale, 0, 0f);
            spriteBatch.Draw(extra, NPC.Center - screenPos, null, Color.LightBlue * NPC.Opacity * .2f, -NPC.rotation * 2f, new Vector2(extra.Width / 2, extra.Height / 2), NPC.scale * .7f, 0, 0f);
            spriteBatch.Draw(extra, NPC.Center - screenPos, null, Color.LightBlue * NPC.Opacity * .1f, -NPC.rotation * 2.5f, new Vector2(extra.Width / 2, extra.Height / 2), NPC.scale * .5f, 0, 0f);

            spriteBatch.End();
            spriteBatch.BeginDefault();
            return false;
        }
    }
}