#if defined(VERTEX) || __VERSION__ > 100 || defined(GL_FRAGMENT_PRECISION_HIGH)
	#define MY_HIGHP_OR_MEDIUMP highp
#else
	#define MY_HIGHP_OR_MEDIUMP mediump
#endif

//  watch shader Mods/FickleFox/assets/shaders/ghostRare.fs
extern MY_HIGHP_OR_MEDIUMP vec2 ghostRare;
extern MY_HIGHP_OR_MEDIUMP number dissolve;
extern MY_HIGHP_OR_MEDIUMP number time;
extern MY_HIGHP_OR_MEDIUMP vec4 texture_details;
extern MY_HIGHP_OR_MEDIUMP vec2 image_details;
extern bool shadow;
extern MY_HIGHP_OR_MEDIUMP vec4 burn_colour_1;
extern MY_HIGHP_OR_MEDIUMP vec4 burn_colour_2;

vec4 dissolve_mask(vec4 tex, vec2 texture_coords, vec2 uv)
{
    if (dissolve < 0.001) {
        return vec4(shadow ? vec3(0.,0.,0.) : tex.xyz, shadow ? tex.a*0.3: tex.a);
    }

    float adjusted_dissolve = (dissolve*dissolve*(3.-2.*dissolve))*1.02 - 0.01; //Adjusting 0.0-1.0 to fall to -0.1 - 1.1 scale so the mask does not pause at extreme values

	float t = time * 10.0 + 2003.;
	vec2 floored_uv = (floor((uv*texture_details.ba)))/max(texture_details.b, texture_details.a);
    vec2 uv_scaled_centered = (floored_uv - 0.5) * 2.3 * max(texture_details.b, texture_details.a);
	
	vec2 field_part1 = uv_scaled_centered + 50.*vec2(sin(-t / 143.6340), cos(-t / 99.4324));
	vec2 field_part2 = uv_scaled_centered + 50.*vec2(cos( t / 53.1532),  cos( t / 61.4532));
	vec2 field_part3 = uv_scaled_centered + 50.*vec2(sin(-t / 87.53218), sin(-t / 49.0000));

    float field = (1.+ (
        cos(length(field_part1) / 19.483) + sin(length(field_part2) / 33.155) * cos(field_part2.y / 15.73) +
        cos(length(field_part3) / 27.193) * sin(field_part3.x / 21.92) ))/2.;
    vec2 borders = vec2(0.2, 0.8);

    float res = (.5 + .5* cos( (adjusted_dissolve) / 82.612 + ( field + -.5 ) *3.14))
    - (floored_uv.x > borders.y ? (floored_uv.x - borders.y)*(5. + 5.*dissolve) : 0.)*(dissolve)
    - (floored_uv.y > borders.y ? (floored_uv.y - borders.y)*(5. + 5.*dissolve) : 0.)*(dissolve)
    - (floored_uv.x < borders.x ? (borders.x - floored_uv.x)*(5. + 5.*dissolve) : 0.)*(dissolve)
    - (floored_uv.y < borders.x ? (borders.x - floored_uv.y)*(5. + 5.*dissolve) : 0.)*(dissolve);

    if (tex.a > 0.01 && burn_colour_1.a > 0.01 && !shadow && res < adjusted_dissolve + 0.8*(0.5-abs(adjusted_dissolve-0.5)) && res > adjusted_dissolve) {
        if (!shadow && res < adjusted_dissolve + 0.5*(0.5-abs(adjusted_dissolve-0.5)) && res > adjusted_dissolve) {
            tex.rgba = burn_colour_1.rgba;
        } else if (burn_colour_2.a > 0.01) {
            tex.rgba = burn_colour_2.rgba;
        }
    }

    return vec4(shadow ? vec3(0.,0.,0.) : tex.xyz, res > adjusted_dissolve ? (shadow ? tex.a*0.3: tex.a) : .0);
}

number hue(number s, number t, number h)
{
	number hs = mod(h, 1.)*6.;
	if (hs < 1.) return (t-s) * hs + s;
	if (hs < 3.) return t;
	if (hs < 4.) return (t-s) * (4.-hs) + s;
	return s;
}

