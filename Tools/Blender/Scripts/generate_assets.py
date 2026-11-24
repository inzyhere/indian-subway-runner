import bpy
import os

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", ".."))
OUTPUT_DIR = os.path.join(ROOT, "Assets", "Resources", "Art", "Models")
os.makedirs(OUTPUT_DIR, exist_ok=True)


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)


def material(name, rgba):
    result = bpy.data.materials.get(name)
    if result is None:
        result = bpy.data.materials.new(name)
    result.diffuse_color = rgba
    result.use_nodes = True
    principled = result.node_tree.nodes.get("Principled BSDF")
    if principled is not None:
        principled.inputs["Base Color"].default_value = rgba
    return result


def finish(obj, name, location, scale, rotation, surface):
    obj.name = name
    obj.location = location
    obj.scale = scale
    if rotation is not None:
        obj.rotation_euler = rotation
    obj.data.materials.append(surface)
    return obj


def cube(name, location, scale, surface, rotation=None):
    bpy.ops.mesh.primitive_cube_add(location=location)
    return finish(bpy.context.object, name, location, scale, rotation, surface)


def sphere(name, location, scale, surface, rotation=None):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=location)
    return finish(bpy.context.object, name, location, scale, rotation, surface)


def cylinder(name, location, scale, surface, rotation=None):
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, location=location)
    return finish(bpy.context.object, name, location, scale, rotation, surface)


def export(name, objects):
    path = os.path.join(OUTPUT_DIR, name + ".obj")
    mtl_name = name + ".mtl"
    used_materials = []
    for obj in objects:
        for slot in obj.material_slots:
            if slot.material is not None and slot.material.name not in used_materials:
                used_materials.append(slot.material.name)

    with open(path, "w") as obj_file:
        obj_file.write("mtllib " + mtl_name + "\n")
        vertex_offset = 1
        for obj in objects:
            obj_file.write("o " + obj.name + "\n")
            mesh = obj.data
            for vertex in mesh.vertices:
                co = obj.matrix_world @ vertex.co
                obj_file.write("v {:.6f} {:.6f} {:.6f}\n".format(co.x, co.z, -co.y))
            names = [slot.material.name for slot in obj.material_slots if slot.material is not None]
            for polygon in mesh.polygons:
                if names:
                    obj_file.write("usemtl " + names[polygon.material_index % len(names)] + "\n")
                indices = [str(vertex_offset + index) for index in polygon.vertices]
                obj_file.write("f " + " ".join(indices) + "\n")
            vertex_offset += len(mesh.vertices)

    with open(os.path.join(OUTPUT_DIR, mtl_name), "w") as mtl_file:
        for material_name in used_materials:
            surface = bpy.data.materials.get(material_name)
            color = surface.diffuse_color
            mtl_file.write("newmtl " + material_name + "\n")
            mtl_file.write("Ka {:.6f} {:.6f} {:.6f}\n".format(color[0], color[1], color[2]))
            mtl_file.write("Kd {:.6f} {:.6f} {:.6f}\n".format(color[0], color[1], color[2]))
            mtl_file.write("Ks 0.100000 0.100000 0.100000\n")
            mtl_file.write("Ns 32.000000\n")

    print("Generated " + path)


def build(name, builder):
    clear_scene()
    objects = builder()
    export(name, objects)


def runner_builder():
    blue = material("RunnerBlue", (0.12, 0.42, 0.82, 1.0))
    skin = material("RunnerSkin", (0.86, 0.64, 0.44, 1.0))
    red = material("RunnerCap", (0.92, 0.18, 0.12, 1.0))
    dark = material("RunnerTrousers", (0.12, 0.15, 0.28, 1.0))
    return [
        cube("Torso", (0.0, 0.0, 1.05), (0.32, 0.2, 0.58), blue),
        sphere("Head", (0.0, 0.0, 1.82), (0.25, 0.25, 0.25), skin),
        cylinder("Cap", (0.0, 0.0, 2.02), (0.28, 0.28, 0.05), red),
        cube("LeftArm", (-0.42, 0.0, 1.08), (0.11, 0.11, 0.46), skin),
        cube("RightArm", (0.42, 0.0, 1.08), (0.11, 0.11, 0.46), skin),
        cube("LeftLeg", (-0.18, 0.0, 0.38), (0.13, 0.13, 0.52), dark),
        cube("RightLeg", (0.18, 0.0, 0.38), (0.13, 0.13, 0.52), dark)
    ]


