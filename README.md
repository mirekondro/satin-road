# Satin Road

A satirical marketplace built as the interdisciplinary project for weeks 40–41.
Users can open their own shop, list products, manage stock and buy from other vendors.
Admins manage the product categories. Everything sold here is fictional.

> **Demo in 30 seconds:** `docker compose up --build` → open **http://localhost:3000** → log in as `buyer_bob` / `demo1234`.

---

## Features

### User stories

| As a… | I can… | Where |
|---|---|---|
| Administrator | create, rename and delete product categories | `/admin/categories` |
| User | register and log in (JWT) | `/register`, `/login` |
| User | create listings and manage my inventory (stock, edit, deactivate) | `/my/listings` |
| User | browse listings, filter by category and buy from other users | `/listings` |
| User | see my purchases and my sales | `/my/orders` |

### Hard stories

| # | Story (from the assignment) | How we interpreted it | Code |
|---|---|---|---|
| 11 | *"If more than 10 orders is placed on a single vendor by a user, the next order price will be reduced by 20%."* | A buyer who already has **more than 10** orders at one vendor gets −20 % on every further order there (the 12th order is the first discounted one). Only orders of **this buyer** at **this vendor** count. The unit price stays the original, only the total is discounted. | `OrderService.QualifiesForDiscount`, `CalculateTotal` |
| 12 | *"For every purchase there's a 1% chance the buyer is FBI and the vendor will be shut down permanently / their products will be removed."* | After a **valid** purchase is saved we roll the dice. On a hit the vendor is marked as shut down and **all** their listings are deactivated in one transaction. The vendor can no longer log in (403) and their products disappear from the shop. | `OrderService.PlaceOrderAsync`, `IChanceProvider`, `OrderRepository.ShutDownVendorAsync` |
| 13 | *"If a vendor has sold more than 100 orders they will be featured on the top of the listings / landing page."* | Vendors with **more than 100** sold orders (101+) are shown on the landing page, best sellers first. Vendors shut down by the FBI are never featured. | `VendorService.IsFeatured`, `GetFeaturedAsync` |

---

## Tech stack

| Part | Technology |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API, **Linq2db**, PostgreSQL 17 |
| API docs | **OpenAPI** (`Microsoft.AspNetCore.OpenApi`) + Swagger UI |
| Auth | JWT bearer tokens, passwords hashed with BCrypt |
| Frontend | **Bun + React 19** (Vite), React Router |
| API client | **swagger-typescript-api** – generated from the backend's OpenAPI document |
| Tests | xUnit |
| Deployment | **Docker** + docker compose, nginx serves the frontend and proxies `/api` |

---

## Getting started

### Run everything with Docker (recommended)

```bash
docker compose up --build
```

| Service | URL |
|---|---|
| Web app | http://localhost:3000 |
| PostgreSQL | localhost:5432 (satin / satin) |

The database is created from `db/init.sql` and filled with demo data from `db/seed.sql`
on the **first** start. To start again from a clean database:

```bash
docker compose down -v
docker compose up --build
```

### Demo accounts

| Username | Password | Role in the demo |
|---|---|---|
| `admin` | `admin123` | Administrator – manages categories |
| `darknet_dave` | `demo1234` | **Featured** vendor (150 sales) |
| `silk_sarah` | `demo1234` | **Featured** vendor (120 sales) |
| `shady_steve` | `demo1234` | Small vendor (5 sales) – good target for the FBI raid demo |
| `buyer_bob` | `demo1234` | Has 11 orders at `darknet_dave` → his **next** order there gets **−20 %** |
| `buyer_alice` | `demo1234` | Loyal customer of both featured vendors |

### FBI raid demo

The probability is configurable (`Fbi:Chance`, default `0.01`). To guarantee a raid on the next purchase:

```bash
FBI_CHANCE=1 docker compose up --build
```

### Local development

```bash
# 1. Database
docker compose up -d db

# 2. Backend – http://localhost:5273, Swagger UI at /swagger
cd backend
dotnet run --project SatinRoad.Api

# 3. Frontend – http://localhost:5173 (Vite proxies /api to the backend)
cd frontend
bun install
bun run dev

# After a backend API change, regenerate the typed client (backend must be running)
bun run gen:api
```

Manual API requests for every endpoint are in `backend/SatinRoad.Api/SatinRoad.Api.http`.

---

## Project structure

```
satin-road/
├── backend/
│   ├── SatinRoad.Api/        Controllers, contracts (DTOs), JWT, error handling, Program.cs
│   ├── SatinRoad.Core/       Business logic – one folder per feature
│   │   ├── Auth/  Categories/  Listings/  Orders/  Vendors/
│   │   ├── Common/           Domain exceptions (→ 400/401/403/404/409)
│   │   ├── Data/             Linq2db repositories + AppDataConnection
│   │   └── Entities/         Table mappings
│   └── SatinRoad.Tests/      xUnit tests + fake repositories
├── frontend/
│   └── src/
│       ├── api/              generated client, hooks (useFetch, useCategories, …)
│       ├── auth/             AuthContext, route guards
│       ├── components/
│       └── pages/
├── db/
│   ├── init.sql              schema, constraints, indexes, categories
│   └── seed.sql              demo users, listings and orders
└── docker-compose.yml
```

### How a request flows

```
React page → generated API client → nginx /api → Controller → Service (business rules) → Repository (Linq2db) → PostgreSQL
```