vec4 RGB(vec4 c)
{
	if (c.y < 0.0001)
		return vec4(vec3(c.z), c.a);

	number t = (c.z < .5) ? c.y*c.z + c.z : -c.y*c.z + (c.y+c.z);
	number s = 2.0 * c.z - t;
	return vec4(hue(s,t,c.x + 1./3.), hue(s,t,c.x), hue(s,t,c.x - 1./3.), c.w);
}

vec4 HSL(vec4 c)
{
	number low = min(c.r, min(c.g, c.b));
	number high = max(c.r, max(c.g, c.b));
	number delta = high - low;
	number sum = high+low;

	vec4 hsl = vec4(.0, .0, .5 * sum, c.a);
	if (delta == .0)
		return hsl;

	hsl.y = (hsl.z < .5) ? delta / sum : delta / (2.0 - sum);

	if (high == c.r)
		hsl.x = (c.g - c.b) / delta;
	else if (high == c.g)
		hsl.x = (c.b - c.r) / delta + 2.0;
	else
		hsl.x = (c.r - c.g) / delta + 4.0;

	hsl.x = mod(hsl.x / 6., 1.);
	return hsl;
}

vec2 hash2(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)),
             dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453);
}


float snow(vec2 uv, float scale) {
    float w = smoothstep(1.0, 0.0, -uv.y * (scale / 10.0));
    if (w < 0.1) return 0.0;

    uv += ghostRare.y / scale;
    uv.y += ghostRare.y * 2.0 / scale;
    uv.x += sin(uv.y + ghostRare.y * 0.5) / scale;

    uv *= scale;
    vec2 s = floor(uv);
    vec2 f = fract(uv);
    vec2 p;
    float k = 3.0, d;
    p = 0.5 + 0.35 * sin(11.0 * fract(sin((s + p + scale) * mat2(7, 3, 6, 5)) * 5.0)) - f;
    d = length(p);
    k = min(d, k);
    k = smoothstep(0.0, k, sin(f.x + f.y) * 0.01);

    return k * w;
}


