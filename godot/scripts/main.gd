extends Node3D

const LANES = [-2.5, 0.0, 2.5]
const TRACK_LENGTH := 36.0
const TRACK_COUNT := 6
const START_SPEED := 15.0
const MAX_SPEED := 28.0
const PLAYER_Y := 0.65
const CHAPTER_SCORE_STEP := 450.0
const CHAPTERS = [
	{"title": "PLATFORM 7", "line": "ACP: Ticket check, sab clear? Toh daud!", "hint": "Dodge the crowd and reach the next coach."},
	{"title": "EXPRESS COACH", "line": "Constable: Packet coach mein hai, saab!", "hint": "Trains are coming. Keep one lane open."},
	{"title": "CHAI CHOWK", "line": "Announcer: Agla station Chai Chowk. Mind the gap.", "hint": "Coin trails mark the safe path."},
	{"title": "NIGHT YARD", "line": "ACP: Andhera mat dekh,signal dekh!", "hint": "Use shields and boosts when the yard tightens."},
	{"title": "BRIDGE JUNCTION", "line": "ACP: Idhar se kahin nahi bachega!", "hint": "Stay sharp. The gap is closing fast."}
]
const POWERUP_TYPES = ["shield", "magnet", "boost"]
const POWERUP_MODELS = {
	"shield": "res://assets/models/SM_PowerUps_Shield_001.obj",
	"magnet": "res://assets/models/SM_PowerUps_Magnet_001.obj",
	"boost": "res://assets/models/SM_PowerUps_Multiplier_001.obj"
}

var _player: Node3D
var _player_body: Node3D
var _pursuer: Node3D
var _camera: Camera3D
var _camera_base := Vector3(0.0, 5.2, 9.5)
var _dynamic: Node3D
var _environment: Environment
var _music: AudioStreamPlayer
var _sfx: AudioStreamPlayer
var _hud: CanvasLayer
var _chapter_label: Label
var _score_label: Label
var _gap_label: Label
var _coin_label: Label
var _distance_label: Label
var _power_label: Label
var _status_label: Label
var _start_button: Button
var _story_panel: PanelContainer
var _story_title: Label
var _story_line: Label
var _story_hint: Label
var _story_button: Button
var _track_segments: Array[Node3D] = []
var _obstacles: Array[Dictionary] = []
var _coins: Array[Dictionary] = []
var _powerups: Array[Dictionary] = []
var _lane: int = 1
var _chapter: int = 1
var _jump_y: float = 0.0
var _jump_velocity: float = 0.0
var _slide_timer: float = 0.0
var _invulnerability: float = 0.0
var _shield_charges: int = 0
var _magnet_timer: float = 0.0
var _boost_timer: float = 0.0
var _camera_shake: float = 0.0
var _status_timer: float = 0.0
var _speed: float = START_SPEED
var _score: float = 0.0
var _distance: float = 0.0
var _coin_count: int = 0
var _gap: float = 20.0
var _next_chapter_score: float = CHAPTER_SCORE_STEP
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
	_show_story(1)

func _process(delta: float) -> void:
	if not _running:
		return
	var dt: float = min(delta, 0.05)
	_invulnerability = max(0.0, _invulnerability - dt)
	_slide_timer = max(0.0, _slide_timer - dt)
	_magnet_timer = max(0.0, _magnet_timer - dt)
	_boost_timer = max(0.0, _boost_timer - dt)
	_camera_shake = max(0.0, _camera_shake - dt)
	if _status_timer > 0.0:
		_status_timer -= dt
		if _status_timer <= 0.0:
			_status_label.text = "Swipe left/right, tap up to jump, tap down to slide."
	if _boost_timer > 0.0:
		_speed = min(MAX_SPEED + 6.0, _speed + dt * 1.4)
	else:
		if _speed > MAX_SPEED:
			_speed = max(MAX_SPEED, _speed - dt * 1.2)
		else:
			_speed = min(MAX_SPEED, _speed + dt * 0.35)
	_score += dt * _speed * 0.55
	_distance += dt * _speed
	_gap = min(24.0, _gap + dt * 0.18)
	if _score >= _next_chapter_score:
		_advance_chapter()
	_update_player(dt)
	_update_world(dt)
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
					_on_start_pressed()
			KEY_SHIFT, KEY_DOWN, KEY_S:
				_slide()
			KEY_ENTER, KEY_R:
				_on_start_pressed()
	elif event is InputEventMouseButton and event.pressed:
		var mouse_event := event as InputEventMouseButton
		if mouse_event.button_index == MOUSE_BUTTON_LEFT and not _running:
			_on_start_pressed()
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
				_on_start_pressed()

