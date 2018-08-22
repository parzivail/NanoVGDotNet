#version 330 core

out vec4 FragColor; 
in vec2 TexCoords;

uniform vec2 iResolution;
uniform sampler2D iChannel0;
uniform float iTime;

uniform float grainAmplitude;
uniform float maskSize;
uniform float scanlineSize;
uniform float jitterChance;
uniform float trackingLossChance;
uniform float unfocus;

vec3 kernel[3];

vec3 mod289(vec3 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec2 mod289(vec2 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec3 permute(vec3 x) {
  return mod289(((x*34.0)+1.0)*x);
}

float rand(vec2 v)
  {
  const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                      0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                     -0.577350269189626,  // -1.0 + 2.0 * C.x
                      0.024390243902439); // 1.0 / 41.0
// First corner
  vec2 i  = floor(v + dot(v, C.yy) );
  vec2 x0 = v -   i + dot(i, C.xx);

// Other corners
  vec2 i1;
  //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
  //i1.y = 1.0 - i1.x;
  i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
  // x0 = x0 - 0.0 + 0.0 * C.xx ;
  // x1 = x0 - i1 + 1.0 * C.xx ;
  // x2 = x0 - 1.0 + 2.0 * C.xx ;
  vec4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;

// Permutations
  i = mod289(i); // Avoid truncation effects in permutation
  vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
    + i.x + vec3(0.0, i1.x, 1.0 ));

  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m ;
  m = m*m ;

// Gradients: 41 points uniformly over a line, mapped onto a diamond.
// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

  vec3 x = 2.0 * fract(p * C.www) - 1.0;
  vec3 h = abs(x) - 0.5;
  vec3 ox = floor(x + 0.5);
  vec3 a0 = x - ox;

// Normalise gradients implicitly by scaling m
// Approximation of: m *= inversesqrt( a0*a0 + h*h );
  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

// Compute final noise value at P
  vec3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return ((130.0 * dot(m, g)) + 1.) / 2.;
}

vec2 ntscInterlaceJitter(vec2 randvec, vec2 uv)
{
    int uvY = int(uv.y * iResolution.y / maskSize);
    if (rand(randvec + vec2(0., uvY)) < jitterChance && mod(float(uvY), 2.) == 0.)
        uv.x += (scanlineSize / iResolution.x) * (rand(randvec + vec2(0., uvY)) * 2. - 1.);
    return uv;
}

vec4 sampleKernel(sampler2D channel, vec2 uv)
{
    kernel[0] = vec3(0.1, 0.2, 0.1);
    kernel[1] = vec3(0.8, -1.4, 0.8);
    kernel[2] = vec3(0.1, 0.2, 0.1);
    
    vec2 onePxX = vec2(unfocus / iResolution.x, 0.);
    vec2 onePxY = vec2(0., unfocus / iResolution.y);
    
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
    vec2 now = vec2(0., iTime * 10.);

    // create UV
    vec2 uv = TexCoords;

	if (rand(now) < trackingLossChance)
		uv.y += (maskSize / iResolution.x);
    
    // jitter lines
    uv = ntscInterlaceJitter(now, uv);
    
    // round off pixels to mask size
    uv = vec2(floor(uv.x / (maskSize / iResolution.x)) * (maskSize / iResolution.x), uv.y);
    
    // sample texture
    vec4 col = sampleKernel(iChannel0, uv);

    // add grain
    col += vec4(vec3(grainAmplitude * (rand(gl_FragCoord.xy + now) * 2. - 1.)), 1.0);
    
    // output to screen
    FragColor = col;
}