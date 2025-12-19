
export function generateTone(audioContext, frequency = 440, duration = 0.5, volume = 0.3) {
  const sampleRate = audioContext.sampleRate;
  const frameCount = sampleRate * duration;
  const audioBuffer = audioContext.createBuffer(1, frameCount, sampleRate);
  const channelData = audioBuffer.getChannelData(0);

  for (let i = 0; i < frameCount; i++) {
    const angle = (frequency * i * 2 * Math.PI) / sampleRate;
    const envelope = Math.max(0, 1 - (i / frameCount) * 2); // Fade out
    channelData[i] = Math.sin(angle) * volume * envelope;
  }

  return audioBuffer;
}

export function generateCatchSound(audioContext) {
  const sampleRate = audioContext.sampleRate;
  const duration = 0.6; 
  const frameCount = sampleRate * duration;
  const audioBuffer = audioContext.createBuffer(1, frameCount, sampleRate);
  const channelData = audioBuffer.getChannelData(0);

  for (let i = 0; i < frameCount; i++) {
    const progress = i / frameCount;
    const frequency = progress < 0.5 ? 261 : 330;
    const angle = (frequency * i * 2 * Math.PI) / sampleRate;
    const envelope = Math.max(0, 1 - progress);
    channelData[i] = Math.sin(angle) * 0.3 * envelope;
  }

  return audioBuffer;
}

export function generateMissSound(audioContext) {
  const sampleRate = audioContext.sampleRate;
  const duration = 0.4;
  const frameCount = sampleRate * duration;
  const audioBuffer = audioContext.createBuffer(1, frameCount, sampleRate);
  const channelData = audioBuffer.getChannelData(0);

  for (let i = 0; i < frameCount; i++) {
    const progress = i / frameCount;
    const frequency = 400 - (progress * 200);
    const angle = (frequency * i * 2 * Math.PI) / sampleRate;
    const envelope = Math.max(0, 1 - progress);
    channelData[i] = (Math.sin(angle) + Math.sin(angle * 0.5)) * 0.2 * envelope;
  }

  return audioBuffer;
}

export function generateFreezeSound(audioContext) {
  const sampleRate = audioContext.sampleRate;
  const duration = 0.3;
  const frameCount = sampleRate * duration;
  const audioBuffer = audioContext.createBuffer(1, frameCount, sampleRate);
  const channelData = audioBuffer.getChannelData(0);

  for (let i = 0; i < frameCount; i++) {
    const progress = i / frameCount;
    const frequency = 600 + (progress * 300);
    const angle = (frequency * i * 2 * Math.PI) / sampleRate;
    const envelope = Math.max(0, 1 - progress);
    const signal = Math.sign(Math.sin(angle));
    channelData[i] = signal * 0.25 * envelope;
  }

  return audioBuffer;
}

export function generateBombSound(audioContext) {
  const sampleRate = audioContext.sampleRate;
  const duration = 0.5;
  const frameCount = sampleRate * duration;
  const audioBuffer = audioContext.createBuffer(1, frameCount, sampleRate);
  const channelData = audioBuffer.getChannelData(0);

  for (let i = 0; i < frameCount; i++) {
    const progress = i / frameCount;
    const frequency = 150 - (progress * 100);
    const angle = (frequency * i * 2 * Math.PI) / sampleRate;
    const sine = Math.sin(angle);
    const noise = (Math.random() * 2 - 1) * 0.5;
    const envelope = Math.exp(-progress * 4); 
    channelData[i] = (sine * 0.7 + noise * 0.3) * 0.35 * envelope;
  }

  return audioBuffer;
}

export function generateGameStartSound(audioContext) {
  const sampleRate = audioContext.sampleRate;
  const duration = 0.8;
  const frameCount = sampleRate * duration;
  const audioBuffer = audioContext.createBuffer(1, frameCount, sampleRate);
  const channelData = audioBuffer.getChannelData(0);

  for (let i = 0; i < frameCount; i++) {
    const progress = i / frameCount;
    // Rising fanfare: 523 Hz (C5) -> 659 Hz (E5) -> 784 Hz (G5)
    let frequency = 523;
    if (progress > 0.33) frequency = 659;
    if (progress > 0.66) frequency = 784;
    
    const angle = (frequency * i * 2 * Math.PI) / sampleRate;
    const envelope = Math.max(0, 1 - progress);
    channelData[i] = Math.sin(angle) * 0.3 * envelope;
  }

  return audioBuffer;
}

const soundLibrary = {
  catch: generateCatchSound,
  miss: generateMissSound,
  freeze: generateFreezeSound,
  bomb: generateBombSound,
  gamestart: generateGameStartSound,
};

export function playSynthSound(audioContext, soundType) {
  if (!audioContext) return;

  const generator = soundLibrary[soundType];
  if (!generator) {
    console.warn(`⚠️ Unknown sound type: ${soundType}`);
    return;
  }

  try {
    const audioBuffer = generator(audioContext);
    const source = audioContext.createBufferSource();
    source.buffer = audioBuffer;
    source.connect(audioContext.destination);
    source.start(0);
    console.log(`🔊 Playing synthesized ${soundType} sound`);
  } catch (e) {
    console.error(`❌ Error playing ${soundType} sound:`, e);
  }
}
