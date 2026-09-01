import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, ValidatorFn, ValidationErrors, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { AlertsService } from '../../services/alerts.service.js'


// 1. פונקציית בונוס חיצונית: בדיקה שתעודת הזהות היא אמיתית ותקנית בישראל
export function israeliIdValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const id = String(control.value).trim();

    // אם השדה ריק, נשאיר ל-Validators.required לטפל בזה
    if (!id) return null;

    // בדיקה ראשונית שזה מכיל בדיוק 9 ספרות
    if (!/^\d{9}$/.test(id)) return { invalidId: true };

    // אלגוריתם משרד הפנים לבדיקת ספרת ביקורת (Luhn Algorithm)
    let sum = 0;
    for (let i = 0; i < 9; i++) {
      let digit = Number(id.charAt(i));
      let step = digit * ((i % 2) + 1); // מכפילים לסירוגין ב-1 וב-2
      if (step > 9) step -= 9; // אם התוצאה דו-ספרתית, מחברים את הספרות (שזה כמו להחסיר 9)
      sum += step;
    }

    // תעודת זהות תקינה רק אם הסכום מתחלק ב-10 ללא שארית
    return sum % 10 === 0 ? null : { invalidId: true };
  };
}



@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private alerts = inject(AlertsService); // בשורה של ה-imports וההזרקות
  // login.ts

  loginForm: FormGroup = this.fb.group({
    identityNumber: ['', [Validators.required, israeliIdValidator()]],
     password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onLogin() {
    if (this.loginForm.invalid) {
      this.alerts.showError('נא למלא את כל השדות: תעודת זהות וסיסמה');
      return; // עוצר כאן ולא שולח קריאה לשרת
    }

    else
      this.authService.login(this.loginForm.value).subscribe({
        next: (userDto) => {
          // השרת מחזיר UserDto עם Token
          localStorage.setItem('token', userDto.token);
          this.alerts.showSuccess('התחברת בהצלחה')
          this.router.navigate(['/home']);
        },

        error: (err) => {

          console.error('Login Error:', err);
          // אם ה-catch בשרת עובד, זה תמיד יהיה אובייקט עם message
          const msg = err.error?.message || 'פרטי התחברות שגויים';
          this.alerts.showError('שגיאה: ' + msg);

        }
      });

  }
}