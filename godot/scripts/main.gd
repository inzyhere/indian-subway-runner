extends Node3D

const LANES = [-2.5, 0.0, 2.5]
const TRACK_LENGTH := 36.0
const TRACK_COUNT := 6
const START_SPEED := 15.0
const MAX_SPEED := 28.0
const PLAYER_Y := 0.65

var _player: Node3D
var _player_body: Node3D
var _pursuer: Node3D
var _camera: Camera3D
var _dynamic: Node3D
var _music: AudioStreamPlayer
var _sfx: AudioStreamPlayer
var _hud: CanvasLayer
var _score_label: Label
var _gap_label: Label
var _coin_label: Label
var _status_label: Label
var _start_button: Button
var _track_segments: Array[Node3D] = []
var _obstacles: Array[Dictionary] = []
var _coins: Array[Dictionary] = []
var _lane: int = 1
var _jump_y: float = 0.0
var _jump_velocity: float = 0.0
var _slide_timer: float = 0.0
var _invulnerability: float = 0.0
var _speed: float = START_SPEED
var _score: float = 0.0
var _coin_count: int = 0
var _gap: float = 20.0
var _spawn_timer: float = 1.0
var _running: bool = false
var _rng := RandomNumberGenerator.new()
var _touch_start := Vector2.ZERO

func _ready() -> void:
	randomize()
	_rng.seed = randi()
	_build_world()
	_build_hud()
	_reset_run(false)

func _process(delta: float) -> void:
	if not _running:
		return
	_invulnerability = max(0.0, _invulnerability - delta)
	_slide_timer = max(0.0, _slide_timer - delta)
	_speed = min(MAX_SPEED, _speed + delta * 0.35)
	_score += delta * _speed * 0.55
	_gap = min(24.0, _gap + delta * 0.18)
	_update_player(delta)
	_update_world(delta)
	_check_collisions()
	_update_hud()
	if _gap <= 0.0:
		_end_run()

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and not event.echo:
		var key_event := event as InputEventKey
		match key_event.keycode:
			KEY_LEFT, KEY_A:
				_move_lane(-1)
			KEY_RIGHT, KEY_D:
				_move_lane(1)
			KEY_SPACE, KEY_UP, KEY_W:
				if _running:
					_jump()
				else:
					_start_run()
			KEY_SHIFT, KEY_DOWN, KEY_S:
				_slide()
			KEY_ENTER, KEY_R:
				_start_run()
	elif event is InputEventMouseButton and event.pressed:
		var mouse_event := event as InputEventMouseButton
		if mouse_event.button_index == MOUSE_BUTTON_LEFT and not _running:
			_start_run()
	elif event is InputEventScreenTouch:
		var touch_event := event as InputEventScreenTouch
		if touch_event.pressed:
			_touch_start = touch_event.position
		else:
			var delta := touch_event.position - _touch_start
			if delta.length() > 30.0:
				if absf(delta.x) > absf(delta.y):
					_move_lane(1 if delta.x > 0.0 else -1)
				elif delta.y < 0.0:
					_jump()
				else:
					_slide()
			elif not _running:
				_start_run()

