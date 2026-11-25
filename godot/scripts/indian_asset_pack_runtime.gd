extends Node

## Optional runtime integration for the independently generated v0.1 starter pack.
## The main authored GLB kit remains the primary gameplay art. These imported assets
## add secondary Indian railway props and distant environment vignettes when present.

const PACK_ROOT := "res://assets/indian_pack_v01/"
const ENVIRONMENT_BLOCKS := [
	"environment/urban_station_block.glb",
	"environment/platform_market_block.glb",
	"environment/rural_railway_block.glb",
	"environment/rail_yard_block.glb"
]
const CHAI_STALL := "models/chai_stall_modular.glb"
const STATION_BENCH := "models/station_bench.glb"
const SUITCASE := "models/travel_suitcase.glb"

var _attached := false

func _process(_delta: float) -> void:
	if _attached:
		set_process(false)
		return
	var main := get_parent()
	if main == null:
		return
	var world := main.get_node_or_null("World") as Node3D
	if world == null:
		return

	var segments_found := 0
	for index in range(6):
		var segment := world.get_node_or_null("TrackSegment%d" % index) as Node3D
		if segment == null:
			continue
		segments_found += 1
		_add_segment_assets(segment, index)

	if segments_found > 0:
		_attached = true
		set_process(false)

func _add_segment_assets(segment: Node3D, index: int) -> void:
	# The source GLBs use Z-up authoring; rotate them into Godot's Y-up world.
	var environment_path := PACK_ROOT + ENVIRONMENT_BLOCKS[index % ENVIRONMENT_BLOCKS.size()]
	var background_side := -14.0 if index % 2 == 0 else 14.0
	_add_model(segment, environment_path, Vector3(background_side, 0.0, 0.0), -PI / 2.0, 0.82)

	match index % 3:
		0:
			var chai_side := -6.5 if index % 2 == 0 else 6.5
			_add_model(segment, PACK_ROOT + CHAI_STALL, Vector3(chai_side, 0.0, -8.0), -PI / 2.0, 0.9)
		1:
			var bench_side_a := 6.25 if index % 2 == 1 else -6.25
			_add_model(segment, PACK_ROOT + STATION_BENCH, Vector3(bench_side_a, 0.0, 1.5), -PI / 2.0, 1.0)
			_add_model(segment, PACK_ROOT + SUITCASE, Vector3(bench_side_a - 0.7, 0.0, -1.0), -PI / 2.0, 1.0)
		2:
			var bench_side_b := -6.25 if index % 2 == 0 else 6.25
			_add_model(segment, PACK_ROOT + STATION_BENCH, Vector3(bench_side_b, 0.0, -5.0), -PI / 2.0, 1.0)
			_add_model(segment, PACK_ROOT + SUITCASE, Vector3(bench_side_b + 0.75, 0.0, -2.8), -PI / 2.0, 1.0)

func _add_model(parent: Node3D, path: String, position: Vector3, rotation_x: float, scale_factor: float) -> void:
	if not ResourceLoader.exists(path):
		return
	var packed := load(path) as PackedScene
	if packed == null:
		push_warning("Indian asset pack resource could not be loaded: " + path)
		return
	var instance := packed.instantiate() as Node3D
	if instance == null:
		push_warning("Indian asset pack root is not Node3D: " + path)
		return
	instance.position = position
	instance.rotation.x = rotation_x
	instance.scale = Vector3.ONE * scale_factor
	parent.add_child(instance)
