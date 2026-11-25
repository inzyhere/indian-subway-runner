import json
import math
import struct
import zlib
from pathlib import Path

FLOAT = 5126
UINT = 5123
ARRAY_BUFFER = 34962
ELEMENT_ARRAY_BUFFER = 34963
LINEAR = 9729
LINEAR_MIPMAP_LINEAR = 9987
REPEAT = 10497


def quat_from_euler(x=0.0, y=0.0, z=0.0):
    cx, sx = math.cos(x * 0.5), math.sin(x * 0.5)
    cy, sy = math.cos(y * 0.5), math.sin(y * 0.5)
    cz, sz = math.cos(z * 0.5), math.sin(z * 0.5)
    return [
        sx * cy * cz + cx * sy * sz,
        cx * sy * cz - sx * cy * sz,
        cx * cy * sz + sx * sy * cz,
        cx * cy * cz - sx * sy * sz,
    ]


def rotate(value, angles):
    x, y, z = value
    rx, ry, rz = [math.radians(a) for a in angles]
    if rx:
        c, s = math.cos(rx), math.sin(rx)
        y, z = y * c - z * s, y * s + z * c
    if ry:
        c, s = math.cos(ry), math.sin(ry)
        x, z = x * c + z * s, -x * s + z * c
    if rz:
        c, s = math.cos(rz), math.sin(rz)
        x, y = x * c - y * s, x * s + y * c
    return (x, y, z)


def normalize(value):
    length = math.sqrt(sum(v * v for v in value)) or 1.0
    return tuple(v / length for v in value)


def transform_geometry(geom, translation=(0.0, 0.0, 0.0), rotation=(0.0, 0.0, 0.0), scale=(1.0, 1.0, 1.0)):
    positions = []
    normals = []
    for position, normal in zip(geom["positions"], geom["normals"]):
        scaled = tuple(position[i] * scale[i] for i in range(3))
        rotated = rotate(scaled, rotation)
        positions.append(tuple(rotated[i] + translation[i] for i in range(3)))
        n_scaled = tuple(normal[i] / scale[i] if scale[i] else 0.0 for i in range(3))
        normals.append(normalize(rotate(n_scaled, rotation)))
    result = dict(geom)
    result["positions"] = positions
    result["normals"] = normals
    return result


def box_geometry(size, uv_tile=None):
    sx, sy, sz = size
    hx, hy, hz = sx * 0.5, sy * 0.5, sz * 0.5
    if uv_tile is None:
        uv_tile = (1.0, 1.0)
    faces = [
        ([(-hx, -hy, hz), (hx, -hy, hz), (hx, hy, hz), (-hx, hy, hz)], (0, 0, 1), (sx, sy)),
        ([(hx, -hy, -hz), (-hx, -hy, -hz), (-hx, hy, -hz), (hx, hy, -hz)], (0, 0, -1), (sx, sy)),
        ([(hx, -hy, hz), (hx, -hy, -hz), (hx, hy, -hz), (hx, hy, hz)], (1, 0, 0), (sz, sy)),
        ([(-hx, -hy, -hz), (-hx, -hy, hz), (-hx, hy, hz), (-hx, hy, -hz)], (-1, 0, 0), (sz, sy)),
        ([(-hx, hy, hz), (hx, hy, hz), (hx, hy, -hz), (-hx, hy, -hz)], (0, 1, 0), (sx, sz)),
        ([(-hx, -hy, -hz), (hx, -hy, -hz), (hx, -hy, hz), (-hx, -hy, hz)], (0, -1, 0), (sx, sz)),
    ]
    positions, normals, uvs, indices = [], [], [], []
    for corners, normal, extents in faces:
        base = len(positions)
        for index, corner in enumerate(corners):
            positions.append(corner)
            normals.append(normal)
            u = (0.0, 1.0, 1.0, 0.0)[index] * extents[0] / uv_tile[0]
            v = (0.0, 0.0, 1.0, 1.0)[index] * extents[1] / uv_tile[1]
            uvs.append((u, v))
        indices.extend([base, base + 1, base + 2, base, base + 2, base + 3])
    return {"positions": positions, "normals": normals, "uvs": uvs, "indices": indices}


def cylinder_geometry(radius, height, segments=16, top_radius=None, rotation=(0.0, 0.0, 0.0)):
    top_radius = radius if top_radius is None else top_radius
    positions, normals, uvs, indices = [], [], [], []
    for i in range(segments + 1):
        angle = 2.0 * math.pi * i / segments
        ca, sa = math.cos(angle), math.sin(angle)
        positions.append((ca * radius, -height * 0.5, sa * radius))
        normals.append((ca, 0.0, sa))
        uvs.append((i / segments, 0.0))
        positions.append((ca * top_radius, height * 0.5, sa * top_radius))
        side_normal = normalize((ca, (radius - top_radius) / max(height, 0.001), sa))
        normals.append(side_normal)
        uvs.append((i / segments, 1.0))
    for i in range(segments):
        base = i * 2
        indices.extend([base, base + 1, base + 3, base, base + 3, base + 2])
    for cap_y, cap_radius, cap_normal in [(-height * 0.5, radius, (0.0, -1.0, 0.0)), (height * 0.5, top_radius, (0.0, 1.0, 0.0))]:
        center = len(positions)
        positions.append((0.0, cap_y, 0.0))
        normals.append(cap_normal)
        uvs.append((0.5, 0.5))
        for i in range(segments + 1):
            angle = 2.0 * math.pi * i / segments
            positions.append((math.cos(angle) * cap_radius, cap_y, math.sin(angle) * cap_radius))
            normals.append(cap_normal)
            uvs.append((0.5 + math.cos(angle) * 0.5, 0.5 + math.sin(angle) * 0.5))
        for i in range(segments):
            if cap_normal[1] < 0:
                indices.extend([center, center + i + 2, center + i + 1])
            else:
                indices.extend([center, center + i + 1, center + i + 2])
    geom = {"positions": positions, "normals": normals, "uvs": uvs, "indices": indices}
    return transform_geometry(geom, rotation=rotation)