def pursuer_builder():
    navy = material("PursuerNavy", (0.08, 0.12, 0.28, 1.0))
    skin = material("PursuerSkin", (0.78, 0.53, 0.36, 1.0))
    khaki = material("PursuerKhaki", (0.42, 0.36, 0.21, 1.0))
    black = material("PursuerBoots", (0.04, 0.04, 0.05, 1.0))
    return [
        cube("Torso", (0.0, 0.0, 1.12), (0.38, 0.22, 0.62), navy),
        sphere("Head", (0.0, 0.0, 1.94), (0.27, 0.27, 0.27), skin),
        cylinder("Cap", (0.0, 0.0, 2.16), (0.3, 0.3, 0.06), navy),
        cube("LeftArm", (-0.48, 0.0, 1.14), (0.13, 0.13, 0.5), navy),
        cube("RightArm", (0.48, 0.0, 1.14), (0.13, 0.13, 0.5), navy),
        cube("LeftLeg", (-0.2, 0.0, 0.4), (0.16, 0.16, 0.56), khaki),
        cube("RightLeg", (0.2, 0.0, 0.4), (0.16, 0.16, 0.56), khaki),
        cube("LeftBoot", (-0.2, -0.08, 0.08), (0.17, 0.24, 0.1), black),
        cube("RightBoot", (0.2, -0.08, 0.08), (0.17, 0.24, 0.1), black)
    ]


def express_builder():
    blue = material("ExpressBlue", (0.1, 0.36, 0.68, 1.0))
    cream = material("ExpressCream", (0.88, 0.84, 0.72, 1.0))
    glass = material("ExpressGlass", (0.05, 0.12, 0.18, 1.0))
    steel = material("ExpressSteel", (0.35, 0.37, 0.4, 1.0))
    objects = [
        cube("Body", (0.0, 0.0, 1.35), (1.1, 3.6, 1.2), blue),
        cube("Roof", (0.0, 0.0, 2.5), (1.02, 3.5, 0.12), cream),
        cube("Cab", (0.0, -2.15, 1.7), (1.05, 0.6, 0.9), cream),
        cube("Windshield", (0.0, -2.48, 1.85), (0.8, 0.04, 0.42), glass)
    ]
    for side in (-1.0, 1.0):
        for index in range(4):
            objects.append(cube("Window" + str(index) + str(side), (side * 1.11, -1.4 + index * 0.9, 1.72), (0.03, 0.42, 0.34), glass))
    for side in (-1.0, 1.0):
        for index in range(4):
            objects.append(cylinder("Wheel" + str(index) + str(side), (side * 1.02, -1.6 + index * 1.05, 0.35), (0.32, 0.12, 0.32), steel, (0.0, 1.5708, 0.0)))
    return objects


def low_barrier_builder():
    orange = material("BarrierOrange", (0.95, 0.38, 0.08, 1.0))
    white = material("BarrierWhite", (0.94, 0.94, 0.9, 1.0))
    return [
        cube("Base", (0.0, 0.0, 0.35), (1.0, 0.28, 0.28), orange),
        cube("Stripe", (0.0, -0.29, 0.35), (0.82, 0.03, 0.12), white),
        cube("Foot", (0.0, 0.0, 0.08), (1.15, 0.4, 0.08), orange)
    ]


def high_barrier_builder():
    red = material("BarrierRed", (0.78, 0.1, 0.12, 1.0))
    white = material("BarrierWhite", (0.94, 0.94, 0.9, 1.0))
    dark = material("BarrierPost", (0.1, 0.1, 0.12, 1.0))
    return [
        cube("Panel", (0.0, 0.0, 1.05), (1.1, 0.16, 0.86), red),
        cube("Stripe", (0.0, -0.17, 1.05), (0.9, 0.03, 0.16), white),
        cube("LeftPost", (-1.0, 0.0, 0.72), (0.12, 0.12, 0.74), dark),
        cube("RightPost", (1.0, 0.0, 0.72), (0.12, 0.12, 0.74), dark)
    ]


