function floatTo16BitPCM(output, offset, input) {
  for (let i = 0; i < input.length; i++, offset += 2) {
    const s = Math.max(-1, Math.min(1, input[i]));
    output.setInt16(offset, s < 0 ? s * 0x8000 : s * 0x7FFF, true);
  }
}
function writeString(view, offset, string) {
  for (let i = 0; i < string.length; i++) {
    view.setUint8(offset + i, string.charCodeAt(i));
  }
}
export function generateWavFile(audioBuffer) {
  const channelData = audioBuffer.getChannelData(0);
  const sampleRate = audioBuffer.sampleRate;
  const length = channelData.length * 2;
  const arrayBuffer = new ArrayBuffer(44 + length);
  const view = new DataView(arrayBuffer);
  writeString(view, 0, "RIFF");
  view.setUint32(4, 36 + length, true);
  writeString(view, 8, "WAVE");
  writeString(view, 12, "fmt ");
  view.setUint32(16, 16, true);
  view.setUint16(20, 1, true);
  view.setUint16(22, 1, true);
  view.setUint32(24, sampleRate, true);
  view.setUint32(28, sampleRate * 2, true);
  view.setUint16(32, 2, true);
  view.setUint16(34, 16, true);
  writeString(view, 36, "data");
  view.setUint32(40, length, true);
  floatTo16BitPCM(view, 44, channelData);
  return new Blob([arrayBuffer], { type: "audio/wav" });
}

export function generateAllSounds() {
  const audioContext = new (window.AudioContext || window.webkitAudioContext)();
  const sounds = {
    catch: generateCatchSound(audioContext),
    miss: generateMissSound(audioContext),
    freeze: generateFreezeSound(audioContext),
    bomb: generateBombSound(audioContext),
  };
  Object.entries(sounds).forEach(([name, buffer]) => {
    const blob = generateWavFile(buffer);
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `${name}.wav`;
    console.log(`📥 Download link for ${name}: ${url}`);
  });
}
function generateCatchSound(audioContext) {
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
function generateMissSound(audioContext) {
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
function generateFreezeSound(audioContext) {
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
function generateBombSound(audioContext) {
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