func _build_world() -> void:
	var world := Node3D.new()
	world.name = "World"
	add_child(world)

	var environment := WorldEnvironment.new()
	_environment = Environment.new()
	_environment.background_mode = Environment.BG_COLOR
	_environment.background_color = Color(0.02, 0.03, 0.06, 1.0)
	_environment.ambient_light_color = Color(0.45, 0.52, 0.65, 1.0)
	_environment.ambient_light_energy = 0.85
	environment.environment = _environment
	world.add_child(environment)

	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-48.0, -28.0, 0.0)
	sun.light_energy = 1.7
	world.add_child(sun)

	var track_material := _make_material(Color(0.22, 0.25, 0.30, 1.0))
	var rail_material := _make_material(Color(0.95, 0.62, 0.12, 1.0))
	var station_material := _make_material(Color(0.12, 0.38, 0.55, 1.0))
	var lamp_material := _make_material(Color(1.0, 0.90, 0.55, 1.0))

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
			var lamp := _make_box(Vector3(0.25, 0.25, 1.2), lamp_material)
			lamp.position = Vector3(offset * 0.72, 3.1, -7.0)
			segment.add_child(lamp)

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
	_camera.position = _camera_base
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
	margin.mouse_filter = Control.MOUSE_FILTER_IGNORE
	margin.add_theme_constant_override("margin_left", 24)
	margin.add_theme_constant_override("margin_top", 20)
	margin.add_theme_constant_override("margin_right", 24)
	margin.add_theme_constant_override("margin_bottom", 20)
	_hud.add_child(margin)

	var vbox := VBoxContainer.new()
	vbox.mouse_filter = Control.MOUSE_FILTER_IGNORE
	vbox.add_theme_constant_override("separation", 8)
	margin.add_child(vbox)

	_chapter_label = _make_label("Chapter 1 — PLATFORM 7")
	_score_label = _make_label("Score: 0")
	_gap_label = _make_label("Chase gap: 20.0 m")
	_coin_label = _make_label("Coins: 0")
	_distance_label = _make_label("Distance: 0 m")
	_power_label = _make_label("Power: none")
	_status_label = _make_label("Swipe, use arrows, or press Enter to start.")

	vbox.add_child(_chapter_label)
	vbox.add_child(_score_label)
	vbox.add_child(_gap_label)
	vbox.add_child(_coin_label)
	vbox.add_child(_distance_label)
	vbox.add_child(_power_label)
	vbox.add_child(_status_label)

	_start_button = Button.new()
	_start_button.text = "START CASE"
	_start_button.custom_minimum_size = Vector2(220.0, 52.0)
	_start_button.pressed.connect(_on_start_pressed)
	vbox.add_child(_start_button)

	var center := CenterContainer.new()
	center.set_anchors_preset(Control.PRESET_FULL_RECT)
	center.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_hud.add_child(center)

	_story_panel = PanelContainer.new()
	_story_panel.custom_minimum_size = Vector2(620.0, 260.0)
	_story_panel.mouse_filter = Control.MOUSE_FILTER_STOP
	center.add_child(_story_panel)

	var story_box := VBoxContainer.new()
	story_box.add_theme_constant_override("separation", 12)
	_story_panel.add_child(story_box)

	_story_title = _make_label("CASE FILE 01 — PLATFORM 7")
	_story_line = _make_label("ACP: Ticket check, sab clear? Toh daud!")
	_story_hint = _make_label("Dodge the crowd and reach the next coach.")
	_story_title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	_story_line.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	_story_hint.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	story_box.add_child(_story_title)
	story_box.add_child(_story_line)
	story_box.add_child(_story_hint)

	_story_button = Button.new()
	_story_button.text = "BEGIN CHASE"
	_story_button.custom_minimum_size = Vector2(220.0, 52.0)
	_story_button.pressed.connect(_begin_chase)
	story_box.add_child(_story_button)
	_story_panel.visible = false

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
	mesh_instance.material_override = material
	return mesh_instance

