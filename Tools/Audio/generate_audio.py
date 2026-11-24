import math
import os
import random
import struct
import wave

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
MUSIC_DIR = os.path.join(ROOT, "Assets", "Resources", "Audio", "Music")
SFX_DIR = os.path.join(ROOT, "Assets", "Resources", "Audio", "SFX")
SAMPLE_RATE = 22050
random.seed(1907)


def ensure_directories():
    os.makedirs(MUSIC_DIR, exist_ok=True)
    os.makedirs(SFX_DIR, exist_ok=True)


def write_wav(path, samples):
    with wave.open(path, "wb") as output:
        output.setnchannels(1)
        output.setsampwidth(2)
        output.setframerate(SAMPLE_RATE)
        frames = bytearray()
        for sample in samples:
            value = max(-32768, min(32767, int(sample * 32767)))
            frames.extend(struct.pack("<h", value))
        output.writeframes(bytes(frames))
    print("Generated " + path)


def silence(duration):
    return [0.0] * int(SAMPLE_RATE * duration)


def add(destination, source, offset=0.0, gain=1.0):
    start = int(offset * SAMPLE_RATE)
    for index, sample in enumerate(source):
        target = start + index
        if 0 <= target < len(destination):
            destination[target] += sample * gain


def envelope(samples, attack=0.01, release=0.08):
    length = len(samples)
    attack_count = max(1, int(attack * SAMPLE_RATE))
    release_count = max(1, int(release * SAMPLE_RATE))
    for index in range(length):
        if index < attack_count:
            factor = index / attack_count
        elif index >= length - release_count:
            factor = max(0.0, (length - index) / release_count)
        else:
            factor = 1.0
        samples[index] *= factor
    return samples


def sine(frequency, duration, volume=1.0):
    count = int(SAMPLE_RATE * duration)
    return [math.sin(2.0 * math.pi * frequency * index / SAMPLE_RATE) * volume for index in range(count)]


def square(frequency, duration, volume=1.0):
    count = int(SAMPLE_RATE * duration)
    return [volume if math.sin(2.0 * math.pi * frequency * index / SAMPLE_RATE) >= 0.0 else -volume for index in range(count)]


def sweep(start_frequency, end_frequency, duration, volume=1.0):
    count = int(SAMPLE_RATE * duration)
    phase = 0.0
    result = []
    for index in range(count):
        ratio = index / max(1, count - 1)
        frequency = start_frequency + (end_frequency - start_frequency) * ratio
        phase += 2.0 * math.pi * frequency / SAMPLE_RATE
        result.append(math.sin(phase) * volume)
    return result


def noise(duration, volume=1.0, smoothing=0.0):
    count = int(SAMPLE_RATE * duration)
    result = []
    previous = 0.0
    for _ in range(count):
        current = random.uniform(-1.0, 1.0)
        if smoothing > 0.0:
            previous = previous * smoothing + current * (1.0 - smoothing)
            current = previous
        result.append(current * volume)
    return result


def normalize(samples, peak=0.85):
    maximum = max(0.0001, max(abs(sample) for sample in samples))
    scale = peak / maximum
    return [sample * scale for sample in samples]


def make_music():
    duration = 8.0
    result = silence(duration)
    melody = [220.0, 261.63, 293.66, 329.63, 293.66, 261.63, 246.94, 220.0]
    bass = [110.0, 130.81, 146.83, 164.81, 146.83, 130.81, 123.47, 110.0]

    for step in range(16):
        beat = step * 0.5
        note = melody[step % len(melody)]
        add(result, envelope(sine(note, 0.42, 0.22), 0.01, 0.14), beat, 1.0)
        add(result, envelope(square(note * 2.0, 0.16, 0.045), 0.005, 0.08), beat + 0.25, 1.0)
        add(result, envelope(sine(bass[step % len(bass)], 0.34, 0.18), 0.01, 0.12), beat, 1.0)

    for beat in range(16):
        time = beat * 0.5
        add(result, envelope(noise(0.09, 0.22, 0.42), 0.002, 0.06), time, 1.0)
        add(result, envelope(sine(86.0, 0.14, 0.2), 0.002, 0.1), time + 0.25, 1.0)

    return normalize(result, 0.72)


def make_coin():
    result = silence(0.18)
    add(result, envelope(sine(880.0, 0.08, 0.7), 0.002, 0.04), 0.0)
    add(result, envelope(sine(1320.0, 0.12, 0.7), 0.002, 0.08), 0.07)
    return normalize(result)


def make_jump():
    return normalize(envelope(sweep(320.0, 740.0, 0.24, 0.65), 0.005, 0.08))


def make_slide():
    return normalize(envelope(noise(0.3, 0.55, 0.72), 0.01, 0.14))


def make_crash():
    result = silence(0.42)
    add(result, envelope(noise(0.36, 0.8, 0.18), 0.002, 0.3), 0.0)
    add(result, envelope(sweep(180.0, 52.0, 0.32, 0.6), 0.002, 0.22), 0.02)
    return normalize(result, 0.9)


def make_power_up():
    result = silence(0.42)
    for index, frequency in enumerate((523.25, 659.25, 783.99, 1046.5)):
        add(result, envelope(sine(frequency, 0.18, 0.55), 0.004, 0.1), index * 0.09)
    return normalize(result)


def make_train_horn():
    result = silence(0.8)
    add(result, envelope(sine(220.0, 0.72, 0.5), 0.03, 0.18), 0.0)
    add(result, envelope(sine(277.18, 0.72, 0.42), 0.03, 0.18), 0.0)
    add(result, envelope(sine(110.0, 0.72, 0.22), 0.03, 0.18), 0.0)
    return normalize(result, 0.78)


def make_ui_click():
    return normalize(envelope(square(660.0, 0.07, 0.45), 0.002, 0.05))


ensure_directories()
write_wav(os.path.join(MUSIC_DIR, "ISR_Loop_Main_001.wav"), make_music())
write_wav(os.path.join(SFX_DIR, "ISR_SFX_Coin_001.wav"), make_coin())
write_wav(os.path.join(SFX_DIR, "ISR_SFX_Jump_001.wav"), make_jump())
write_wav(os.path.join(SFX_DIR, "ISR_SFX_Slide_001.wav"), make_slide())
write_wav(os.path.join(SFX_DIR, "ISR_SFX_Crash_001.wav"), make_crash())
write_wav(os.path.join(SFX_DIR, "ISR_SFX_PowerUp_001.wav"), make_power_up())
write_wav(os.path.join(SFX_DIR, "ISR_SFX_TrainHorn_001.wav"), make_train_horn())
write_wav(os.path.join(SFX_DIR, "ISR_SFX_UiClick_001.wav"), make_ui_click())
