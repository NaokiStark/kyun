float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 xWorldViewProjection;
Texture2D xColoredTexture;
float DisplacementScroll;
float DisplacementDist;

sampler ColoredTextureSampler = sampler_state
{
	texture = <xColoredTexture>;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 textureCoordinates : TEXCOORD0;
	float4 Color: COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 textureCoordinates : TEXCOORD0;
	float4 Color: COLOR0;
};

float2 radialDistortion(float2 coord, float2 pos)
{
	float distortion = DisplacementDist;

	float2 cc = pos - 0.5;
	float dist = dot(cc, cc) * distortion;
	return coord * (pos + cc * (1.0 + dist) * dist) / pos;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	float2 texCoord2 = input.textureCoordinates.xy + DisplacementScroll;
	float2 texCoord3 = input.textureCoordinates.xy - DisplacementScroll;

	float4 color1 = tex2D(ColoredTextureSampler, texCoord2);
	float4 color2 = tex2D(ColoredTextureSampler, input.textureCoordinates.xy);
	float4 color3 = tex2D(ColoredTextureSampler, texCoord3);

	//float4 cc = tex2D(ColoredTextureSampler, radialDistortion(input.textureCoordinates, input.textureCoordinates));

	
	return input.Color * tex2D(ColoredTextureSampler, input.textureCoordinates) * float4(color1.r, color2.g, color3.b, color1.a);
	//return input.color * float4(color1.r, color2.g, color3.b, color1.a) * cc;
	//return input.color;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
      VertexShaderOutput output;
      output.Position = input.Position;
      output.Color = input.Color;
      output.textureCoordinates = input.textureCoordinates;
      return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        //VertexShader = compile vs_4_0 VertexShaderFunction();
		
		PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
