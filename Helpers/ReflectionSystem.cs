using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

#nullable enable
namespace Redemption.Helpers;
public class ReflectionSystem : ModSystem
{
    // Spritebatch fields
    public static FieldInfo? sbSortMode, sbBlendState, sbSamplerState, sbDepthStencilState, sbRasterizerState, sbCustomEffect, sbTransformMatrix, sbBeginCalled;

    public override void Load()
    {
        if (Main.dedServ)
            return;

        // Spritebatch fields
        Type sbType = Main.spriteBatch.GetType();
        sbSortMode = sbType.GetField("sortMode", BindingFlags.Instance | BindingFlags.NonPublic);
        sbBlendState = sbType.GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic);
        sbSamplerState = sbType.GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic);
        sbDepthStencilState = sbType.GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic);
        sbRasterizerState = sbType.GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic);
        sbCustomEffect = sbType.GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic);
        sbTransformMatrix = sbType.GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic);
        sbBeginCalled = sbType.GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic);
    }
}
