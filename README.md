# TravelAI — AI-Powered City Trip Planner

A full-stack web app that builds personalized one-day city itineraries. The user sets a destination, date, duration, travel style, budget and family composition, and the system returns an optimized route with an interactive map, a short AI-written explanation for each stop, and the option to save the trip.

<!-- Add a screenshot or short GIF here -->
<!-- ![TravelAI demo](docs/demo.gif) -->

**Live demo:** _add link here_

## Key Idea: RAG Instead of Blind Trust in AI

The AI is never allowed to invent places. The planning flow follows a Retrieval-Augmented Generation (RAG) approach:

1. **Google Places API** returns a list of real, verified places for the chosen destination (coordinates, place IDs).
2. **Gemini** receives that list together with the user profile (budget, children, accessibility, weather) and acts only as a decision engine: it selects a suitable subset, orders it geographically, and writes a short explanation per place.
3. Because Gemini chooses only from real data, hallucinated places are avoided.

<!-- Add a small flow diagram: User -> Google Places -> Gemini -> Route + Map -->

## Features

- Registration and login with JWT, including Israeli ID number validation (Ministry of Interior algorithm).
- User profile: home address, children's ages (dynamic list), and a preference for accessible routes.
- Trip creation: destination, date, duration, style (history / food / nature / shopping), budget (free only / paid included), and which children join this specific trip.
- AI planning that combines Google Places, Gemini and Open-Meteo (weather), taking young children and accessibility into account.
- Interactive Google Map with the selected places.
- **Graceful fallback:** if the AI is unavailable (free quota exhausted or network failure), the app falls back to the top places returned by Google, so the user never sees an error screen.
- Trip history and packing list (in progress).

## Architecture

### Backend: C# / ASP.NET Core, Clean Architecture

| Layer | Responsibility |
|---|---|
| `TravelPlanner.Domain` | Entities, interfaces, DTOs. The core, with no external dependencies |
| `TravelPlanner.Application` | Use cases (CQRS with MediatR), DTOs, mappings |
| `TravelPlanner.Infrastructure` | EF Core data access, external services (Google Maps, Gemini, Weather), JWT |
| `TravelPlanner.WebApi` | Controllers, endpoints, authentication and CORS configuration |

### Frontend: Angular (Standalone Components, Signals)

```
components/   home, register, login, profile, plan-trip, trip-map (+ place-card)
services/     auth, trip, profile, weather, loading, alerts
guards/       auth-guard: blocks access for unauthenticated users
interceptors/ auth (attaches JWT to every request), loading (global spinner)
```

## Tech Stack

| Area | Technology |
|---|---|
| Backend | ASP.NET Core Web API, EF Core, MediatR (CQRS), JWT, BCrypt |
| Frontend | Angular (Standalone, Signals, Reactive Forms), SweetAlert2 |
| Database | SQL Server |
| AI | Google Gemini (direct REST API) |
| Maps and places | Google Maps JavaScript API, Google Places API |
| Weather | Open-Meteo (free, no API key) |

## Getting Started

### Server

1. Create `TravelPlanner.WebApi/appsettings.Development.json` based on `appsettings.json`, with your connection string, Google Maps and Gemini keys, and a JWT key. This file is not committed to Git.
2. Run `Update-Database` to apply the migrations.
3. Start the API from Visual Studio (F5).

### Client

1. Set your Google Maps key in `src/environments/environment.development.ts`.
2. Run `npm install`.
3. Run `ng serve`.

## Security Notes

- API keys and secrets (connection string, JWT key) are kept out of the code in `appsettings.Development.json` and `environment.development.ts`, which are not committed to Git.
- The Google Maps key is inherently exposed on the client (required for the JavaScript API). Restrict it by HTTP referrer in Google Cloud Console before deploying to production.


