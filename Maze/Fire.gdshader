shader_type canvas_item;

uniform float radius = 100.0;
uniform float edge_softness = 50.0;

void fragment() {
    vec2 center = vec2(576.0, 324.0);
    float dist = distance(FRAGCOORD.xy, center);
    float alpha = smoothstep(radius, radius + edge_softness, dist);
    COLOR = vec4(0.0, 0.0, 0.1, alpha);
}
