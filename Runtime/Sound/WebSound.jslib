mergeInto(LibraryManager.library, {
    _PlaySound: function (soundKey) {
        var parsedSoundKey = Pointer_stringify(soundKey);
        if (window.isContainSoundsKey(parsedSoundKey)) {
            window.getSoundObject(parsedSoundKey).play();
        }
    },

    _SetMute: function (soundKey, mute) {
        var parsedSoundKey = Pointer_stringify(soundKey);

        if (window.isContainSoundsKey(parsedSoundKey)) {
            window.getSoundObject(parsedSoundKey).mute(mute == 1);
        }
    },

    _Stop: function (soundKey) {
        var parsedSoundKey = Pointer_stringify(soundKey);
        if (window.isContainSoundsKey(parsedSoundKey)) {
            window.getSoundObject(parsedSoundKey).stop();
        }
    },

    _Pause: function (soundKey) {
        var parsedSoundKey = Pointer_stringify(soundKey);
        if (window.isContainSoundsKey(parsedSoundKey)) {
            window.getSoundObject(parsedSoundKey).pause();
        }
    },

    _SetLoop: function (soundKey, loop) {
        var parsedSoundKey = Pointer_stringify(soundKey);
        if (window.isContainSoundsKey(parsedSoundKey)) {
            window.getSoundObject(parsedSoundKey).loop(loop == 1);
        }
    },

    _SetVolume: function (soundKey, volume) {
        var parsedSoundKey = Pointer_stringify(soundKey);
        if (window.isContainSoundsKey(parsedSoundKey)) {
            window.getSoundObject(parsedSoundKey).volume(volume);
        }
    },

    _Duration: function (soundKey) {
        var parsedSoundKey = Pointer_stringify(soundKey);
        if (window.isContainSoundsKey(parsedSoundKey)) {
            return window.getSoundObject(parsedSoundKey).duration();
        }
    },

    _SetPitch: function (soundKey, pitch) {
        var parsedSoundKey = Pointer_stringify(soundKey);
        if (window.isContainSoundsKey(parsedSoundKey)) {
            return window.getSoundObject(parsedSoundKey).rate(pitch);
        }
    },
});
