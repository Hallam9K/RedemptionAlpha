using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Textures;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.UI;

public class ElementButtonUI : BuilderToggle
{
    public override string HoverTexture => Texture;

    public override bool Active() => true;

    public override int NumberOfStates => 1;

    public override Position OrderPosition => new After(TorchBiome);

    public override bool OnLeftClick(ref SoundStyle? sound)
    {
        Main.LocalPlayer.Redemption().elementStatsUISeen = true;
        ElementPanelUI.Visible = !ElementPanelUI.Visible;
        sound = ElementPanelUI.Visible ? SoundID.MenuOpen : SoundID.MenuClose;
        return true;
    }

    Texture2D glowTex;
    public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
    {
        drawParams.Frame = drawParams.Texture.Frame(1, 2);

        if (Main.LocalPlayer.Redemption().elementStatsUISeen)
            return true;

        glowTex ??= CommonTextures.BigFlare.Value;
        Vector2 drawOrigin = new(glowTex.Width / 2, glowTex.Height / 2);
        Color c = Color.LightBlue;
        c.A = 0;
        float cAlpha = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, .4f, 1f, .4f);
        float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, .9f, 1f, .9f);

        spriteBatch.Draw(glowTex, drawParams.Position, null, c * cAlpha, Main.GlobalTimeWrappedHourly, drawOrigin, 0.15f * scale, 0, 0);
        spriteBatch.Draw(glowTex, drawParams.Position, null, c * cAlpha, (Main.GlobalTimeWrappedHourly) + MathHelper.PiOver2, drawOrigin, 0.15f * scale, 0, 0);

        return true;
    }

    public override bool DrawHover(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
    {
        drawParams.Frame = drawParams.Texture.Frame(1, 2, 0, 1);
        return true;
    }

    public override string DisplayValue()
    {
        return Language.GetTextValue("Mods.Redemption.UI.ElementPanelUI.Button");
    }
}