def sphere_geometry(radius, latitude=10, longitude=16):
    positions, normals, uvs, indices = [], [], [], []
    for lat in range(latitude + 1):
        theta = math.pi * lat / latitude
        for lon in range(longitude + 1):
            phi = 2.0 * math.pi * lon / longitude
            direction = (math.sin(theta) * math.cos(phi), math.cos(theta), math.sin(theta) * math.sin(phi))
            positions.append(tuple(v * radius for v in direction))
            normals.append(direction)
            uvs.append((lon / longitude, lat / latitude))
    for lat in range(latitude):
        for lon in range(longitude):
            a = lat * (longitude + 1) + lon
            b = a + longitude + 1
            indices.extend([a, b, a + 1, a + 1, b, b + 1])
    return {"positions": positions, "normals": normals, "uvs": uvs, "indices": indices}


def png_chunk(kind, payload):
    return struct.pack(">I", len(payload)) + kind + payload + struct.pack(">I", zlib.crc32(kind + payload) & 0xFFFFFFFF)


def make_png(width, height, pixel_fn):
    rows = bytearray()
    for y in range(height):
        rows.append(0)
        for x in range(width):
            rows.extend(pixel_fn(x, y))
    header = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    return b"\x89PNG\r\n\x1a\n" + png_chunk(b"IHDR", header) + png_chunk(b"IDAT", zlib.compress(bytes(rows), 9)) + png_chunk(b"IEND", b"")


def pattern(kind):
    size = 64

    def speckle(x, y):
        n = (x * 92837111 ^ y * 689287499 ^ 0x5BD1E995) & 0xFF
        base = 74 + n % 42
        return (base, base - 6, base - 15, 255)

    def wood(x, y):
        grain = int(10 * math.sin(y * 0.7 + math.sin(x * 0.2) * 2.0))
        base = 72 + grain
        return (base + 18, base, base - 20, 255)

    def rail(x, y):
        streak = 128 + int(28 * math.sin(x * 0.55)) + (x * y % 17)
        return (streak, streak - 18, streak - 34, 255)

    def tile(x, y):
        edge = x % 16 < 2 or y % 16 < 2
        if edge:
            return (96, 92, 84, 255)
        noise = (x * 31 + y * 17) % 11
        return (176 + noise, 167 + noise, 149 + noise, 255)

    def plaster(x, y):
        noise = (x * 13 ^ y * 7) % 9
        band = 0 if y % 22 < 4 else (10 if y < 32 else 0)
        return (202 + noise - band, 190 + noise - band, 166 + noise - band, 255)

    def livery(x, y):
        if y < 12 or y > 52:
            return (176, 35, 36, 255)
        if 16 < y < 22 or 42 < y < 48:
            return (231, 214, 178, 255)
        noise = (x * 7 + y * 3) % 7
        return (150 + noise, 27, 26, 255)

    def fabric(x, y):
        weave = 6 if (x + y) % 4 == 0 else 0
        return (28 + weave, 54 + weave, 104 + weave, 255)

    def canvas(x, y):
        weave = 8 if (x % 6 == 0 or y % 6 == 0) else 0
        return (118 + weave, 76 + weave, 42 + weave, 255)

    def brushed(x, y):
        streak = (x * 5 + y) % 13
        value = 138 + streak
        return (value, value, value + 6, 255)

    functions = {
        "ballast": speckle,
        "wood": wood,
        "rail": rail,
        "tile": tile,
        "plaster": plaster,
        "livery": livery,
        "fabric": fabric,
        "canvas": canvas,
        "metal": brushed,
    }
    return make_png(size, size, functions[kind])


