import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal, computed } from '@angular/core';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  private baseUrl = '/api/Users';
  // סריקה ראשונית: האם יש משתמש ב-LocalStorage?
  private currentUserSignal = signal<any>(this.getUserFromStorage());
  // משתנה לקריאה בלבד שמודיע אם מחוברים
  isLoggedIn = computed(() => !!this.currentUserSignal());

  // משתנה לקריאה בלבד עבור שם המשתמש
  currentUser = computed(() => this.currentUserSignal());

  private getUserFromStorage() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  // פונקציה להתחברות
  login(user: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, user).pipe(
      tap((userDto: any) => {
        // שמירה גם של הטוקן וגם של פרטי המשתמש
        localStorage.setItem('token', userDto.token);
        localStorage.setItem('user', JSON.stringify(userDto));
        // עדכון ה-Signal (זה יגרום ל-Navbar להתעדכן מיד!)
        this.currentUserSignal.set(userDto);
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currentUserSignal.set(null);
  }

  // פונקציה להרשמה
  register(user: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, user);
  }

  updateCurrentUserData(updatedFields: { firstName?: string; lastName?: string }) {
    const currentUser = this.currentUserSignal();
    if (!currentUser) return;

    const updatedUser = { ...currentUser, ...updatedFields };

    localStorage.setItem('user', JSON.stringify(updatedUser));
    this.currentUserSignal.set(updatedUser);
  }

  // מחזיר את ה-ID של המשתמש המחובר
  getUserId(): number {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    return user.id || 0;
  }
}