vec4 effect(vec4 colour, Image texture, vec2 texture_coords, vec2 screen_coords)
{
   vec4 tex = Texel(texture, texture_coords);
vec2 uv = (((texture_coords) * image_details) - texture_details.xy * texture_details.ba) / texture_details.ba;

float whiteness = tex.r * tex.g * tex.b;
bool is_white = whiteness > 0.85;

number low = min(tex.r, min(tex.g, tex.b));
number high = max(tex.r, max(tex.g, tex.b));
number delta = high - low - 0.1;

// Protect saturated colors near center
vec2 centeredUV = abs(uv - 0.5);
bool isNearCenter = max(centeredUV.x, centeredUV.y) < 0.25;
bool isDeepPurple = tex.r > 0.3 && tex.b > 0.3 && tex.g < 0.2;
bool isProtectedColor = delta > 0.4;// && isNearCenter;

vec2 adjusted_uv = uv - vec2(0.5);


// Radial distance check
float radial_dist = length(adjusted_uv);
bool is_outer_region = radial_dist > 0.1;

if (is_white) {

    // Compute animated position by shifting upward over time
    float riseSpeed = 0.1; // tweak this
    //vec2 animatedUV = uv - vec2(0.0, mod(time * riseSpeed + randOffset.y, 1.0));

    vec2 smoke_uv = uv * 20.0;
    float gradientMask = clamp(dot(uv, normalize(vec2(1.0, 1.0))), 0.0, 1.0);
    float maskStrength = smoothstep(0.3, 0.8, gradientMask);

    //trying to integrate .r, rotstion, to see if we can make the smoke move more
    float t = time * 12.5 + ghostRare.x * 0.015;

    float noise = (
        sin(smoke_uv.x + t + ghostRare.r * 1.15) +
        cos(smoke_uv.y * 1.3 - t * 0.6) +
        sin(smoke_uv.x * 0.8 + smoke_uv.y * 0.6 + t * 0.7) +
        cos((smoke_uv.x + smoke_uv.y) * 1.5 - t * 0.9)
    ) * 0.25 + 0.5;

    // float softness = smoothstep(0.1, 0.9, noise);
    float softness = smoothstep(0.1, 0.9, noise) * maskStrength;

    //new sparkle test
    float inverseMask = 1.0 - maskStrength;

    // Animate upward motion from center line

    vec2 animatedUV = uv - vec2(0.0, mod(time * riseSpeed + ghostRare.y * 2.0, 1.0)); // randomize offset per card

// Mote generation
vec2 gridPos = floor(animatedUV * 40.0);
vec2 randOffset = hash2(gridPos);
vec2 motePos = (gridPos + randOffset) / 40.0;

float dist = length(animatedUV - motePos);
float mote = smoothstep(0.02, 0.0, dist);

// Flicker
float flicker = abs(sin(time * 5.0 + dot(gridPos, vec2(3.1, 4.7)))) * 0.5 + 0.5;

// Fade as they move up
float verticalDistFromCenter = abs(animatedUV.y - 0.5);
float fadeOut = smoothstep(0.3, 0.5, verticalDistFromCenter);
mote *= 1.0 - fadeOut;

// Final color
vec3 moteColor = vec3(1.0) * mote * flicker * inverseMask * 0.6;

    // Grid for mote placement
    // vec2 gridPos = floor(animatedUV * 40.0);
    // //vec2 randOffset = fract(sin(dot(gridPos, vec2(12.9898,78.233))) * 43758.5453);
    // vec2 randOffset = hash2(gridPos);

    // vec2 motePos = (gridPos + randOffset) / 40.0;

    // // Distance from pixel to mote
    // float dist = length(uv - motePos);
    // float mote = smoothstep(0.02, 0.0, dist); // small soft circle

    // // Optional twinkle/flicker
    // float flicker = abs(sin(time *ghostRare.y * 5.0 + dot(gridPos, vec2(3.1, 4.7)))) * 0.5 + 0.5;

    // vec3 moteColor = vec3(1.0) * mote * flicker * inverseMask * 0.6;
    //end new


    tex.rgb += moteColor;
    vec4 k = vec4(vec3(softness * 0.6), 1.0);
    k.rgb;// += moteColor;// + finalColor;
    return dissolve_mask(k, texture_coords, uv);
}

if (isProtectedColor){
     number fac = 0.8 + 0.9 * sin(11. * uv.x + 4.32 * uv.y + ghostRare.r * 12. + cos(ghostRare.r * 5.3 + uv.y * 4.2 - uv.x * 4.));
    number fac2 = 0.5 + 0.5 * sin(8. * uv.x + 2.32 * uv.y + ghostRare.r * 5. - cos(ghostRare.r * 2.3 + uv.x * 8.2));
    number fac3 = 0.5 + 0.5 * sin(10. * uv.x + 5.32 * uv.y + ghostRare.r * 6.111 + sin(ghostRare.r * 5.3 + uv.y * 3.2));
    number fac4 = 0.5 + 0.5 * sin(3. * uv.x + 2.32 * uv.y + ghostRare.r * 8.111 + sin(ghostRare.r * 1.3 + uv.y * 11.2));
    number fac5 = sin(0.9 * 16. * uv.x + 5.32 * uv.y + ghostRare.r * 12. + cos(ghostRare.r * 5.3 + uv.y * 4.2 - uv.x * 4.));

    number maxfac = 0.7 * max(max(fac, max(fac2, max(fac3, 0.0))) + (fac + fac2 + fac3 * fac4), 0.);

    // looks weird
    vec3 inverted = mix(tex.rgb, 1.0 - tex.rgb, smoothstep(0.1, 1.0, high));
    
    float gradientFactor = (uv.x + uv.y) * 0.5;  
    vec3 blueTint = vec3(0.2, 0.4, 1.0);  
    vec3 redTint = vec3(1.0, 0.2, 0.3); 

    // mix it up
    vec3 finalColor = mix(blueTint, redTint, gradientFactor) * inverted;
    
    finalColor.r = finalColor.r - delta + delta * maxfac * (0.7 + fac5 * 0.27) - 0.1;
    finalColor.g = finalColor.g - delta + delta * maxfac * (0.7 - fac5 * 0.27) - 0.1;
    finalColor.b = finalColor.b - delta + delta * maxfac * 0.7 - 0.1;

    tex.rgb = finalColor;
    tex.a = tex.a * (0.5 * max(min(1., max(0., 0.3 * max(low * 0.2, delta) + min(max(maxfac * 0.1, 0.), 0.4))), 0.) + 0.15 * maxfac * (0.1 + delta));

    return dissolve_mask(tex, texture_coords, uv);
}

    // Faded color logic
    number saturation_fac = 1.0 - max(0.0, 0.05 * (1.1 - delta));
    vec4 hsl = HSL(vec4(tex.r * saturation_fac, tex.g * saturation_fac, tex.b, tex.a));

    float t = ghostRare.x * 2.221 + time * 10;
    vec2 floored_uv = (floor((uv * texture_details.ba))) / texture_details.ba;
    vec2 uv_scaled_centered = (floored_uv - 0.5) * 50.0;

    vec2 field_part1 = uv_scaled_centered + 50.0 * vec2(sin(-t / 143.63), cos(-t / 99.43));
    vec2 field_part2 = uv_scaled_centered + 50.0 * vec2(cos(t / 53.15), cos(t / 61.45));
    vec2 field_part3 = uv_scaled_centered + 50.0 * vec2(sin(-t / 87.53), sin(-t / 49.00));

    float field = (1.0 + (
        cos(length(field_part1) / 19.48) +
        sin(length(field_part2) / 33.15) * cos(field_part2.y / 15.73) +
        cos(length(field_part3) / 27.19) * sin(field_part3.x / 21.92)
    )) / 2.0;

    float res = 0.5 + 0.5 * cos((ghostRare.x) * 2.612 + (field - 0.5) * 3.14);

    // hue toward bluish greys
    float hueShift = 0.58 + 0.02 * sin(time * 0.5 + field * 6.283); 
    hsl.x = mix(hsl.x, hueShift, 0.9); 

    // Shimmering Shine Boost from negative shine
    float fac = 0.8 + 0.9 * sin(11. * uv.x + 4.32 * uv.y + ghostRare.r * 12. + cos(ghostRare.r * 5.3 + uv.y * 4.2 - uv.x * 4.));
    float fac2 = 0.5 + 0.5 * sin(8. * uv.x + 2.32 * uv.y + ghostRare.r * 5. - cos(ghostRare.r * 2.3 + uv.x * 8.2));
    float fac3 = 0.5 + 0.5 * sin(10. * uv.x + 5.32 * uv.y + ghostRare.r * 6.111 + sin(ghostRare.r * 5.3 + uv.y * 3.2));
    float fac4 = 0.5 + 0.5 * sin(3. * uv.x + 2.32 * uv.y + ghostRare.r * 8.111 + sin(ghostRare.r * 1.3 + uv.y * 11.2));
    float fac5 = sin(0.9 * 16. * uv.x + 5.32 * uv.y + ghostRare.r * 12. + cos(ghostRare.r * 5.3 + uv.y * 4.2 - uv.x * 4.));

    float maxfac = 0.7 * max(max(fac, max(fac2, max(fac3, 0.0))) + (fac + fac2 + fac3 * fac4), 0.);

    // Option A: if you're still working in HSL, boost the lightness
    hsl.z = clamp(hsl.z + maxfac * 0.25, 0.3, 1.0);

    // Option B: if already in RGB
    // tex.rgb += maxfac * vec3(0.2, 0.25, 0.3); // adds a light shimmer tint

    
    hsl.y = min(0.25, hsl.y * 0.4);  // very faint saturation
    hsl.z = mix(0.6, 0.85, pow(abs(sin(time + uv.x * 2.0)), 3.0));

    
    float specular = pow(abs(sin(time * 1.5 + uv.y * 5.0)), 8.0) * 0.2;
    hsl.z = clamp(hsl.z + specular, 0.3, 1.0);

    tex.rgb = RGB(hsl).rgb;

    if (tex.a < 0.7) tex.a = tex.a / 3.0;

    return dissolve_mask(tex * colour, texture_coords, uv);
}


extern MY_HIGHP_OR_MEDIUMP vec2 mouse_screen_pos;
extern MY_HIGHP_OR_MEDIUMP float hovering;
extern MY_HIGHP_OR_MEDIUMP float screen_scale;

#ifdef VERTEX
vec4 position( mat4 transform_projection, vec4 vertex_position )
{
    if (hovering <= 0.){
        return transform_projection * vertex_position;
    }
    float mid_dist = length(vertex_position.xy - 0.5*love_ScreenSize.xy)/length(love_ScreenSize.xy);
    vec2 mouse_offset = (vertex_position.xy - mouse_screen_pos.xy)/screen_scale;
    float scale = 0.2*(-0.03 - 0.3*max(0., 0.3-mid_dist))
                *hovering*(length(mouse_offset)*length(mouse_offset))/(2. -mid_dist);

    return transform_projection * vertex_position + vec4(0,0,0,scale);
}
#endif