class GLBBuilder:
    def __init__(self, generator):
        self.generator = generator
        self.binary = bytearray()
        self.buffer_views = []
        self.accessors = []
        self.nodes = []
        self.meshes = []
        self.materials = []
        self.textures = []
        self.images = []
        self.animations = []
        self.samplers = [{"magFilter": LINEAR, "minFilter": LINEAR_MIPMAP_LINEAR, "wrapS": REPEAT, "wrapT": REPEAT}]
        self.material_cache = {}
        self.texture_cache = {}
        self.scene_nodes = []

    def align(self):
        while len(self.binary) % 4:
            self.binary.append(0)

    def add_view(self, data, target=None):
        self.align()
        offset = len(self.binary)
        self.binary.extend(data)
        view = {"buffer": 0, "byteOffset": offset, "byteLength": len(data)}
        if target is not None:
            view["target"] = target
        self.buffer_views.append(view)
        return len(self.buffer_views) - 1

    def add_accessor(self, values, component_type, accessor_type, count, target, minimum=None, maximum=None):
        if component_type == FLOAT:
            data = struct.pack("<" + "f" * len(values), *values)
        else:
            data = struct.pack("<" + "H" * len(values), *values)
        accessor = {
            "bufferView": self.add_view(data, target),
            "componentType": component_type,
            "count": count,
            "type": accessor_type,
        }
        if minimum is not None:
            accessor["min"] = minimum
        if maximum is not None:
            accessor["max"] = maximum
        self.accessors.append(accessor)
        return len(self.accessors) - 1

    def add_texture(self, kind, name):
        key = (kind, name)
        if key in self.texture_cache:
            return self.texture_cache[key]
        png = pattern(kind)
        view = self.add_view(png)
        image_index = len(self.images)
        self.images.append({"bufferView": view, "mimeType": "image/png", "name": name})
        texture_index = len(self.textures)
        self.textures.append({"source": image_index, "sampler": 0, "name": name})
        self.texture_cache[key] = texture_index
        return texture_index

    def add_material(self, name, color, metallic=0.0, roughness=0.75, texture=None, emission=None, unlit=False):
        key = (name, tuple(color), metallic, roughness, texture, emission, unlit)
        if key in self.material_cache:
            return self.material_cache[key]
        pbr = {
            "baseColorFactor": list(color),
            "metallicFactor": metallic,
            "roughnessFactor": roughness,
        }
        if texture is not None:
            texture_index = self.add_texture(texture[0], texture[1])
            pbr["baseColorTexture"] = {"index": texture_index}
        material = {"name": name, "pbrMetallicRoughness": pbr, "doubleSided": True}
        if emission is not None:
            material["emissiveFactor"] = list(emission)
        if unlit:
            material["extensions"] = {"KHR_materials_unlit": {}}
        self.materials.append(material)
        self.material_cache[key] = len(self.materials) - 1
        return len(self.materials) - 1

    def add_mesh(self, name, geom, material):
        positions = [component for value in geom["positions"] for component in value]
        normals = [component for value in geom["normals"] for component in value]
        uvs = [component for value in geom["uvs"] for component in value]
        minimum = [min(value[i] for value in geom["positions"]) for i in range(3)]
        maximum = [max(value[i] for value in geom["positions"]) for i in range(3)]
        pos_index = self.add_accessor(positions, FLOAT, "VEC3", len(geom["positions"]), ARRAY_BUFFER, minimum, maximum)
        norm_index = self.add_accessor(normals, FLOAT, "VEC3", len(geom["normals"]), ARRAY_BUFFER)
        uv_index = self.add_accessor(uvs, FLOAT, "VEC2", len(geom["uvs"]), ARRAY_BUFFER)
        index_index = self.add_accessor(geom["indices"], UINT, "SCALAR", len(geom["indices"]), ELEMENT_ARRAY_BUFFER)
        primitive = {
            "attributes": {"POSITION": pos_index, "NORMAL": norm_index, "TEXCOORD_0": uv_index},
            "indices": index_index,
            "material": material,
        }
        self.meshes.append({"name": name, "primitives": [primitive]})
        return len(self.meshes) - 1

    def add_node(self, name, mesh=None, children=None, translation=None, rotation=None, scale=None, scene_root=False):
        node = {"name": name}
        if mesh is not None:
            node["mesh"] = mesh
        if children:
            node["children"] = children
        if translation:
            node["translation"] = list(translation)
        if rotation:
            node["rotation"] = list(rotation)
        if scale:
            node["scale"] = list(scale)
        index = len(self.nodes)
        self.nodes.append(node)
        if scene_root:
            self.scene_nodes.append(index)
        return index

    def attach(self, parent, child):
        self.nodes[parent].setdefault("children", []).append(child)

    def add_geometry_node(self, name, geom, material, parent=None, translation=None, rotation=None, scale=None, scene_root=False):
        mesh = self.add_mesh(name + "_mesh", geom, material)
        node = self.add_node(name, mesh=mesh, translation=translation, rotation=rotation, scale=scale, scene_root=scene_root)
        if parent is not None:
            self.attach(parent, node)
        return node

    def add_box(self, name, size, material, position=(0.0, 0.0, 0.0), rotation=(0.0, 0.0, 0.0), uv_tile=None, parent=None, scene_root=False):
        geom = transform_geometry(box_geometry(size, uv_tile), translation=position, rotation=rotation)
        return self.add_geometry_node(name, geom, material, parent=parent, scene_root=scene_root)

    def add_cylinder(self, name, radius, height, material, position=(0.0, 0.0, 0.0), rotation=(0.0, 0.0, 0.0), segments=16, top_radius=None, parent=None, scene_root=False):
        geom = transform_geometry(cylinder_geometry(radius, height, segments, top_radius), translation=position, rotation=rotation)
        return self.add_geometry_node(name, geom, material, parent=parent, scene_root=scene_root)

    def add_sphere(self, name, radius, material, position=(0.0, 0.0, 0.0), parent=None, scene_root=False):
        geom = transform_geometry(sphere_geometry(radius), translation=position)
        return self.add_geometry_node(name, geom, material, parent=parent, scene_root=scene_root)

    def add_animation(self, name, tracks):
        samplers, channels = [], []
        for node, path, times, values in tracks:
            input_index = self.add_accessor(times, FLOAT, "SCALAR", len(times), ARRAY_BUFFER, [min(times)], [max(times)])
            components = 4 if path == "rotation" else 3
            output_index = self.add_accessor(values, FLOAT, "VEC4" if components == 4 else "VEC3", len(values) // components, ARRAY_BUFFER)
            sampler_index = len(samplers)
            samplers.append({"input": input_index, "output": output_index, "interpolation": "LINEAR"})
            channels.append({"sampler": sampler_index, "target": {"node": node, "path": path}})
        self.animations.append({"name": name, "samplers": samplers, "channels": channels})

    def save(self, path):
        self.align()
        document = {
            "asset": {"version": "2.0", "generator": self.generator},
            "scene": 0,
            "scenes": [{"nodes": self.scene_nodes, "name": "Root"}],
            "nodes": self.nodes,
            "meshes": self.meshes,
            "materials": self.materials,
            "animations": self.animations,
            "textures": self.textures,
            "images": self.images,
            "samplers": self.samplers,
            "accessors": self.accessors,
            "bufferViews": self.buffer_views,
            "buffers": [{"byteLength": len(self.binary)}],
        }
        if not self.animations:
            document.pop("animations")
        if not self.textures:
            document.pop("textures")
            document.pop("images")
            document.pop("samplers")
        if not self.materials:
            document.pop("materials")
        json_data = json.dumps(document, separators=(",", ":"), ensure_ascii=False).encode("utf-8")
        while len(json_data) % 4:
            json_data += b" "
        binary_data = bytes(self.binary)
        total = 12 + 8 + len(json_data) + 8 + len(binary_data)
        output = bytearray()
        output.extend(struct.pack("<4sII", b"glTF", 2, total))
        output.extend(struct.pack("<I4s", len(json_data), b"JSON"))
        output.extend(json_data)
        output.extend(struct.pack("<I4s", len(binary_data), b"BIN\x00"))
        output.extend(binary_data)
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(bytes(output))


def build_character(path, police):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    skin = builder.add_material("skin", (0.82, 0.59, 0.42, 1.0), roughness=0.85)
    hair = builder.add_material("hair", (0.08, 0.06, 0.05, 1.0), roughness=0.9)
    shoe = builder.add_material("shoe", (0.05, 0.05, 0.06, 1.0), roughness=0.6)
    if police:
        uniform = builder.add_material("navy_uniform", (0.16, 0.28, 0.58, 1.0), roughness=0.82, texture=("fabric", "T_Uniform"))
        accent = builder.add_material("uniform_accent", (0.91, 0.76, 0.22, 1.0), roughness=0.68)
        jacket = builder.add_material("police_jacket", (0.10, 0.19, 0.42, 1.0), roughness=0.78)
    else:
        uniform = builder.add_material("suspect_jacket", (0.48, 0.12, 0.12, 1.0), roughness=0.88, texture=("canvas", "T_Jacket"))
        accent = builder.add_material("suspect_accent", (0.92, 0.42, 0.16, 1.0), roughness=0.72)
        jacket = builder.add_material("suspect_trousers", (0.12, 0.10, 0.12, 1.0), roughness=0.9)
    badge = builder.add_material("badge", (0.96, 0.83, 0.32, 1.0), metallic=0.7, roughness=0.3)
    root = builder.add_node("CharacterRoot", rotation=quat_from_euler(0.0, math.pi, 0.0), scene_root=True)
    hips = builder.add_node("Hips", translation=(0.0, 0.86, 0.0))
    builder.attach(root, hips)
    torso = builder.add_node("Torso")
    builder.attach(hips, torso)
    builder.add_box("TorsoMesh", (0.52, 0.72, 0.30), uniform, position=(0.0, 0.34, 0.0), parent=torso)
    builder.add_box("ChestPanel", (0.36, 0.20, 0.04), accent, position=(0.0, 0.48, -0.17), parent=torso)
    if police:
        builder.add_box("Badge", (0.10, 0.12, 0.02), badge, position=(-0.14, 0.50, -0.20), parent=torso)
        builder.add_box("Belt", (0.54, 0.09, 0.32), shoe, position=(0.0, 0.02, 0.0), parent=torso)
        builder.add_box("Satchel", (0.24, 0.28, 0.12), jacket, position=(0.36, 0.14, 0.10), rotation=(0.0, 0.0, -8.0), parent=torso)
    else:
        builder.add_box("Scarf", (0.34, 0.10, 0.32), accent, position=(0.0, 0.69, 0.0), parent=torso)
    neck = builder.add_node("Neck", translation=(0.0, 0.72, 0.0))
    builder.attach(torso, neck)
    builder.add_box("NeckMesh", (0.16, 0.12, 0.16), skin, parent=neck)
    builder.add_sphere("Head", 0.23, skin, position=(0.0, 0.20, 0.0), parent=neck)
    builder.add_sphere("Hair", 0.235, hair, position=(0.0, 0.28, 0.02), parent=neck)
    builder.add_box("FaceShadow", (0.26, 0.10, 0.03), hair, position=(0.0, 0.22, -0.21), parent=neck)
    if police:
        builder.add_cylinder("CapBand", 0.25, 0.12, jacket, position=(0.0, 0.44, 0.0), parent=neck)
        builder.add_box("CapPeak", (0.44, 0.04, 0.22), jacket, position=(0.0, 0.41, -0.22), parent=neck)
        builder.add_box("CapBadge", (0.08, 0.08, 0.02), badge, position=(0.0, 0.45, -0.25), parent=neck)
    else:
        builder.add_box("Cap", (0.42, 0.12, 0.42), jacket, position=(0.0, 0.44, 0.0), rotation=(0.0, 8.0, 0.0), parent=neck)
    arm_left = builder.add_node("ArmL", translation=(-0.34, 0.64, 0.0))
    arm_right = builder.add_node("ArmR", translation=(0.34, 0.64, 0.0))
    builder.attach(torso, arm_left)
    builder.attach(torso, arm_right)
    for name, joint, side in [("ArmL", arm_left, -1), ("ArmR", arm_right, 1)]:
        builder.add_box(name + "Upper", (0.16, 0.38, 0.17), uniform, position=(0.0, -0.19, 0.0), parent=joint)
        builder.add_sphere(name + "Elbow", 0.09, skin, position=(0.0, -0.40, 0.0), parent=joint)
        builder.add_box(name + "Lower", (0.14, 0.34, 0.15), skin, position=(0.0, -0.58, 0.0), parent=joint)
        builder.add_box(name + "Cuff", (0.15, 0.07, 0.16), accent if police else jacket, position=(0.0, -0.42, 0.0), parent=joint)
    leg_left = builder.add_node("LegL", translation=(-0.14, 0.0, 0.0))
    leg_right = builder.add_node("LegR", translation=(0.14, 0.0, 0.0))
    builder.attach(hips, leg_left)
    builder.attach(hips, leg_right)
    for name, joint in [("LegL", leg_left), ("LegR", leg_right)]:
        builder.add_box(name + "Upper", (0.20, 0.42, 0.22), jacket, position=(0.0, -0.21, 0.0), parent=joint)
        builder.add_box(name + "Lower", (0.18, 0.40, 0.20), jacket, position=(0.0, -0.60, 0.0), parent=joint)
        builder.add_box(name + "Shoe", (0.22, 0.12, 0.34), shoe, position=(0.0, -0.83, -0.06), parent=joint)
        builder.add_box(name + "Stripe", (0.19, 0.05, 0.21), accent if police else jacket, position=(0.0, -0.42, 0.0), parent=joint)
    times = [0.0, 0.25, 0.5, 0.75, 1.0]
    run_values = {
        "ArmL": [0, 32, 0, -32, 0],
        "ArmR": [0, -32, 0, 32, 0],
        "LegL": [0, -34, 0, 34, 0],
        "LegR": [0, 34, 0, -34, 0],
    }
    tracks = []
    for node_name, angles in run_values.items():
        node = {"ArmL": arm_left, "ArmR": arm_right, "LegL": leg_left, "LegR": leg_right}[node_name]
        values = []
        for angle in angles:
            values.extend(quat_from_euler(math.radians(angle), 0.0, 0.0))
        tracks.append((node, "rotation", times, values))
    builder.add_animation("run", tracks)
    idle_values = []
    for _ in times:
        idle_values.extend(quat_from_euler(0.0, 0.0, 0.0))
    builder.add_animation("idle", [(node, "rotation", times, list(idle_values)) for node in [arm_left, arm_right, leg_left, leg_right]])
    jump_angles = {"ArmL": -70, "ArmR": -70, "LegL": 45, "LegR": -35}
    jump_tracks = []
    for node_name, angle in jump_angles.items():
        node = {"ArmL": arm_left, "ArmR": arm_right, "LegL": leg_left, "LegR": leg_right}[node_name]
        q = quat_from_euler(math.radians(angle), 0.0, 0.0)
        jump_tracks.append((node, "rotation", times, q * 5))
    builder.add_animation("jump", jump_tracks)
    slide_values = []
    for _ in times:
        slide_values.extend(quat_from_euler(math.radians(-65), 0.0, 0.0))
    builder.add_animation("slide", [(torso, "rotation", times, slide_values)])
    builder.save(path)


def build_train(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    body = builder.add_material("coach_livery", (1.0, 1.0, 1.0, 1.0), roughness=0.55, texture=("livery", "T_TrainLivery"))
    roof = builder.add_material("coach_roof", (0.30, 0.32, 0.35, 1.0), metallic=0.45, roughness=0.5, texture=("metal", "T_Roof"))
    window = builder.add_material("window_glass", (0.08, 0.16, 0.22, 1.0), metallic=0.25, roughness=0.18)
    trim = builder.add_material("coach_trim", (0.95, 0.78, 0.24, 1.0), metallic=0.35, roughness=0.42)
    wheel = builder.add_material("wheel", (0.10, 0.10, 0.11, 1.0), metallic=0.6, roughness=0.5)
    lamp = builder.add_material("lamp", (1.0, 0.88, 0.55, 1.0), emission=(0.65, 0.48, 0.18), roughness=0.3)
    root = builder.add_node("ExpressCoach", rotation=quat_from_euler(0.0, math.pi, 0.0), scene_root=True)
    builder.add_box("Body", (2.20, 1.85, 8.0), body, position=(0.0, 1.28, 0.0), uv_tile=(2.0, 1.0), parent=root)
    builder.add_box("Underframe", (2.30, 0.28, 8.05), wheel, position=(0.0, 0.35, 0.0), parent=root)
    builder.add_box("Roof", (2.10, 0.28, 8.0), roof, position=(0.0, 2.28, 0.0), parent=root)
    builder.add_box("RoofSpine", (0.55, 0.16, 8.05), trim, position=(0.0, 2.47, 0.0), parent=root)
    for z in (-3.2, -1.6, 0.0, 1.6, 3.2):
        builder.add_box("RoofRib%.1f" % z, (2.14, 0.08, 0.14), roof, position=(0.0, 2.43, z), parent=root)
    for side in (-1, 1):
        x = side * 1.115
        for z in (-3.0, -1.5, 1.5, 3.0):
            builder.add_box("Window%+.1f%d" % (z, side), (0.03, 0.50, 0.92), window, position=(x, 1.63, z), parent=root)
            builder.add_box("WindowTrim%+.1f%d" % (z, side), (0.04, 0.06, 1.00), trim, position=(x, 1.90, z), parent=root)
        builder.add_box("Door%d" % side, (0.04, 1.30, 0.82), trim, position=(x, 1.18, 0.0), parent=root)
        builder.add_box("DoorWindow%d" % side, (0.05, 0.42, 0.56), window, position=(x, 1.58, 0.0), parent=root)
        builder.add_box("Stripe%d" % side, (0.04, 0.10, 7.80), trim, position=(x, 0.82, 0.0), parent=root)
        for z in (-2.8, -1.2, 1.2, 2.8):
            builder.add_cylinder("Wheel%d%+.1f" % (side, z), 0.34, 0.14, wheel, position=(side * 1.08, 0.26, z), rotation=(0.0, 0.0, 90.0), segments=18, parent=root)
        builder.add_box("Buffer%d" % side, (0.16, 0.20, 0.16), wheel, position=(x, 0.45, -4.05), parent=root)
    builder.add_box("CabFront", (1.98, 1.15, 0.20), trim, position=(0.0, 1.55, -4.06), parent=root)
    builder.add_box("Windshield", (1.45, 0.50, 0.06), window, position=(0.0, 1.70, -4.18), parent=root)
    builder.add_sphere("HeadlightLeft", 0.11, lamp, position=(-0.70, 1.03, -4.16), parent=root)
    builder.add_sphere("HeadlightRight", 0.11, lamp, position=(0.70, 1.03, -4.16), parent=root)
    builder.save(path)


def build_low_barrier(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    metal = builder.add_material("barrier_metal", (0.42, 0.45, 0.48, 1.0), metallic=0.55, roughness=0.5)
    hazard = builder.add_material("hazard", (1.0, 0.61, 0.12, 1.0), roughness=0.65)
    paint = builder.add_material("barrier_paint", (0.90, 0.88, 0.78, 1.0), roughness=0.78)
    root = builder.add_node("PlatformBarrier", scene_root=True)
    builder.add_box("Base", (1.90, 0.12, 0.70), metal, position=(0.0, 0.06, 0.0), parent=root)
    builder.add_box("Panel", (1.74, 0.58, 0.10), paint, position=(0.0, 0.42, 0.0), parent=root)
    for x in (-0.62, -0.20, 0.22, 0.64):
        builder.add_box("Hazard%+.2f" % x, (0.16, 0.58, 0.12), hazard, position=(x, 0.42, -0.01), rotation=(0.0, 0.0, 18.0), parent=root)
    for x in (-0.82, 0.82):
        builder.add_box("Post%+.2f" % x, (0.10, 0.82, 0.12), metal, position=(x, 0.41, 0.0), parent=root)
        builder.add_box("Foot%+.2f" % x, (0.28, 0.08, 0.42), metal, position=(x, 0.04, 0.0), parent=root)
    builder.save(path)


def build_high_barrier(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    canvas = builder.add_material("luggage_canvas", (1.0, 1.0, 1.0, 1.0), roughness=0.9, texture=("canvas", "T_Luggage"))
    leather = builder.add_material("leather", (0.32, 0.16, 0.10, 1.0), roughness=0.72)
    metal = builder.add_material("post_metal", (0.35, 0.38, 0.42, 1.0), metallic=0.6, roughness=0.45)
    strap = builder.add_material("strap", (0.93, 0.72, 0.25, 1.0), roughness=0.6)
    root = builder.add_node("LuggageStack", scene_root=True)
    for x in (-0.94, 0.94):
        builder.add_box("Post%+.2f" % x, (0.10, 2.15, 0.14), metal, position=(x, 1.07, 0.0), parent=root)
        builder.add_box("PostFoot%+.2f" % x, (0.30, 0.08, 0.40), metal, position=(x, 0.04, 0.0), parent=root)
    builder.add_box("TopBeam", (1.98, 0.12, 0.18), metal, position=(0.0, 2.10, 0.0), parent=root)
    for i, (x, y, z, sx, sy, sz) in enumerate([
        (-0.48, 1.42, 0.0, 0.72, 0.50, 0.48),
        (0.28, 1.38, 0.02, 0.80, 0.46, 0.52),
        (-0.18, 1.90, 0.0, 0.92, 0.40, 0.50),
        (0.62, 1.86, -0.04, 0.52, 0.42, 0.44),
    ]):
        material = canvas if i % 2 == 0 else leather
        builder.add_box("Case%d" % i, (sx, sy, sz), material, position=(x, y, z), rotation=(0.0, (i - 1.5) * 5.0, 0.0), parent=root)
        builder.add_box("Strap%d" % i, (sx * 0.18, sy * 1.02, sz * 1.02), strap, position=(x, y, z), rotation=(0.0, (i - 1.5) * 5.0, 0.0), parent=root)
    builder.save(path)


def build_coin(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    gold = builder.add_material("coin_gold", (1.0, 0.80, 0.22, 1.0), metallic=0.85, roughness=0.24)
    rim = builder.add_material("coin_rim", (0.92, 0.62, 0.10, 1.0), metallic=0.9, roughness=0.2)
    highlight = builder.add_material("coin_highlight", (1.0, 0.94, 0.62, 1.0), emission=(0.20, 0.16, 0.03), roughness=0.3)
    root = builder.add_node("Coin", scene_root=True)
    builder.add_cylinder("CoinFace", 0.40, 0.10, gold, segments=28, rotation=(90.0, 0.0, 0.0), parent=root)
    builder.add_cylinder("OuterRim", 0.43, 0.06, rim, segments=28, rotation=(90.0, 0.0, 0.0), parent=root)
    builder.add_cylinder("InnerRim", 0.34, 0.07, highlight, segments=28, rotation=(90.0, 0.0, 0.0), parent=root)
    for z in (-0.06, 0.06):
        builder.add_box("RupeeStem%+.2f" % z, (0.07, 0.30, 0.02), rim, position=(0.02, 0.0, z), parent=root)
        builder.add_box("RupeeTop%+.2f" % z, (0.22, 0.07, 0.02), rim, position=(-0.05, 0.12, z), parent=root)
        builder.add_box("RupeeMid%+.2f" % z, (0.18, 0.06, 0.02), rim, position=(-0.03, 0.00, z), parent=root)
        builder.add_box("RupeeLeg%+.2f" % z, (0.06, 0.18, 0.02), rim, position=(-0.12, -0.10, z), rotation=(0.0, 0.0, 18.0), parent=root)
    builder.save(path)


def build_powerup(path, kind):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    if kind == "shield":
        blue = builder.add_material("shield_blue", (0.12, 0.58, 0.94, 1.0), metallic=0.35, roughness=0.35)
        silver = builder.add_material("shield_silver", (0.86, 0.90, 0.95, 1.0), metallic=0.85, roughness=0.22)
        gold = builder.add_material("shield_gold", (1.0, 0.76, 0.20, 1.0), metallic=0.7, roughness=0.3)
        root = builder.add_node("ShieldPowerup", scene_root=True)
        builder.add_box("ShieldBody", (0.64, 0.78, 0.12), blue, position=(0.0, 0.0, 0.0), parent=root)
        builder.add_box("ShieldPoint", (0.52, 0.30, 0.12), blue, position=(0.0, -0.45, 0.0), rotation=(0.0, 0.0, 45.0), parent=root)
        builder.add_box("RimLeft", (0.08, 0.86, 0.14), silver, position=(-0.32, 0.0, 0.0), parent=root)
        builder.add_box("RimRight", (0.08, 0.86, 0.14), silver, position=(0.32, 0.0, 0.0), parent=root)
        builder.add_box("StarVertical", (0.08, 0.38, 0.15), gold, position=(0.0, 0.08, 0.0), parent=root)
        builder.add_box("StarHorizontal", (0.34, 0.08, 0.15), gold, position=(0.0, 0.08, 0.0), parent=root)
    elif kind == "magnet":
        red = builder.add_material("magnet_red", (0.90, 0.16, 0.22, 1.0), metallic=0.35, roughness=0.35)
        silver = builder.add_material("magnet_pole", (0.86, 0.89, 0.93, 1.0), metallic=0.9, roughness=0.2)
        root = builder.add_node("MagnetPowerup", scene_root=True)
        builder.add_box("Bridge", (0.60, 0.18, 0.22), red, position=(0.0, 0.34, 0.0), parent=root)
        for x in (-0.22, 0.22):
            builder.add_box("Arm%+.2f" % x, (0.18, 0.62, 0.22), red, position=(x, 0.03, 0.0), parent=root)
            builder.add_box("Pole%+.2f" % x, (0.20, 0.14, 0.24), silver, position=(x, -0.32, 0.0), parent=root)
        builder.add_box("MagnetGlow", (0.36, 0.08, 0.24), silver, position=(0.0, 0.35, 0.0), parent=root)
    else:
        amber = builder.add_material("boost_amber", (1.0, 0.62, 0.12, 1.0), emission=(0.25, 0.12, 0.01), roughness=0.36)
        cream = builder.add_material("boost_cream", (1.0, 0.92, 0.68, 1.0), roughness=0.5)
        dark = builder.add_material("boost_dark", (0.18, 0.12, 0.06, 1.0), roughness=0.7)
        root = builder.add_node("BoostPowerup", scene_root=True)
        builder.add_cylinder("Badge", 0.40, 0.12, dark, segments=24, rotation=(90.0, 0.0, 0.0), parent=root)
        builder.add_cylinder("BadgeFace", 0.34, 0.13, amber, segments=24, rotation=(90.0, 0.0, 0.0), parent=root)
        builder.add_box("ArrowStem", (0.10, 0.44, 0.15), cream, position=(0.0, -0.03, 0.0), parent=root)
        builder.add_box("ArrowHeadA", (0.30, 0.10, 0.15), cream, position=(-0.10, 0.19, 0.0), rotation=(0.0, 0.0, 45.0), parent=root)
        builder.add_box("ArrowHeadB", (0.30, 0.10, 0.15), cream, position=(0.10, 0.19, 0.0), rotation=(0.0, 0.0, -45.0), parent=root)
    builder.save(path)


def build_track(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    ballast = builder.add_material("ballast", (1.0, 1.0, 1.0, 1.0), roughness=0.96, texture=("ballast", "T_Ballast"))
    sleeper = builder.add_material("sleeper", (1.0, 1.0, 1.0, 1.0), roughness=0.92, texture=("wood", "T_Sleeper"))
    rail = builder.add_material("rail", (1.0, 1.0, 1.0, 1.0), metallic=0.72, roughness=0.4, texture=("rail", "T_Rail"))
    concrete = builder.add_material("track_concrete", (0.54, 0.53, 0.49, 1.0), roughness=0.9)
    pole = builder.add_material("ohe_pole", (0.32, 0.36, 0.40, 1.0), metallic=0.55, roughness=0.55)
    wire = builder.add_material("ohe_wire", (0.18, 0.18, 0.19, 1.0), metallic=0.8, roughness=0.35)
    signal_red = builder.add_material("signal_red", (0.95, 0.10, 0.08, 1.0), emission=(0.55, 0.02, 0.01), roughness=0.3)
    signal_green = builder.add_material("signal_green", (0.10, 0.78, 0.28, 1.0), emission=(0.02, 0.35, 0.08), roughness=0.3)
    root = builder.add_node("TrackKit", scene_root=True)
    builder.add_box("BallastBed", (8.40, 0.30, 36.0), ballast, position=(0.0, -0.15, 0.0), uv_tile=(2.0, 9.0), parent=root)
    builder.add_box("LeftDrain", (0.36, 0.14, 36.0), concrete, position=(-4.38, 0.02, 0.0), parent=root)
    builder.add_box("RightDrain", (0.36, 0.14, 36.0), concrete, position=(4.38, 0.02, 0.0), parent=root)
    for i in range(24):
        z = -17.0 + float(i) * 1.5
        builder.add_box("Sleeper%02d" % i, (7.75, 0.13, 0.42), sleeper, position=(0.0, 0.06, z), uv_tile=(4.0, 0.5), parent=root)
    for x in (-3.80, -1.30, 1.30, 3.80):
        builder.add_box("Rail%+.2f" % x, (0.14, 0.20, 36.0), rail, position=(x, 0.24, 0.0), uv_tile=(0.2, 9.0), parent=root)
        builder.add_box("RailFoot%+.2f" % x, (0.24, 0.06, 36.0), rail, position=(x, 0.13, 0.0), parent=root)
    for i, z in enumerate((-12.0, -2.0, 8.0, 18.0)):
        for side in (-1, 1):
            x = side * 5.15
            builder.add_cylinder("OHEPole%d%d" % (i, side), 0.10, 4.70, pole, position=(x, 2.35, z), segments=12, parent=root)
            builder.add_box("OHEArm%d%d" % (i, side), (1.50, 0.10, 0.12), pole, position=(x - side * 0.70, 4.25, z), parent=root)
            builder.add_box("OHEBracket%d%d" % (i, side), (0.10, 0.45, 0.10), pole, position=(x - side * 0.10, 4.05, z), parent=root)
        builder.add_box("ContactWire%d" % i, (0.05, 0.05, 10.0), wire, position=(0.0, 4.35, z + 5.0), parent=root)
    for i, z in enumerate((-9.0, 11.0)):
        builder.add_cylinder("SignalPost%d" % i, 0.08, 3.10, pole, position=(5.65, 1.55, z), segments=10, parent=root)
        builder.add_box("SignalBox%d" % i, (0.34, 0.86, 0.26), pole, position=(5.65, 3.00, z), parent=root)
        builder.add_sphere("SignalRed%d" % i, 0.10, signal_red, position=(5.65, 3.24, z - 0.15), parent=root)
        builder.add_sphere("SignalGreen%d" % i, 0.10, signal_green, position=(5.65, 2.82, z - 0.15), parent=root)
    builder.save(path)


def build_platform(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    tile = builder.add_material("platform_tile", (1.0, 1.0, 1.0, 1.0), roughness=0.92, texture=("tile", "T_PlatformTile"))
    edge = builder.add_material("platform_edge", (0.95, 0.72, 0.12, 1.0), roughness=0.72)
    concrete = builder.add_material("platform_concrete", (0.62, 0.61, 0.57, 1.0), roughness=0.88)
    metal = builder.add_material("canopy_metal", (0.28, 0.36, 0.44, 1.0), metallic=0.52, roughness=0.48)
    blue = builder.add_material("station_blue", (0.14, 0.36, 0.55, 1.0), roughness=0.68)
    lamp = builder.add_material("platform_lamp", (1.0, 0.90, 0.62, 1.0), emission=(0.52, 0.42, 0.20), roughness=0.26)
    sign = builder.add_material("sign_board", (0.10, 0.24, 0.36, 1.0), roughness=0.6)
    root = builder.add_node("PlatformKit", scene_root=True)
    for side in (-1, 1):
        x = side * 6.35
        builder.add_box("Platform%+.0f" % side, (3.20, 0.52, 36.0), tile, position=(x, 0.26, 0.0), uv_tile=(1.6, 9.0), parent=root)
        builder.add_box("EdgeStripe%+.0f" % side, (0.14, 0.06, 36.0), edge, position=(side * 4.75, 0.55, 0.0), parent=root)
        for i, z in enumerate((-14.0, -4.0, 6.0, 16.0)):
            builder.add_cylinder("CanopyColumn%d%d" % (i, side), 0.13, 3.10, concrete, position=(side * 6.90, 2.05, z), segments=12, parent=root)
            builder.add_box("CanopyBeam%d%d" % (i, side), (2.70, 0.12, 0.16), metal, position=(side * 6.10, 3.56, z), parent=root)
            builder.add_sphere("Lamp%d%d" % (i, side), 0.12, lamp, position=(side * 5.40, 3.42, z), parent=root)
        builder.add_box("CanopyRoof%+.0f" % side, (3.40, 0.16, 35.60), blue, position=(side * 6.45, 3.70, 0.0), uv_tile=(1.0, 9.0), parent=root)
        for i, z in enumerate((-8.0, 12.0)):
            builder.add_box("BenchSeat%d%d" % (i, side), (0.95, 0.10, 1.90), metal, position=(side * 6.25, 0.68, z), parent=root)
            builder.add_box("BenchBack%d%d" % (i, side), (0.10, 0.52, 1.90), metal, position=(side * 6.70, 0.95, z), parent=root)
            builder.add_box("BenchLeg%d%d" % (i, side), (0.08, 0.42, 0.08), metal, position=(side * 6.20, 0.43, z - 0.75), parent=root)
        builder.add_box("Sign%+.0f" % side, (0.10, 0.68, 2.60), sign, position=(side * 4.95, 2.30, -2.0), parent=root)
    builder.save(path)


def build_station(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    plaster = builder.add_material("station_plaster", (1.0, 1.0, 1.0, 1.0), roughness=0.88, texture=("plaster", "T_StationPlaster"))
    blue = builder.add_material("station_blue", (0.12, 0.32, 0.50, 1.0), roughness=0.72)
    glass = builder.add_material("station_glass", (0.10, 0.19, 0.25, 1.0), metallic=0.2, roughness=0.2)
    trim = builder.add_material("station_trim", (0.92, 0.78, 0.38, 1.0), metallic=0.3, roughness=0.48)
    dark = builder.add_material("station_dark", (0.12, 0.12, 0.14, 1.0), roughness=0.65)
    root = builder.add_node("StationKit", scene_root=True)
    builder.add_box("Facade", (3.0, 4.2, 18.0), plaster, position=(-8.20, 2.10, 0.0), uv_tile=(1.5, 4.5), parent=root)
    builder.add_box("BasePlinth", (3.20, 0.55, 18.20), dark, position=(-8.20, 0.28, 0.0), parent=root)
    builder.add_box("Cornice", (3.30, 0.28, 18.30), blue, position=(-8.20, 4.28, 0.0), parent=root)
    builder.add_box("Roof", (3.60, 0.34, 18.60), dark, position=(-8.20, 4.58, 0.0), parent=root)
    for i, z in enumerate((-6.6, -3.3, 0.0, 3.3, 6.6)):
        builder.add_box("Arch%d" % i, (0.12, 2.10, 1.70), blue, position=(-6.66, 1.72, z), parent=root)
        builder.add_box("ArchGlass%d" % i, (0.14, 1.42, 1.22), glass, position=(-6.58, 1.64, z), parent=root)
        builder.add_box("ArchTrim%d" % i, (0.16, 0.12, 1.86), trim, position=(-6.56, 2.82, z), parent=root)
        builder.add_cylinder("Pillar%d" % i, 0.16, 3.20, plaster, position=(-6.62, 1.60, z + 1.22), segments=12, parent=root)
    builder.add_box("Signboard", (0.16, 0.82, 5.20), blue, position=(-6.52, 3.62, 0.0), parent=root)
    builder.add_box("SignTrim", (0.18, 0.10, 5.30), trim, position=(-6.50, 4.06, 0.0), parent=root)
    builder.add_cylinder("ClockFace", 0.52, 0.14, trim, position=(-6.48, 3.60, -7.20), rotation=(0.0, 0.0, 90.0), segments=24, parent=root)
    builder.add_box("ClockHandA", (0.06, 0.42, 0.10), dark, position=(-6.40, 3.72, -7.20), parent=root)
    builder.add_box("ClockHandB", (0.30, 0.06, 0.10), dark, position=(-6.40, 3.60, -7.02), parent=root)
    builder.save(path)


def build_props(path):
    builder = GLBBuilder("Indian Subway Runner visual generator")
    canvas = builder.add_material("stall_canvas", (1.0, 1.0, 1.0, 1.0), roughness=0.9, texture=("canvas", "T_StallCanvas"))
    metal = builder.add_material("prop_metal", (0.38, 0.41, 0.44, 1.0), metallic=0.58, roughness=0.48)
    wood = builder.add_material("prop_wood", (1.0, 1.0, 1.0, 1.0), roughness=0.88, texture=("wood", "T_PropWood"))
    sign = builder.add_material("prop_sign", (0.94, 0.72, 0.16, 1.0), roughness=0.64)
    tea = builder.add_material("tea", (0.32, 0.16, 0.08, 1.0), roughness=0.7)
    root = builder.add_node("PropsKit", scene_root=True)
    builder.add_box("ChaiCounter", (1.45, 0.92, 2.10), canvas, position=(6.05, 0.72, -5.0), parent=root)
    builder.add_box("ChaiTop", (1.65, 0.12, 2.30), wood, position=(6.05, 1.24, -5.0), parent=root)
    builder.add_box("ChaiCanopy", (1.80, 0.10, 2.60), sign, position=(6.05, 2.25, -5.0), rotation=(0.0, 0.0, -4.0), parent=root)
    for x in (5.35, 6.75):
        builder.add_cylinder("StallPost%.2f" % x, 0.07, 2.10, metal, position=(x, 1.10, -5.0), segments=10, parent=root)
    for i, z in enumerate((-5.55, -5.0, -4.45)):
        builder.add_cylinder("TeaCup%d" % i, 0.11, 0.14, metal, position=(6.05, 1.38, z), segments=12, parent=root)
        builder.add_cylinder("TeaSurface%d" % i, 0.09, 0.02, tea, position=(6.05, 1.46, z), segments=12, parent=root)
    for i, (x, z) in enumerate(((5.95, 8.0), (6.45, 8.6), (5.55, 9.1))):
        builder.add_box("Suitcase%d" % i, (0.52, 0.38, 0.36), wood if i % 2 == 0 else canvas, position=(x, 0.72, z), rotation=(0.0, i * 12.0, 0.0), parent=root)
        builder.add_box("SuitcaseHandle%d" % i, (0.10, 0.14, 0.30), metal, position=(x, 0.95, z), parent=root)
    for i, z in enumerate((-12.0, 0.0, 12.0)):
        builder.add_cylinder("BinPost%d" % i, 0.06, 0.90, metal, position=(5.45, 0.50, z), segments=10, parent=root)
        builder.add_box("Signboard%d" % i, (0.08, 0.48, 1.30), sign, position=(5.45, 1.38, z), parent=root)
    builder.add_cylinder("WaterTank", 0.72, 1.10, metal, position=(-6.10, 3.65, 10.0), segments=16, parent=root)
    builder.add_box("WaterTankStand", (1.20, 0.12, 1.20), metal, position=(-6.10, 3.03, 10.0), parent=root)
    builder.save(path)


def main():
    root = Path(__file__).resolve().parents[3]
    output = root / "godot" / "assets" / "models"
    output.mkdir(parents=True, exist_ok=True)
    build_character(output / "SM_Character_PoliceRunner_Animated.glb", True)
    build_character(output / "SM_Character_Suspect_Animated.glb", False)
    build_train(output / "SM_Train_Express_Detailed.glb")
    build_low_barrier(output / "SM_Obstacle_PlatformBarrier_Low.glb")
    build_high_barrier(output / "SM_Obstacle_LuggageStack_High.glb")
    build_coin(output / "SM_Collectible_Coin_Polished.glb")
    for kind, name in (("shield", "SM_Powerup_Shield.glb"), ("magnet", "SM_Powerup_Magnet.glb"), ("boost", "SM_Powerup_Boost.glb")):
        build_powerup(output / name, kind)
    build_track(output / "SM_Track_Kit.glb")
    build_platform(output / "SM_Platform_Kit.glb")
    build_station(output / "SM_Station_Kit.glb")
    build_props(output / "SM_Props_RailwayKit.glb")
    print("generated 13 GLB assets in", output)


if __name__ == "__main__":
    main()