- **Controllers** are thin: read the request, call a service, map the result.
- **Services** contain all business rules (validation, discount, FBI raid, featured vendors).
  They only depend on **interfaces** (`IOrderRepository`, `IChanceProvider`, …) – that is what makes them unit-testable.
- **Repositories** are the only place that talks to the database.
- Services throw domain exceptions (`ValidationException`, `NotFoundException`, …);
  `DomainExceptionHandler` turns them into HTTP status codes, so controllers need no `try/catch`.

---

## Way of working

### GitHub Projects & Issues

Every user story and hard story was split into issues with acceptance criteria
(labels `backend`, `frontend`, `devops`, `test`, `docs`, `hard-story`) and tracked on a GitHub Project board
(Todo → In progress → In review → Done).

### Branching strategy

```
feature/<issue>-<name>  ──PR + review──►  dev  ──release PR──►  main
```

- `main` – stable, demo-ready version
- `dev` – integration branch (default branch)
- one branch per issue, e.g. `feature/12-fbi-raid`
- every change goes through a **pull request** with `Closes #<issue>` and a **code review** by the other team member
- `main` and `dev` are protected by a ruleset (PR + 1 approval required, no force pushes)

---

## Testing methodology – Test Driven Development

We chose **TDD** for the business logic in `SatinRoad.Core`.

For every feature we followed **red → green**:

1. **Red** – write the tests first against an empty skeleton (methods throw `NotImplementedException`) and commit them failing.
2. **Green** – implement the logic until all tests pass and commit.
3. **Refactor** – clean up while the tests stay green.

This is visible in the git history as pairs of commits:

```
test: add FBI raid tests (red)
feat: 1% FBI raid shuts down vendor and removes their listings (green)
```

| Feature | Red commit | Green commit |
|---|---|---|
| Categories | `test: add failing CategoryService tests` | `CategoryService implemented` |
| Auth | `test: add AuthService tests (red)` | `feat: implement AuthService (green)` |
| Listings | `test: add failing ListingService tests` | `implement ListingService` |
| Orders + 20 % discount | `test: add order and 20% discount tests (red)` | `feat: place orders with 20% vendor discount (green)` |
| FBI raid | `test: add FBI raid tests (red)` | `feat: 1% FBI raid … (green)` |
| Featured vendors | `test: add featured vendors tests (red)` | `feat: featured vendors with more than 100 sales (green)` |

### What we test and why

We test the **services**, because that is where the rules live. Controllers and repositories are thin and are verified manually (`.http` file, Swagger, Docker).

| Test class | What it covers |
|---|---|
| `CategoryServiceTests` | name validation, duplicates (case-insensitive), cannot delete a category with listings |
| `AuthServiceTests` | registration rules, password is stored hashed, wrong password / unknown user → 401, shut-down account → 403 |
| `ListingServiceTests` | validation (title, price, stock), only the owner can edit, deactivated listings are hidden |
| `OrderServiceTests` | quantity limits, unknown/inactive listing, buying your own listing, not enough stock, stock decreases |
| `DiscountTests` | **boundary values 10 / 11** previous orders, rounding, orders at other vendors or by other buyers do not count |
| `FbiRaidTests` | raid / no raid, all vendor listings removed, other vendors untouched, order still saved, probability passed correctly, invalid orders never trigger a raid |
| `RandomChanceProviderTests` | the real random generator at 0 % and 100 % |
| `FeaturedVendorsTests` | **boundary values 100 / 101** sales, FBI-closed vendors excluded, sorting |

### Techniques

- **Fake repositories** (`FakeOrderRepository`, `FakeUserRepository`, …) keep data in memory, so tests run in milliseconds without a database.
- **Controlled randomness** – the 1 % FBI chance is behind `IChanceProvider`. Tests inject `FakeChanceProvider(true/false)`, which makes a random feature fully deterministic.
- **Boundary value testing** – every hard story is tested exactly at its limit (10/11 orders, 100/101 sales, 0 %/100 % chance), because an off-by-one (`>` vs `>=`) is the most likely bug.

### Run the tests

```bash
cd backend
dotnet test
```

The suite has over 100 test cases and runs in about two seconds.

---

## Sustainability (Lighthouse)

<!-- TODO: add the Lighthouse scores and screenshots (before / after) -->

| Category | Before | After |
|---|---|---|
| Performance | – | – |
| Accessibility | – | – |
| Best Practices | – | – |
| SEO | – | – |

The audit is run against the **production build** (`docker compose up --build`, http://localhost:3000), not the Vite dev server.

### What we did to reduce the footprint

| Measure | Why it matters |
|---|---|
| **Code splitting** – pages other than the landing page and listings are loaded on first visit (`router.tsx`) | less JavaScript downloaded and parsed on the first page load |
| **System fonts** instead of web fonts | no extra font downloads |
| **gzip compression** in nginx | text assets are transferred several times smaller |
| **Long-term caching** of hashed assets (`Cache-Control: immutable`, 1 year) | returning visitors download nothing again |
| **Dark theme** | lower power consumption on OLED screens |
| **Multi-stage Docker images** (SDK/Bun only in the build stage) | much smaller images to store and transfer |
| **No UI framework**, plain CSS | small bundle |
| **Database indexes** for the hard-story queries (`ix_orders_buyer_vendor`, `ix_orders_vendor`) | discount and featured-vendor queries do not scan the whole orders table |
| SEO meta description, `robots.txt`, `theme-color` | better Lighthouse SEO / best-practices score |
