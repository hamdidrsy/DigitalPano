const CACHE_NAME = 'digitalpano-v1';
const SHELL_ASSETS = ['/css/pano.css', '/js/pano.js', '/offline.html'];

self.addEventListener('install', event => {
  event.waitUntil(caches.open(CACHE_NAME).then(cache => cache.addAll(SHELL_ASSETS)));
  self.skipWaiting();
});

self.addEventListener('activate', event => {
  event.waitUntil(caches.keys().then(keys => Promise.all(
    keys.filter(key => key !== CACHE_NAME).map(key => caches.delete(key))
  )));
  self.clients.claim();
});

const offlineResponse = async request => {
  const cached = await caches.match(request);
  if (!cached) return caches.match('/offline.html');
  const headers = new Headers(cached.headers);
  headers.set('X-DigitalPano-Offline', 'true');
  return new Response(await cached.blob(), { status: cached.status, statusText: cached.statusText, headers });
};

self.addEventListener('fetch', event => {
  const request = event.request;
  if (request.method !== 'GET') return;
  const url = new URL(request.url);
  if (url.origin !== self.location.origin || url.pathname.startsWith('/hubs/')) return;

  if (request.mode === 'navigate' && url.pathname.startsWith('/pano/')) {
    event.respondWith(fetch(request).then(response => {
      if (response.ok) caches.open(CACHE_NAME).then(cache => cache.put(request, response.clone()));
      return response;
    }).catch(() => offlineResponse(request)));
    return;
  }

  if (url.pathname.endsWith('.css') || url.pathname.endsWith('.js')) {
    event.respondWith(caches.match(request).then(cached => cached || fetch(request).then(response => {
      if (response.ok) caches.open(CACHE_NAME).then(cache => cache.put(request, response.clone()));
      return response;
    })));
    return;
  }

  if (url.pathname.includes('/medya/') && !request.headers.has('range')) {
    event.respondWith(caches.match(request).then(cached => cached || fetch(request).then(response => {
      const contentType = response.headers.get('content-type') || '';
      if (response.ok && contentType.startsWith('image/')) caches.open(CACHE_NAME).then(cache => cache.put(request, response.clone()));
      return response;
    })));
  }
});
