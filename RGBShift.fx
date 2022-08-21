#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0


float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 xWorldViewProjection;
Texture xColoredTexture;
float DisplacementScroll;
float DisplacementDist;

sampler ColoredTextureSampler = sampler_state
{
	texture = <xColoredTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 textureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 textureCoordinates : TEXCOORD0;
	float4 color: COLOR0;
};

float2 radialDistortion(float2 coord, float2 pos)
{
	float distortion = DisplacementDist;

	float2 cc = pos - 0.5;
	float dist = dot(cc, cc) * distortion;
	return coord * (pos + cc * (1.0 + dist) * dist) / pos;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	
	float2 texCoord2 = input.textureCoordinates.xy + DisplacementScroll;
	float2 texCoord3 = input.textureCoordinates.xy - DisplacementScroll;

	float4 color1 = tex2D(ColoredTextureSampler, texCoord2);
	float4 color2 = tex2D(ColoredTextureSampler, input.textureCoordinates.xy);
	float4 color3 = tex2D(ColoredTextureSampler, texCoord3);

	float4 cc = tex2D(ColoredTextureSampler, radialDistortion(input.textureCoordinates, input.textureCoordinates));

	
	return input.color * float4(color1.r, color2.g, color3.b, color1.a) * cc;

}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        //VertexShader = compile vs_2_0 VertexShaderFunction();
		
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
