

// May be in your best interest to read this page: https://thebookofshaders.com/

    // Samplers are textures numbered with s[n], where [n] is the index used for the gd.Textures array.
        // Samplers also use the SamplerState set with the gd.SamplerStates array (where the first index is set by sb.Begin.)
    // When using sb.Draw the first index (s0) is the texture you set as the first argument.
sampler image0 : register(s0);

float somefloat;

    // float2s are the same structure as a c# Vector2.
float2 somefloat2;
    // Same goes for higher dimensional vectors (up to float4)
    // float3s and float4s also store color data where each vector (x,y,z,w or r,g,b,a is a value from 0-1)
float3 somefloat3;
float4 somefloat4;

    // Parameters in this global scope can be set from the rendering code by using something like:
        // MyEffectAsset.Value.Parameters["name"].SetValue(value);
        // Colors must converted to vectors with .ToVector3/4();

    // Unqualified to speak on ps shader inputs. (May want to check the following:
        // https://learn.microsoft.com/en-us/windows/win32/direct3d11/pixel-shader-stage
        // https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics
    // )
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
        // Samples the texture at the texture cordinate of this pixel, remember texture coordinates are normalized from 0-1 on both axis.
    float4 col = tex2D(image0, coords);

    return col;
}

    // Passes are indexed based on the order in the technique here. (Pass name is irrelevant.)
        // Use MyEffectAsset.Value.CurrentTechnique.Passes[passIndex].Apply(); to apply your shader. 
            // Additionally remember to use a spritebatch began with SpriteSortMode.Immediate, OR an ordinary spritebatch began with your effect passed as an argument.
technique Technique1
{
    pass Pass1
    {
            // If your shader is too complex/uses newer features, try using compile ps_3_0 PixelShaderFunction(); to use a more up to date shader model.
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