func _build_world() -> void:
	var world := Node3D.new()
	world.name = "World"
	add_child(world)

	var environment := WorldEnvironment.new()
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	env.background_color = Color(0.02, 0.03, 0.06, 1.0)
	env.ambient_light_color = Color(0.45, 0.52, 0.65, 1.0)
	env.ambient_light_energy = 0.8
	environment.environment = env
	world.add_child(environment)

	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-48.0, -28.0, 0.0)
	sun.light_energy = 1.7
	world.add_child(sun)

	var track_material := _make_material(Color(0.22, 0.25, 0.30, 1.0))
	var rail_material := _make_material(Color(0.95, 0.62, 0.12, 1.0))
	var station_material := _make_material(Color(0.12, 0.38, 0.55, 1.0))

	for i in range(TRACK_COUNT):
		var segment := Node3D.new()
		segment.position.z = -144.0 + float(i) * TRACK_LENGTH
		world.add_child(segment)
		_track_segments.append(segment)

		var floor_mesh := _make_box(Vector3(8.4, 0.3, TRACK_LENGTH), track_material)
		floor_mesh.position.y = -0.15
		segment.add_child(floor_mesh)

		for side in [-1.0, 1.0]:
			var rail := _make_box(Vector3(0.24, 0.55, TRACK_LENGTH), rail_material)
			rail.position = Vector3(side * 4.05, 0.28, 0.0)
			segment.add_child(rail)

		for offset in [-5.4, 5.4]:
			var station := _make_box(Vector3(2.6, 2.4, 5.0), station_material)
			station.position = Vector3(offset, 1.2, -6.0)
			segment.add_child(station)

	_player = Node3D.new()
	_player.name = "Player"
	_player.position = Vector3(0.0, PLAYER_Y, 0.0)
	add_child(_player)
	_player_body = _build_character("res://assets/models/SM_Characters_Runner_001.obj", Color(0.10, 0.35, 0.95, 1.0))
	_player.add_child(_player_body)

	_pursuer = _build_character("res://assets/models/SM_Characters_Pursuer_001.obj", Color(0.95, 0.15, 0.12, 1.0))
	_pursuer.name = "Pursuer"
	_pursuer.position = Vector3(0.0, PLAYER_Y, 9.0)
	add_child(_pursuer)

	_camera = Camera3D.new()
	_camera.name = "Camera"
	_camera.position = Vector3(0.0, 5.2, 9.5)
	_camera.look_at(Vector3(0.0, 1.1, -9.0), Vector3.UP)
	add_child(_camera)

	_dynamic = Node3D.new()
	_dynamic.name = "Dynamic"
	add_child(_dynamic)

	_music = AudioStreamPlayer.new()
	_music.name = "Music"
	_music.volume_db = -14.0
	var music_stream := load("res://assets/audio/music/ISR_Loop_Main_001.wav") as AudioStream
	if music_stream != null:
		if music_stream is AudioStreamWAV:
			(music_stream as AudioStreamWAV).loop_mode = AudioStreamWAV.LOOP_FORWARD
		_music.stream = music_stream
	add_child(_music)

	_sfx = AudioStreamPlayer.new()
	_sfx.name = "Sfx"
	_sfx.volume_db = -8.0
	add_child(_sfx)

func _build_hud() -> void:
	_hud = CanvasLayer.new()
	_hud.name = "Hud"
	add_child(_hud)

	var margin := MarginContainer.new()
	margin.set_anchors_preset(Control.PRESET_FULL_RECT)
	margin.add_theme_constant_override("margin_left", 24)
	margin.add_theme_constant_override("margin_top", 20)
	margin.add_theme_constant_override("margin_right", 24)
	margin.add_theme_constant_override("margin_bottom", 20)
	_hud.add_child(margin)

	var vbox := VBoxContainer.new()
	vbox.add_theme_constant_override("separation", 8)
	margin.add_child(vbox)

	_score_label = _make_label("Score: 0")
	_gap_label = _make_label("Chase gap: 20.0 m")
	_coin_label = _make_label("Coins: 0")
	_status_label = _make_label("Swipe, use arrows, or press Enter to start.")

	vbox.add_child(_score_label)
	vbox.add_child(_gap_label)
	vbox.add_child(_coin_label)
	vbox.add_child(_status_label)

	_start_button = Button.new()
	_start_button.text = "START RUN"
	_start_button.custom_minimum_size = Vector2(220.0, 52.0)
	_start_button.pressed.connect(_start_run)
	vbox.add_child(_start_button)

func _make_label(text: String) -> Label:
	var label := Label.new()
	label.text = text
	label.mouse_filter = Control.MOUSE_FILTER_IGNORE
	return label

func _make_material(color: Color) -> StandardMaterial3D:
	var material := StandardMaterial3D.new()
	material.albedo_color = color
	material.roughness = 0.72
	material.metallic = 0.05
	return material

func _make_box(size: Vector3, material: Material) -> MeshInstance3D:
	var mesh_instance := MeshInstance3D.new()
	var box := BoxMesh.new()
	box.size = size
	mesh_instance.mesh = box
	mesh_instance.material = material
	return mesh_instance

