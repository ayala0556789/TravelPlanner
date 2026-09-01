import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup,FormArray, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { AlertsService } from '../../services/alerts.service';

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
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})

export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private alertService=inject(AlertsService) 

  // הגדרת מבנה הטופס עם הוולידציות החדשות והמחמירות
  registerForm: FormGroup = this.fb.group({
    // [חוק חדש]: מותר רק אותיות בעברית ובאנגלית ורווחים (בלי מספרים ובלי תווים מיוחדים)
    firstName: ['', [Validators.required, Validators.pattern(/^[a-zA-Zא-ת\s]+$/)]],
    lastName: ['', [Validators.required, Validators.pattern(/^[a-zA-Zא-ת\s]+$/)]],
    
    // [חוק חדש]: אימייל חייב להכיל סיומת תקנית מלאה (למשל סיומת של 2 אותיות ומעלה כמו com, co.il)
    email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
    
    // [חוק חדש]: קריאה לפונקציית הבדיקה האמיתית של תעודת הזהות שיצרנו למעלה
    identityNumber: ['', [Validators.required, israeliIdValidator()]],
    
    password: ['', [Validators.required, Validators.minLength(6)]],
    homeAddress: ['', Validators.required],
    preferAccessiblePaths: [false],           // חדש: נגישות קבועה
    childrenAges: this.fb.array([])           // חדש: רשימה דינמית של גילאים
  
  });

    // גישה נוחה ל-FormArray מתוך ה-HTML
  get childrenAgesArray(): FormArray {
    return this.registerForm.get('childrenAges') as FormArray;
  }

   // הוספת שדה גיל חדש לרשימה
   addChild() {
    this.childrenAgesArray.push(
      this.fb.control(null, [Validators.required, Validators.min(0), Validators.max(18)])
    );
  }

  // הסרת שדה גיל לפי מיקום ברשימה
  removeChild(index: number) {
    this.childrenAgesArray.removeAt(index);
  }

  onSubmit() {
    if (this.registerForm.valid) {

      // בניית האובייקט שנשלח לשרת - כולל הפיכת ה-FormArray לרשימת מספרים פשוטה
      const payload = {
        ...this.registerForm.value,
        childrenAges: this.registerForm.value.childrenAges.map((age: any) => Number(age))
      };

      this.authService.register(payload).subscribe({
        next: (res) => this.router.navigate(['/login']),
        error: (err) => {
          console.error('Registration Error:', err);
          const msg = err.error?.message || 'קרתה שגיאה ברישום, נא לנסות שוב';
          this.alertService.showError('שגיאה: ' + msg);
        }
      });
    } 
    
    else {
      this.alertService.showError('נא למלא את כל הנתונים בצורה תקינה. שימו לב לשדות האדומים!');
    }
  }
}










//:)קוד קל-ללא בדיקות תקינות מחמירות:)

// import { Component, inject } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
// import { Router } from '@angular/router';
// import { AuthService } from '../../services/auth';


// @Component({
//   selector: 'app-register',
//   standalone: true,
//   // ReactiveFormsModule כדי שהטופס ב-HTML יעבוד
//   imports: [CommonModule, ReactiveFormsModule],
//   templateUrl: './register.html',
//   styleUrl: './register.scss'
// })

// export class RegisterComponent {
//   private fb = inject(FormBuilder);
//   private authService = inject(AuthService);
//   private router = inject(Router);

//   // הגדרת מבנה הטופס עם ולידציות
//   // register.ts
//   registerForm: FormGroup = this.fb.group({
//     firstName: ['', Validators.required],
//     lastName: ['', Validators.required],
//     email: ['', [Validators.required, Validators.email]],
//     identityNumber: ['', [Validators.required, Validators.pattern('^[0-9]{9}$')]], // בדיוק 9 ספרות
//     password: ['', [Validators.required, Validators.minLength(6)]],
//     homeAddress: ['', Validators.required],
//     hasBaby: [false]
//   });

//   onSubmit() {
//     //   !!! valid- לבדוק
//     //בחייםם לא מגיע ל בדוק נתונים!!
//     if (this.registerForm.valid) {

//       this.authService.register(this.registerForm.value).subscribe({
//         next: (res) => this.router.navigate(['/login']),

//         error: (err) => {
//           console.error('Registration Error:', err);
          
//           const msg = err.error?.message || 'קרתה שגיאה ברישום, נא לנסות שוב';
//           alert('שגיאה: ' + msg);
//         }
//       });
//     }
//     else {
//       alert('נא למלא את כל הנתונים בצורה תקינה')

//     }
//   }
// }