def coin_builder():
    gold = material("CoinGold", (1.0, 0.72, 0.08, 1.0))
    amber = material("CoinAmber", (0.86, 0.45, 0.04, 1.0))
    return [
        cylinder("Disc", (0.0, 0.0, 0.0), (0.34, 0.34, 0.045), gold, (1.5708, 0.0, 0.0)),
        cylinder("Inset", (0.0, -0.05, 0.0), (0.22, 0.22, 0.05), amber, (1.5708, 0.0, 0.0))
    ]


def shield_builder():
    green = material("PowerGreen", (0.12, 0.86, 0.6, 1.0))
    white = material("PowerWhite", (0.9, 0.96, 0.94, 1.0))
    return [
        sphere("Core", (0.0, 0.0, 0.0), (0.38, 0.38, 0.38), green),
        cylinder("Ring", (0.0, 0.0, 0.0), (0.52, 0.52, 0.04), white, (1.5708, 0.0, 0.0))
    ]


def magnet_builder():
    red = material("MagnetRed", (0.9, 0.12, 0.12, 1.0))
    steel = material("MagnetSteel", (0.72, 0.74, 0.78, 1.0))
    return [
        cube("LeftArm", (-0.3, 0.0, -0.2), (0.18, 0.2, 0.42), red),
        cube("RightArm", (0.3, 0.0, -0.2), (0.18, 0.2, 0.42), red),
        cube("Bridge", (0.0, 0.0, 0.2), (0.78, 0.2, 0.18), steel)
    ]


def multiplier_builder():
    gold = material("MultiplierGold", (1.0, 0.76, 0.1, 1.0))
    dark = material("MultiplierDark", (0.12, 0.1, 0.04, 1.0))
    return [
        cube("Stem", (0.0, 0.0, 0.0), (0.12, 0.12, 0.7), gold),
        cube("TopBar", (0.0, 0.0, 0.35), (0.7, 0.12, 0.12), dark),
        cube("BottomBar", (0.0, 0.0, -0.35), (0.7, 0.12, 0.12), dark)
    ]


def platform_builder():
    stone = material("PlatformStone", (0.72, 0.7, 0.62, 1.0))
    yellow = material("PlatformYellow", (0.95, 0.76, 0.12, 1.0))
    return [
        cube("Slab", (0.0, 0.0, 0.0), (2.4, 6.0, 0.22), stone),
        cube("SafetyLine", (-1.0, 0.0, 0.14), (0.08, 5.8, 0.03), yellow)
    ]


def station_builder():
    concrete = material("StationConcrete", (0.62, 0.64, 0.66, 1.0))
    green = material("StationGreen", (0.08, 0.32, 0.24, 1.0))
    objects = [
        cube("Roof", (0.0, 0.0, 3.0), (3.2, 7.0, 0.16), green),
        cube("BackWall", (1.6, 0.0, 1.5), (0.12, 7.0, 1.5), concrete)
    ]
    for index in range(5):
        objects.append(cube("Pillar" + str(index), (-1.45, -2.8 + index * 1.4, 1.5), (0.12, 0.12, 1.5), concrete))
    return objects


build("SM_Characters_Runner_001", runner_builder)
build("SM_Characters_Pursuer_001", pursuer_builder)
build("SM_Trains_Express_001", express_builder)
build("SM_Obstacles_LowBarrier_001", low_barrier_builder)
build("SM_Obstacles_HighBarrier_001", high_barrier_builder)
build("SM_Collectibles_Coin_001", coin_builder)
build("SM_PowerUps_Shield_001", shield_builder)
build("SM_PowerUps_Magnet_001", magnet_builder)
build("SM_PowerUps_Multiplier_001", multiplier_builder)
build("SM_Environment_Platform_001", platform_builder)
build("SM_Environment_Station_001", station_builder)

clear_scene()
print("Generated " + str(len([name for name in os.listdir(OUTPUT_DIR) if name.endswith(".obj")])) + " OBJ models in " + OUTPUT_DIR)