func _build_character(path: String, color: Color) -> Node3D:
	var root := Node3D.new()
	var loaded = load(path)
	var mesh := loaded as Mesh
	if mesh != null:
		var model := MeshInstance3D.new()
		model.mesh = mesh
		model.scale = Vector3(0.75, 0.75, 0.75)
		model.material = _make_material(color)
		root.add_child(model)
	else:
		var body := MeshInstance3D.new()
		var capsule := CapsuleMesh.new()
		capsule.radius = 0.42
		capsule.height = 1.5
		body.mesh = capsule
		body.material = _make_material(color)
		root.add_child(body)
	return root

func _reset_run(auto_start: bool) -> void:
	for child in _dynamic.get_children():
		child.queue_free()
	_obstacles.clear()
	_coins.clear()

	for i in range(_track_segments.size()):
		_track_segments[i].position.z = -144.0 + float(i) * TRACK_LENGTH

	_lane = 1
	_jump_y = 0.0
	_jump_velocity = 0.0
	_slide_timer = 0.0
	_invulnerability = 0.0
	_speed = START_SPEED
	_score = 0.0
	_coin_count = 0
	_gap = 20.0
	_spawn_timer = 1.1
	_player.position = Vector3(LANES[_lane], PLAYER_Y, 0.0)
	_player_body.scale = Vector3.ONE
	_pursuer.position = Vector3(0.0, PLAYER_Y, 9.0)
	_update_hud()

	_running = auto_start
	_start_button.text = "START RUN"
	_start_button.visible = not auto_start
	_status_label.text = "Swipe, use arrows, or press Enter to start."
	if auto_start and _music.stream != null and not _music.playing:
		_music.play()

func _start_run() -> void:
	_play_sfx("res://assets/audio/sfx/ISR_SFX_UiClick_001.wav")
	_reset_run(true)
	_status_label.text = "Run! Jump low barriers, slide under high barriers."

func _end_run() -> void:
	if not _running:
		return
	_running = false
	_gap = 0.0
	_gap_label.text = "Chase gap: 0.0 m"
	_status_label.text = "Caught! The criminal escaped this time. Restart to try again."
	_start_button.text = "RESTART RUN"
	_start_button.visible = true
	_music.stop()
	_play_sfx("res://assets/audio/sfx/ISR_SFX_Crash_001.wav")

func _move_lane(direction: int) -> void:
	if not _running:
		return
	_lane = clampi(_lane + direction, 0, LANES.size() - 1)

func _jump() -> void:
	if not _running or _jump_y > 0.0 or _jump_velocity > 0.0:
		return
	_jump_velocity = 9.5
	_play_sfx("res://assets/audio/sfx/ISR_SFX_Jump_001.wav")

func _slide() -> void:
	if not _running:
		return
	_slide_timer = 0.75
	_play_sfx("res://assets/audio/sfx/ISR_SFX_Slide_001.wav")

func _update_player(delta: float) -> void:
	_player.position.x = lerpf(_player.position.x, LANES[_lane], min(1.0, delta * 14.0))
	if _jump_y > 0.0 or _jump_velocity > 0.0:
		_jump_velocity -= 34.0 * delta
		_jump_y += _jump_velocity * delta
		if _jump_y <= 0.0:
			_jump_y = 0.0
			_jump_velocity = 0.0

	var sliding := _slide_timer > 0.0
	_player.position.y = PLAYER_Y + _jump_y - (0.28 if sliding else 0.0)
	var target_scale := Vector3(1.0, 0.55, 1.0) if sliding else Vector3.ONE
	_player_body.scale = _player_body.scale.lerp(target_scale, min(1.0, delta * 16.0))
	_pursuer.position.z = 9.0 - (20.0 - _gap) * 0.18

func _update_world(delta: float) -> void:
	for segment in _track_segments:
		segment.position.z += _speed * delta
		if segment.position.z > 36.0:
			segment.position.z -= TRACK_LENGTH * float(_track_segments.size())

	for i in range(_obstacles.size() - 1, -1, -1):
		var node := _obstacles[i]["node"] as Node3D
		node.position.z += _speed * delta
		if node.position.z > 18.0:
			node.queue_free()
			_obstacles.remove_at(i)

	for i in range(_coins.size() - 1, -1, -1):
		var node := _coins[i]["node"] as Node3D
		node.position.z += _speed * delta
		node.rotation_degrees.y += delta * 180.0
		if node.position.z > 18.0:
			node.queue_free()
			_coins.remove_at(i)

	_spawn_timer -= delta
	if _spawn_timer <= 0.0:
		_spawn_timer = _rng.randf_range(0.55, 1.05)
		if _rng.randf() < 0.64:
			_spawn_obstacle()
		else:
			_spawn_coin()

