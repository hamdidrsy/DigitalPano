(() => {
  'use strict';

  const root = document.querySelector('[data-pano-root]');
  const connectionStatus = document.querySelector('[data-connection-status]');
  const snapshotKey = root?.dataset.screenSlug ? `digitalpano:snapshot:${root.dataset.screenSlug}` : null;
  if (snapshotKey && root.innerHTML.trim()) {
    try { localStorage.setItem(snapshotKey, root.innerHTML); } catch { /* Depolama kapalıysa yayın devam eder. */ }
  }
  const setConnectionState = online => {
    if (!connectionStatus) return;
    connectionStatus.textContent = online ? 'Çevrimiçi · Otomatik güncellenir' : 'Çevrimdışı · Son yayın gösteriliyor';
    connectionStatus.classList.toggle('is-offline', !online);
  };
  setConnectionState(navigator.onLine);
  const slides = Array.from(document.querySelectorAll('[data-slide]'));
  let currentIndex = -1;
  let timerId;

  const stopCurrent = () => {
    window.clearTimeout(timerId);
    if (currentIndex >= 0) {
      const video = slides[currentIndex].querySelector('video');
      if (video) {
        video.pause();
        video.currentTime = 0;
      }
    }
  };

  const showSlide = index => {
    if (slides.length === 0) return;
    stopCurrent();
    slides.forEach((slide, slideIndex) => {
      slide.hidden = slideIndex !== index;
    });
    currentIndex = index;
    const slide = slides[index];
    const next = () => showSlide((index + 1) % slides.length);
    const video = slide.querySelector('video');
    if (video) {
      video.onended = next;
      video.onerror = () => window.setTimeout(next, 3000);
      const playAttempt = video.play();
      if (playAttempt) playAttempt.catch(() => window.setTimeout(next, Number(slide.dataset.duration) || 10000));
      return;
    }

    timerId = window.setTimeout(next, Number(slide.dataset.duration) || 10000);
  };

  if (slides.length > 0) showSlide(0);

  const timeFormatter = new Intl.DateTimeFormat('tr-TR', {
    timeZone: 'Europe/Istanbul', hour: '2-digit', minute: '2-digit', second: '2-digit'
  });
  const dateFormatter = new Intl.DateTimeFormat('tr-TR', {
    timeZone: 'Europe/Istanbul', day: '2-digit', month: 'long', year: 'numeric'
  });
  const dayFormatter = new Intl.DateTimeFormat('tr-TR', {
    timeZone: 'Europe/Istanbul', weekday: 'long'
  });
  const updateClock = () => {
    const now = new Date();
    const timeElement = document.querySelector('[data-pano-time]');
    const dateElement = document.querySelector('[data-pano-date]');
    const dayElement = document.querySelector('[data-pano-day]');
    if (timeElement) timeElement.textContent = timeFormatter.format(now);
    if (dateElement) dateElement.textContent = dateFormatter.format(now);
    if (dayElement) dayElement.textContent = dayFormatter.format(now);
  };
  updateClock();
  window.setInterval(updateClock, 1000);

  const heartbeat = () => {
    if (!root?.dataset.heartbeatUrl) return;
    fetch(root.dataset.heartbeatUrl, { method: 'POST', cache: 'no-store', credentials: 'same-origin' }).catch(() => {});
  };
  heartbeat();
  window.setInterval(heartbeat, 60000);

  let refreshScheduled = false;
  const refreshPage = async () => {
    if (refreshScheduled || !root?.dataset.refreshUrl) return;
    refreshScheduled = true;
    try {
      const response = await fetch(root.dataset.refreshUrl, { cache: 'no-store', credentials: 'same-origin' });
      if (!response.ok || response.headers.get('X-DigitalPano-Offline') === 'true') throw new Error('offline');
      setConnectionState(true);
      window.setTimeout(() => window.location.replace(root.dataset.refreshUrl), 350);
    } catch {
      refreshScheduled = false;
      setConnectionState(false);
    }
  };

  const recordSeparator = String.fromCharCode(0x1e);
  let socket;
  let reconnectAttempt = 0;
  let reconnectTimer;
  const scheduleReconnect = () => {
    window.clearTimeout(reconnectTimer);
    const delays = [1000, 2000, 5000, 10000, 30000];
    reconnectTimer = window.setTimeout(connectSignalR, delays[Math.min(reconnectAttempt, delays.length - 1)]);
    reconnectAttempt += 1;
  };
  const connectSignalR = async () => {
    if (!root?.dataset.signalrUrl) return;
    try {
      const query = root.dataset.signalrUrl.split('?')[1];
      const response = await fetch(`/hubs/pano/negotiate?${query}&negotiateVersion=1`, {
        method: 'POST', cache: 'no-store', credentials: 'same-origin'
      });
      if (!response.ok) throw new Error('SignalR negotiation failed');
      const negotiation = await response.json();
      const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
      socket = new WebSocket(`${protocol}//${window.location.host}/hubs/pano?id=${encodeURIComponent(negotiation.connectionToken)}&${query}`);
      socket.onopen = () => socket.send(JSON.stringify({ protocol: 'json', version: 1 }) + recordSeparator);
      socket.onmessage = event => {
        event.data.split(recordSeparator).filter(Boolean).forEach(frame => {
          const message = JSON.parse(frame);
          if (message.type === 1 && message.target === 'YayinDegisti') refreshPage();
          if (message.type === 6) socket.send(JSON.stringify({ type: 6 }) + recordSeparator);
        });
        reconnectAttempt = 0;
      };
      socket.onclose = scheduleReconnect;
      socket.onerror = () => socket.close();
    } catch {
      scheduleReconnect();
    }
  };

  connectSignalR();
  window.setInterval(refreshPage, 30000);
  window.addEventListener('offline', () => setConnectionState(false));
  window.addEventListener('online', () => { setConnectionState(true); refreshPage(); });

  if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/service-worker.js', { scope: '/' }).catch(() => {});
  }
})();