func _build_character(path: String, color: Color) -> Node3D:
	var root := Node3D.new()
	var loaded = load(path)
	var mesh := loaded as Mesh
	if mesh != null:
		var model := MeshInstance3D.new()
		model.mesh = mesh
		model.scale = Vector3(0.75, 0.75, 0.75)
		model.material_override = _make_material(color)
		root.add_child(model)
	else:
		var body := MeshInstance3D.new()
		var capsule := CapsuleMesh.new()
		capsule.radius = 0.42
		capsule.height = 1.5
		body.mesh = capsule
		body.material_override = _make_material(color)
		root.add_child(body)
	return root

func _reset_run(auto_start: bool) -> void:
	for child in _dynamic.get_children():
		child.queue_free()
	_obstacles.clear()
	_coins.clear()
	_powerups.clear()

	for i in range(_track_segments.size()):
		_track_segments[i].position.z = -144.0 + float(i) * TRACK_LENGTH

	_lane = 1
	_jump_y = 0.0
	_jump_velocity = 0.0
	_slide_timer = 0.0
	_invulnerability = 0.0
	_shield_charges = 0
	_magnet_timer = 0.0
	_boost_timer = 0.0
	_camera_shake = 0.0
	_status_timer = 0.0
	_speed = START_SPEED
	_score = 0.0
	_distance = 0.0
	_coin_count = 0
	_gap = 20.0
	_next_chapter_score = CHAPTER_SCORE_STEP
	_spawn_timer = 1.1
	_player.position = Vector3(LANES[_lane], PLAYER_Y, 0.0)
	_player_body.scale = Vector3.ONE
	_pursuer.position = Vector3(0.0, PLAYER_Y, 9.0)
	_update_hud()

	_running = auto_start
	_start_button.text = "START CASE" if _chapter == 1 else "CONTINUE CASE"
	_start_button.visible = not auto_start
	_status_label.text = "Swipe left/right, tap up to jump, tap down to slide."
	if auto_start:
		_story_panel.visible = false
		if _music.stream != null and not _music.playing:
			_music.play()
	else:
		_music.stop()

func _on_start_pressed() -> void:
	_play_sfx("res://assets/audio/sfx/ISR_SFX_UiClick_001.wav")
	if _story_panel.visible:
		_begin_chase()
		return
	_show_story(_chapter)

func _begin_chase() -> void:
	_play_sfx("res://assets/audio/sfx/ISR_SFX_UiClick_001.wav")
	_reset_run(true)
	_announce(String(_chapter_entry(_chapter)["line"]), 2.4)

func _show_story(chapter: int) -> void:
	var entry := _chapter_entry(chapter)
	_story_title.text = "CASE FILE %02d — %s" % [chapter, String(entry["title"])]
	_story_line.text = String(entry["line"])
	_story_hint.text = String(entry["hint"])
	_story_button.text = "BEGIN CHASE" if _score <= 0.0 else "CONTINUE CASE"
	_story_panel.visible = true
	_start_button.visible = false

func _chapter_entry(chapter: int) -> Dictionary:
	var index := (chapter - 1) % CHAPTERS.size()
	return CHAPTERS[index]

func _announce(text: String, duration: float) -> void:
	_status_label.text = text
	_status_timer = duration

