# TravelAI — מערכת חכמה לתכנון ימי טיול עירוניים

מערכת web מלאה לתכנון מסלולי טיול עירוניים בהתאמה אישית. המשתמש מגדיר יעד, זמן, תקציב, סגנון וההרכב המשפחתי, והמערכת בונה לו לו"ז אופטימלי הכולל מפה, הסברי AI לכל תחנה ושמירת המסלול לשימוש עתידי.

## הרעיון המרכזי

המערכת לא סומכת על ה-AI "בעיניים עצומות". הזרימה בנויה כארכיטקטורת **RAG** (Retrieval-Augmented Generation):

1. **Google Places API** מחזיר רשימת מקומות אמיתיים ומדויקים ביעד הנבחר (קואורדינטות, מזהים).
2. **Gemini AI** מקבל את הרשימה הזו + פרופיל המשתמש (תקציב, ילדים, נגישות, מזג אוויר), ומשמש כ"מנוע החלטות" בלבד: בוחר תת-קבוצה מתאימה, מסדר אותה גיאוגרפית, וכותב הסבר קצר לכל מקום.

כך נמנעות הזיות (Hallucinations) — ה-AI לעולם לא ממציא מקום, רק בוחר מתוך נתונים אמיתיים.

## ארכיטקטורה

### צד שרת — C# / ASP.NET Core, Clean Architecture

```
TravelPlanner.Domain          Entities, Interfaces, DTOs — ליבת המערכת, ללא תלויות חיצוניות
TravelPlanner.Application     Use Cases (CQRS עם MediatR), DTOs, Mappings
TravelPlanner.Infrastructure  מימוש מול DB (EF Core), שירותים חיצוניים (Google Maps, Gemini, Weather), JWT
TravelPlanner.WebApi          Controllers, נקודות קצה, הגדרות אימות ו-CORS
```

### צד לקוח — Angular (Standalone Components, Signals)

```
components/   home, register, login, profile, plan-trip, trip-map (+ place-card)
services/     auth, trip, profile, weather, loading, alerts
guards/       auth-guard — חוסם גישה למי שאינו מחובר
interceptors/ auth (מצרף JWT לכל בקשה), loading (מציג Spinner גלובלי)
```

## פיצ'רים עיקריים

- **הרשמה והתחברות** עם JWT, כולל ולידציית תעודת זהות ישראלית (אלגוריתם משרד הפנים).
- **פרופיל אישי**: כתובת מגורים, גילאי ילדים (רשימה דינמית), העדפת מסלולים נגישים.
- **יצירת טיול מותאם אישית**: יעד, תאריך, משך זמן, סגנון (היסטוריה / קולינריה / טבע / קניות), תקציב (חינמי בלבד / כולל תשלום), ובחירה מי מהילדים מצטרף לטיול הספציפי.
- **תכנון AI**: שילוב Google Places + Gemini + Open-Meteo (מזג אוויר) לבניית מסלול מותאם, כולל התחשבות בנוחות למשפחות עם ילדים קטנים ובנגישות.
- **מפת Google אינטראקטיבית** עם סימון המקומות שנבחרו.
- **מנגנון Fallback**: אם ה-AI אינו זמין (מכסה חינמית / תקלת רשת), המערכת נופלת בעדינות למקומות המובילים שהוחזרו מגוגל, כדי שהמשתמש לעולם לא יקבל מסך שגיאה.
- **היסטוריית טיולים** ורשימת ציוד לטיול (בפיתוח).

## טכנולוגיות

| שכבה | טכנולוגיה |
|---|---|
| Backend | ASP.NET Core Web API, EF Core, MediatR (CQRS), JWT, BCrypt |
| Frontend | Angular (Standalone, Signals, Reactive Forms), SweetAlert2 |
| DB | SQL Server |
| AI | Google Gemini (REST API ישיר) |
| מפות ומקומות | Google Maps JavaScript API, Google Places API |
| מזג אוויר | Open-Meteo (חינמי, ללא מפתח) |

## הרצה מקומית

### שרת
1. עדכני `TravelPlanner.WebApi/appsettings.Development.json` עם: Connection String, מפתחות Google Maps ו-Gemini, ומפתח JWT (קובץ זה אינו נכלל ב-Git — יש ליצור אותו לפי `appsettings.json`).
2. `Update-Database` להרצת ה-Migrations.
3. הרצה מ-Visual Studio (F5).

### לקוח
1. עדכני `src/environments/environment.development.ts` עם מפתח Google Maps.
2. `npm install`
3. `ng serve`

## הערות אבטחה

- מפתחות API וסודות (Connection String, JWT Key) מופרדים מהקוד באמצעות `appsettings.Development.json` / `environment.development.ts`, שאינם עולים ל-Git.
- מפתח Google Maps חשוף מטבעו בצד הלקוח (נדרש לטעינת JavaScript API) — יש להגביל אותו ב-Google Cloud Console לפי HTTP Referrer לפני עלייה לסביבת ייצור.