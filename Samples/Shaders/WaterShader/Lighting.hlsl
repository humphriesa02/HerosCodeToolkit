
float LightingSpecular(float3 L, float3 N, float3 V, float exponent)
{
    L = SafeNormalize(L);
    N = SafeNormalize(N);
    V = SafeNormalize(V);

    float NdotL = saturate(dot(N, L));

    if (NdotL <= 0.0)
        return 0.0;

    float3 H = SafeNormalize(L + V);
    float NdotH = saturate(dot(N, H));

    return pow(NdotH, max(exponent, 1.0));
}

void MainLighting_float(
    float3 normalWS,
    float3 positionWS,
    float3 viewWS,
    float smoothness,
    out float specular)
{
    specular = 0.0;

    #ifndef SHADERGRAPH_PREVIEW
    float exponent = exp2(10.0 * saturate(smoothness) + 1.0);

    normalWS = SafeNormalize(normalWS);
    viewWS = SafeNormalize(viewWS);

    Light mainLight =
        GetMainLight(TransformWorldToShadowCoord(positionWS));

    specular = LightingSpecular(
        mainLight.direction,
        normalWS,
        viewWS,
        exponent
    );

    specular *= mainLight.color.r; // Temporary diagnostic only
    specular *= mainLight.distanceAttenuation;
    specular *= mainLight.shadowAttenuation;
    #endif
}

void AdditionalLighting_float(
    float3 normalWS,
    float3 positionWS,
    float3 viewWS,
    float smoothness,
    float hardness,
    out float3 specular)
{
    specular = 0.0;

    #ifndef SHADERGRAPH_PREVIEW
    float exponent = exp2(10.0 * saturate(smoothness) + 1.0);

    normalWS = SafeNormalize(normalWS);
    viewWS = SafeNormalize(viewWS);
    hardness = saturate(hardness);

    int pixelLightCount = GetAdditionalLightsCount();

    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, positionWS);

        float specularSoft = LightingSpecular(
            light.direction,
            normalWS,
            viewWS,
            exponent
        );

        float specularHard = smoothstep(
            0.005, 0.01, specularSoft
        );

        float specularTerm = lerp(
            specularSoft,
            specularHard,
            hardness
        );

        specular += specularTerm
            * light.color
            * light.distanceAttenuation
            * light.shadowAttenuation;
    }
    #endif
}