func _advance_chapter() -> void:
	_chapter += 1
	_next_chapter_score += CHAPTER_SCORE_STEP
	var index := (_chapter - 1) % CHAPTERS.size()
	if index % 2 == 0:
		_environment.background_color = Color(0.03, 0.04, 0.09, 1.0)
		_environment.ambient_light_color = Color(0.48, 0.55, 0.70, 1.0)
	else:
		_environment.background_color = Color(0.04, 0.03, 0.07, 1.0)
		_environment.ambient_light_color = Color(0.58, 0.48, 0.62, 1.0)
	_play_sfx("res://assets/audio/sfx/ISR_SFX_PowerUp_001.wav")
	_announce("CHAPTER %d — %s" % [_chapter, String(_chapter_entry(_chapter)["title"])], 2.6)

func _end_run() -> void:
	if not _running:
		return
	_running = false
	_gap = 0.0
	_gap_label.text = "Chase gap: 0.0 m"
	_status_label.text = "Near capture! The criminal slipped away again."
	_music.stop()
	_play_sfx("res://assets/audio/sfx/ISR_SFX_Crash_001.wav")
	_show_story(_chapter)

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

	_camera.position = _camera_base
	if _camera_shake > 0.0:
		_camera.position += Vector3(
			_rng.randf_range(-0.16, 0.16),
			_rng.randf_range(-0.10, 0.10),
			0.0
		)

func _update_world(delta: float) -> void:
	for segment in _track_segments:
		segment.position.z += _speed * delta
		if segment.position.z > 36.0:
			segment.position.z -= TRACK_LENGTH * float(_track_segments.size())

	for i in range(_obstacles.size() - 1, -1, -1):
		var node := _obstacles[i]["node"] as Node3D
		node.position.z += _speed * delta
		if node.position.z > float(_obstacles[i]["half_length"]) + 16.0:
			node.queue_free()
			_obstacles.remove_at(i)

	for i in range(_coins.size() - 1, -1, -1):
		var node := _coins[i]["node"] as Node3D
		node.position.z += _speed * delta
		node.rotation_degrees.y += delta * 180.0
		if node.position.z > 18.0:
			node.queue_free()
			_coins.remove_at(i)

	for i in range(_powerups.size() - 1, -1, -1):
		var node := _powerups[i]["node"] as Node3D
		node.position.z += _speed * delta
		node.rotation_degrees.y += delta * 120.0
		if node.position.z > 18.0:
			node.queue_free()
			_powerups.remove_at(i)

	_spawn_timer -= delta
	if _spawn_timer <= 0.0:
		var interval: float = maxf(0.38, 0.98 - float(_chapter - 1) * 0.07 - _speed * 0.012)
		_spawn_timer = _rng.randf_range(interval * 0.82, interval * 1.18)
		var roll := _rng.randf()
		if roll < 0.08:
			_spawn_powerup()
		elif roll < 0.24:
			_spawn_coin_trail(_rng.randi_range(0, LANES.size() - 1))
		else:
			_spawn_wave()

func _spawn_wave() -> void:
	var safe_lane := _rng.randi_range(0, LANES.size() - 1)
	var blocked: Array[int] = [0, 1, 2]
	blocked.erase(safe_lane)
	blocked.shuffle()

	var block_count := 1
	if _chapter >= 3 and _rng.randf() < 0.55:
		block_count = 2

	for i in range(block_count):
		var kind := "low"
		if _chapter >= 3 and _rng.randf() < 0.28:
			kind = "train"
		elif _rng.randf() < 0.5:
			kind = "high"
		_spawn_obstacle(blocked[i], kind, -72.0 - float(i) * 8.0)

	if _rng.randf() < 0.45:
		_spawn_coin_trail(safe_lane)

