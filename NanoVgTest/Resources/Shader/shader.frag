#version 330 core

out vec4 FragColor; 
in vec2 TexCoords;

uniform vec2 iResolution;
uniform sampler2D iChannel0;
uniform float iTime;

vec3 kernel[3];

const float kernelSampleDistance = 0.48;
const float maskSize = 4.;

float rand(vec2 p) { return fract(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); }

vec2 ntscInterlaceJitter(vec2 randvec, vec2 uv)
{
    int uvY = int(uv.y * iResolution.y / maskSize);
    if (rand(randvec + vec2(0., uvY)) > 0.9 && mod(float(uvY), 2.) == 0.)
        uv.x += (2. / iResolution.x) * (rand(randvec + vec2(0., uvY)) * 2. - 1.);
    return uv;
}

vec4 sampleKernel(sampler2D channel, vec2 uv)
{
    kernel[0] = vec3(0.2, 0.4, 0.2);
    kernel[1] = vec3(0.8, -2.3, 0.8);
    kernel[2] = vec3(0.2, 0.4, 0.2);
    
    vec2 onePxX = vec2(kernelSampleDistance / iResolution.x, 0.);
    vec2 onePxY = vec2(0., kernelSampleDistance / iResolution.y);
    
    vec4 col = kernel[0].x * texture(channel, uv - onePxX - onePxY);
    col += kernel[0].y * texture(channel, uv - onePxY);
    col += kernel[0].z * texture(channel, uv + onePxX - onePxY);

    col += kernel[1].x * texture(channel, uv - onePxX);
    col += kernel[1].y * texture(channel, uv);
    col += kernel[1].z * texture(channel, uv + onePxX);

    col += kernel[2].x * texture(iChannel0, uv - onePxX + onePxY);
    col += kernel[2].y * texture(channel, uv + onePxY);
    col += kernel[2].z * texture(channel, uv + onePxX + onePxY);
    
    return col;
}

void main()
{    
    // current time seed
    vec2 now = vec2(iTime / 2., iTime * 2.);

    // create UV
    vec2 uv = TexCoords;
    
    // jitter lines
    uv = ntscInterlaceJitter(now, uv);
    
    // round off pixels to mask size
    uv = vec2(floor(uv.x / (maskSize / iResolution.x)) * (maskSize / iResolution.x), uv.y);
    
    // sample texture
    vec4 col = sampleKernel(iChannel0, uv);

    // add grain
    col += vec4(vec3(0.04 * (rand(gl_FragCoord.xy + now) * 2. - 1.)), 1.0);
    
    // output to screen
    FragColor = col;
}