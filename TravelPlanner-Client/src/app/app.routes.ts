import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home';
import { RegisterComponent } from './components/register/register';
import { PlanTripComponent } from './components/plan-trip/plan-trip'
import { TripMapComponent } from './components/trip-map/trip-map'
import { authGuard } from './guards/auth-guard.js';
import { LoginComponent } from './components/login/login'
import { ProfileComponent } from './components/profile/profile';

export const routes: Routes = [
  // הגדרת דף הבית כברירת מחדל
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'plan-trip', component: PlanTripComponent, canActivate: [authGuard] },
  { path: 'trip-map/:id', component: TripMapComponent, canActivate: [authGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
  // אם המשתמש כותב נתיב שלא קיים - יחזור לבית
  { path: '**', redirectTo: '' }

];
