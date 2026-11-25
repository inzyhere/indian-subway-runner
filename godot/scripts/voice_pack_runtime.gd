extends Node

## Temporary synthetic voice-pack integration. Replace these guide clips with
## licensed human recordings before release. One voice may play at a time.

const PACK_ROOT := "res://assets/indian_pack_v01/audio/"
const POLICE_CALL := "police_call_hindi.wav"
const CRIMINAL_TAUNT := "criminal_taunt_hindi.wav"
const ANNOUNCEMENTS := [
	"announcement_platform_hindi.wav",
	"announcement_platform_english.wav",
	"announcement_train_hindi.wav"
]

var _player: AudioStreamPlayer
var _last_running := false
var _last_chapter := 1
var _taunt_timer := 34.0
var _cooldown := 0.0
var _rng := RandomNumberGenerator.new()

func _ready() -> void:
	_rng.randomize()
	_last_chapter = int(get_parent().get("_chapter"))
	_player = AudioStreamPlayer.new()
	_player.name = "OptionalVoicePack"
	_player.volume_db = -9.0
	add_child(_player)

func _process(delta: float) -> void:
	_cooldown = maxf(0.0, _cooldown - delta)
	var main := get_parent()
	if main == null:
		return
	var running := bool(main.get("_running"))
	var chapter := int(main.get("_chapter"))

	if running and not _last_running:
		_taunt_timer = 34.0
		_play_clip(POLICE_CALL)
	elif running and chapter != _last_chapter:
		var announcement_index := posmod(chapter - 2, ANNOUNCEMENTS.size())
		_play_clip(String(ANNOUNCEMENTS[announcement_index]))

	if running:
		_taunt_timer -= delta
		if _taunt_timer <= 0.0:
			_play_clip(CRIMINAL_TAUNT)
			_taunt_timer = _rng.randf_range(38.0, 52.0)
	elif _last_running and _player.playing:
		_player.stop()

	_last_running = running
	_last_chapter = chapter

func _play_clip(filename: String) -> void:
	if _player == null or _player.playing or _cooldown > 0.0:
		return
	var path := PACK_ROOT + filename
	if not ResourceLoader.exists(path):
		return
	var stream := load(path) as AudioStream
	if stream == null:
		return
	_player.stream = stream
	_player.play()
	_cooldown = 3.0