func _spawn_obstacle() -> void:
	var lane := _rng.randi_range(0, LANES.size() - 1)
	var kind := "low" if _rng.randf() < 0.55 else "high"
	var node := Node3D.new()
	node.position = Vector3(LANES[lane], 0.0, -72.0)

	var color := Color(0.95, 0.45, 0.10, 1.0) if kind == "low" else Color(0.85, 0.12, 0.45, 1.0)
	var model_path := "res://assets/models/SM_Obstacles_LowBarrier_001.obj" if kind == "low" else "res://assets/models/SM_Obstacles_HighBarrier_001.obj"
	var loaded = load(model_path)
	var mesh := loaded as Mesh
	var visual: MeshInstance3D
	if mesh != null:
		visual = MeshInstance3D.new()
		visual.mesh = mesh
		visual.scale = Vector3(0.75, 0.75, 0.75)
	else:
		visual = _make_box(Vector3(1.9, 0.85, 0.7), _make_material(color))
	visual.material = _make_material(color)
	visual.position.y = 0.43 if kind == "low" else 1.55
	node.add_child(visual)

	_dynamic.add_child(node)
	_obstacles.append({"node": node, "lane": lane, "kind": kind, "hit": false})

func _spawn_coin() -> void:
	var lane := _rng.randi_range(0, LANES.size() - 1)
	var node := Node3D.new()
	node.position = Vector3(LANES[lane], 0.95, -72.0)

	var loaded = load("res://assets/models/SM_Collectibles_Coin_001.obj")
	var mesh := loaded as Mesh
	if mesh != null:
		var model := MeshInstance3D.new()
		model.mesh = mesh
		model.scale = Vector3(0.55, 0.55, 0.55)
		model.material = _make_material(Color(1.0, 0.82, 0.18, 1.0))
		node.add_child(model)
	else:
		var coin := MeshInstance3D.new()
		var cylinder := CylinderMesh.new()
		cylinder.top_radius = 0.38
		cylinder.bottom_radius = 0.38
		cylinder.height = 0.12
		coin.mesh = cylinder
		coin.rotation_degrees = Vector3(90.0, 0.0, 0.0)
		coin.material = _make_material(Color(1.0, 0.82, 0.18, 1.0))
		node.add_child(coin)

	_dynamic.add_child(node)
	_coins.append({"node": node, "lane": lane})

func _check_collisions() -> void:
	for i in range(_coins.size() - 1, -1, -1):
		var entry := _coins[i]
		var node := entry["node"] as Node3D
		if entry["lane"] == _lane and absf(node.position.z) < 1.3:
			_coin_count += 1
			_score += 75.0
			_play_sfx("res://assets/audio/sfx/ISR_SFX_Coin_001.wav")
			node.queue_free()
			_coins.remove_at(i)

	for entry in _obstacles:
		var node := entry["node"] as Node3D
		if entry["lane"] != _lane or absf(node.position.z) > 1.25 or bool(entry["hit"]):
			continue
		var cleared := false
		if String(entry["kind"]) == "low":
			cleared = _jump_y > 0.75
		else:
			cleared = _slide_timer > 0.0
		if not cleared:
			entry["hit"] = true
			_hit_obstacle()

func _hit_obstacle() -> void:
	if _invulnerability > 0.0:
		return
	_invulnerability = 1.0
	_gap -= 5.5
	_score = max(0.0, _score - 40.0)
	_play_sfx("res://assets/audio/sfx/ISR_SFX_Crash_001.wav")
	if _gap <= 0.0:
		_end_run()

func _update_hud() -> void:
	_score_label.text = "Score: %d" % int(_score)
	_gap_label.text = "Chase gap: %.1f m" % max(0.0, _gap)
	_coin_label.text = "Coins: %d" % _coin_count

func _play_sfx(path: String) -> void:
	var stream := load(path) as AudioStream
	if stream != null:
		_sfx.stream = stream
		_sfx.play()
