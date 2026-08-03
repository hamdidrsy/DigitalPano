(() => {
  'use strict';

  const root = document.querySelector('[data-pano-root]');
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
    document.querySelector('[data-pano-time]').textContent = timeFormatter.format(now);
    document.querySelector('[data-pano-date]').textContent = dateFormatter.format(now);
    document.querySelector('[data-pano-day]').textContent = dayFormatter.format(now);
  };
  updateClock();
  window.setInterval(updateClock, 1000);

  const heartbeat = () => {
    if (!root?.dataset.heartbeatUrl) return;
    fetch(root.dataset.heartbeatUrl, { method: 'POST', cache: 'no-store', credentials: 'same-origin' }).catch(() => {});
  };
  heartbeat();
  window.setInterval(heartbeat, 60000);
})();