func _spawn_obstacle(lane: int, kind: String, z: float) -> void:
	var node := Node3D.new()
	node.position = Vector3(LANES[lane], 0.0, z)

	var color := Color(0.95, 0.45, 0.10, 1.0)
	var model_path := "res://assets/models/SM_Obstacles_LowBarrier_001.obj"
	var model_scale := 0.75
	var visual_y := 0.43
	var half_length := 1.25
	var size := Vector3(1.9, 0.85, 0.7)
	if kind == "high":
		color = Color(0.85, 0.12, 0.45, 1.0)
		model_path = "res://assets/models/SM_Obstacles_HighBarrier_001.obj"
		visual_y = 1.55
	elif kind == "train":
		color = Color(0.10, 0.48, 0.62, 1.0)
		model_path = "res://assets/models/SM_Trains_Express_001.obj"
		model_scale = 0.6
		visual_y = 1.15
		half_length = 3.2
		size = Vector3(2.2, 2.4, 8.0)
		_play_sfx("res://assets/audio/sfx/ISR_SFX_TrainHorn_001.wav")

	var loaded = load(model_path)
	var mesh := loaded as Mesh
	var visual: MeshInstance3D
	if mesh != null:
		visual = MeshInstance3D.new()
		visual.mesh = mesh
		visual.scale = Vector3(model_scale, model_scale, model_scale)
	else:
		visual = _make_box(size, _make_material(color))
	visual.material_override = _make_material(color)
	visual.position.y = visual_y
	node.add_child(visual)

	_dynamic.add_child(node)
	_obstacles.append({"node": node, "lane": lane, "kind": kind, "hit": false, "half_length": half_length})

func _spawn_coin(lane: int, z: float) -> void:
	var node := Node3D.new()
	node.position = Vector3(LANES[lane], 0.95, z)

	var loaded = load("res://assets/models/SM_Collectibles_Coin_001.obj")
	var mesh := loaded as Mesh
	if mesh != null:
		var model := MeshInstance3D.new()
		model.mesh = mesh
		model.scale = Vector3(0.55, 0.55, 0.55)
		model.material_override = _make_material(Color(1.0, 0.82, 0.18, 1.0))
		node.add_child(model)
	else:
		var coin := MeshInstance3D.new()
		var cylinder := CylinderMesh.new()
		cylinder.top_radius = 0.38
		cylinder.bottom_radius = 0.38
		cylinder.height = 0.12
		coin.mesh = cylinder
		coin.rotation_degrees = Vector3(90.0, 0.0, 0.0)
		coin.material_override = _make_material(Color(1.0, 0.82, 0.18, 1.0))
		node.add_child(coin)

	_dynamic.add_child(node)
	_coins.append({"node": node, "lane": lane})

func _spawn_coin_trail(lane: int) -> void:
	for i in range(4):
		_spawn_coin(lane, -60.0 - float(i) * 4.5)

func _spawn_powerup() -> void:
	var lane := _rng.randi_range(0, LANES.size() - 1)
	var kind: String = String(POWERUP_TYPES[_rng.randi_range(0, POWERUP_TYPES.size() - 1)])
	var node := Node3D.new()
	node.position = Vector3(LANES[lane], 1.2, -70.0)

	var color := _powerup_color(kind)
	var loaded = load(String(POWERUP_MODELS[kind]))
	var mesh := loaded as Mesh
	var visual: MeshInstance3D
	if mesh != null:
		visual = MeshInstance3D.new()
		visual.mesh = mesh
		visual.scale = Vector3(0.6, 0.6, 0.6)
	else:
		visual = MeshInstance3D.new()
		var sphere := SphereMesh.new()
		sphere.radius = 0.45
		sphere.height = 0.9
		visual.mesh = sphere
	visual.material_override = _make_material(color)
	node.add_child(visual)

	_dynamic.add_child(node)
	_powerups.append({"node": node, "lane": lane, "kind": kind})

func _powerup_color(kind: String) -> Color:
	if kind == "shield":
		return Color(0.20, 0.85, 1.0, 1.0)
	if kind == "magnet":
		return Color(1.0, 0.35, 0.55, 1.0)
	return Color(0.95, 0.75, 0.15, 1.0)

