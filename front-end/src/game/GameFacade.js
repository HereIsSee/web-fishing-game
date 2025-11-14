
class RendererSubsystem {
  constructor() {
  }
  showSplash(containerId, splashHtml) {
    const container = document.getElementById(containerId);
    if (!container) return;
    container.innerHTML = splashHtml;
  }
  clear(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;
    container.innerHTML = '';
  }
}

class ScoreManagerSubsystem {
  constructor() {}
  computeRanking(playerScores = {}) {
    const entries = Object.entries(playerScores).map(([name, score]) => ({ name, score }));
    entries.sort((a,b) => b.score - a.score);
    return entries;
  }
  formatHtml(ranking) {
    const rows = ranking.map(r => `<div class="splash-row"><span class="splash-name">${r.name}</span><span class="splash-score">${r.score}</span></div>`).join('');
    return `<div class="splash-container"><h2>Final Scores</h2>${rows}</div>`;
  }
}

class AudioSubsystem {
  constructor() {}
  playSound(url) {
    try {
      const a = new Audio(url);
      a.play().catch(() => {});
    } catch (e) {}
  }
}

export default class GameFacade {
  constructor(opts = {}) {
    this.renderer = opts.renderer || new RendererSubsystem();
    this.scoreManager = opts.scoreManager || new ScoreManagerSubsystem();
    this.audio = opts.audio || new AudioSubsystem();
  }

  showFinalScoreSplash(containerId, playerScores, opts = {}) {
    const ranking = this.scoreManager.computeRanking(playerScores || {});
    const html = this.scoreManager.formatHtml(ranking);
    const fullHtml = `
      <div class="splash-root">
        ${html}
        <div style="margin-top:12px;display:flex;gap:8px;justify-content:center;">
          <button id="splash-restart">Restart</button>
          <button id="splash-close">Close</button>
        </div>
      </div>
    `;
    this.renderer.showSplash(containerId, fullHtml);
    if (opts.soundUrl) this.audio.playSound(opts.soundUrl);
  }

  clearSplash(containerId) {
    this.renderer.clear(containerId);
  }
}

export class WebUIClient {
  constructor(facade, containerId = 'root') {
    this.facade = facade;
    this.containerId = containerId;
  }
  showScores(playerScores) {
    this.facade.showFinalScoreSplash(this.containerId, playerScores, { soundUrl: null });
  }
}

export class BotClient {
  constructor(facade) {
    this.facade = facade;
  }
  notifyScores(playerScores) {
    console.log('BotClient notified scores:', playerScores);
  }
}
