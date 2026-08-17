# Screenshots

These are real captures of the running app, not mockups. If the UI changes and they go stale, here's
how they were made so you can redo them.

They were taken with **headless Edge** (any Chromium works) driving the dev server. The catch is that
the authenticated pages need a session, and headless `--screenshot` can't click a login form. The way
round it is to seed `localStorage` first using a throwaway page served from the same origin, with a
persistent `--user-data-dir` so the session survives into the next invocation.

1. Start the API and the dev server, and make sure there are a few contacts to look at.
2. Drop a temporary `frontend/public/__seed.html` that POSTs to `/api/auth/login` and writes
   `ab.token`, `ab.expiresAt`, `ab.fullName` and `ab.email` into `localStorage`.
   **Restart `ng serve`** afterwards — it only picks up `public/` at startup, and until it does you'll
   get the SPA fallback instead of your file.
3. Load that page once headless with `--user-data-dir=<profile>`.
4. Take the shots with the same profile.

```bash
edge --headless=new --disable-gpu --hide-scrollbars --ignore-certificate-errors \
     --user-data-dir=<profile> --window-size=1280,860 \
     --screenshot=contacts.png --virtual-time-budget=12000 \
     http://localhost:4300/contacts
```

`--ignore-certificate-errors` is needed because the API is on HTTPS with the dev certificate.
`--virtual-time-budget` gives Angular time to boot and fetch before the shutter.

| File | Page | Window |
|---|---|---|
| `login.png` | `/login` | 1280 × 860 |
| `register.png` | `/register` | 1280 × 860 |
| `contacts.png` | `/contacts` | 1280 × 860 |
| `contact-form.png` | `/contacts` with the edit dialog open | 1280 × 940 |
| `settings.png` | `/settings` | 1280 × 860 |
| `mobile.png` | `/contacts`, narrow | 500 × 900 |

Two things that will bite you:

**The dialog shot needs a click, which headless can't do.** It was captured by temporarily making
`ContactListComponent` open the edit dialog when the URL carries `?shot=edit`, then taking the shot and
removing the hook again. Do the same and remember to take it back out.

**500px is the floor.** Chromium clamps its window width around 500px, so `--window-size=360,740`
silently lays the page out wider and crops the image — you get a screenshot that looks broken but
isn't. 500px is still under both breakpoints, so the collapsed toolbar and reduced table columns show
up properly. `--force-device-scale-factor` doesn't help; it scales the raster, not the layout viewport.
For a true 360px shot, use a real browser's device toolbar by hand.