func _check_collisions() -> void:
	for i in range(_coins.size() - 1, -1, -1):
		var entry := _coins[i]
		var node := entry["node"] as Node3D
		var magnet_catch := _magnet_timer > 0.0 and node.position.z > -20.0 and node.position.z < 8.0
		if (entry["lane"] == _lane and absf(node.position.z) < 1.3) or magnet_catch:
			_collect_coin(i)

	for i in range(_obstacles.size() - 1, -1, -1):
		var entry := _obstacles[i]
		var node := entry["node"] as Node3D
		if bool(entry["hit"]) or entry["lane"] != _lane:
			continue
		if absf(node.position.z) > float(entry["half_length"]):
			continue
		var kind := String(entry["kind"])
		var cleared := false
		if kind == "low":
			cleared = _jump_y > 0.75
		elif kind == "high":
			cleared = _slide_timer > 0.0
		if cleared:
			entry["hit"] = true
			_score += 35.0
			continue
		entry["hit"] = true
		_hit_obstacle()

	for i in range(_powerups.size() - 1, -1, -1):
		var entry := _powerups[i]
		var node := entry["node"] as Node3D
		if entry["lane"] == _lane and absf(node.position.z) < 1.4:
			_apply_powerup(String(entry["kind"]))
			node.queue_free()
			_powerups.remove_at(i)

func _collect_coin(index: int) -> void:
	var node := _coins[index]["node"] as Node3D
	_coin_count += 1
	_score += 75.0
	_play_sfx("res://assets/audio/sfx/ISR_SFX_Coin_001.wav")
	node.queue_free()
	_coins.remove_at(index)

func _apply_powerup(kind: String) -> void:
	if kind == "shield":
		_shield_charges = min(2, _shield_charges + 1)
		_announce("POWER-UP: SHIELD READY", 1.6)
	elif kind == "magnet":
		_magnet_timer = 9.0
		_announce("POWER-UP: COIN MAGNET", 1.6)
	else:
		_boost_timer = 5.0
		_announce("POWER-UP: SPRINT BOOST", 1.6)
	_score += 120.0
	_play_sfx("res://assets/audio/sfx/ISR_SFX_PowerUp_001.wav")

func _hit_obstacle() -> void:
	if _invulnerability > 0.0:
		return
	_invulnerability = 1.1
	_camera_shake = 0.35
	if _shield_charges > 0:
		_shield_charges -= 1
		_announce("SHIELD SAVED YOU!", 1.4)
		_play_sfx("res://assets/audio/sfx/ISR_SFX_PowerUp_001.wav")
		return
	_gap -= 4.5 + float(_chapter) * 0.3
	_score = max(0.0, _score - 40.0)
	_play_sfx("res://assets/audio/sfx/ISR_SFX_Crash_001.wav")
	_announce("ACP gained ground!", 1.2)
	if _gap <= 0.0:
		_end_run()

func _update_hud() -> void:
	_chapter_label.text = "Chapter %d — %s" % [_chapter, String(_chapter_entry(_chapter)["title"])]
	_score_label.text = "Score: %d" % int(_score)
	_gap_label.text = "Chase gap: %.1f m" % max(0.0, _gap)
	_coin_label.text = "Coins: %d" % _coin_count
	_distance_label.text = "Distance: %d m" % int(_distance)

	var parts: Array[String] = []
	if _shield_charges > 0:
		parts.append("SHIELD x%d" % _shield_charges)
	if _magnet_timer > 0.0:
		parts.append("MAGNET %.0fs" % _magnet_timer)
	if _boost_timer > 0.0:
		parts.append("BOOST %.0fs" % _boost_timer)
	var power_text := ""
	for part in parts:
		if power_text != "":
			power_text += " + "
		power_text += part
	if power_text == "":
		power_text = "none"
	_power_label.text = "Power: %s" % power_text

func _play_sfx(path: String) -> void:
	var stream := load(path) as AudioStream
	if stream != null:
		_sfx.stream = stream
		_sfx.play()
