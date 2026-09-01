import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root' // זה הופך את השירות לזמין בכל האפליקציה אוטומטית!
})
export class AlertsService {

    showSuccess(title: string) {
        Swal.fire({ // זו הפונקציה הראשית של הספרייה שמציגה את החלון
          icon: 'success', // הקובע את האייקון (במקרה הזה V ירוק)
          title: title,    // הטקסט הראשי שיופיע בכותרת
          showConfirmButton: false, // מונע מהמשתמש ללחוץ על "אישור" - זה נעלם לבד
          timer: 1500      // מגדיר שהחלון ייסגר אוטומטית אחרי 1.5 שניות
        });
    }

  showError(text: string) {
    Swal.fire({
        icon: 'error',              // הופך את האייקון ל-X אדום
        title: 'אופס, משהו השתבש...', // הכותרת המודגשת בחלק העליון
        text: text,                 // התוכן הדינמי (מה שתשלחי לפונקציה)
        confirmButtonColor: '#ff6a88' // צבע הכפתור (אדום/ורוד בהתאם לעיצוב שלך)
      });
  }

  showWarning(text: string) {
    Swal.fire({
        icon: 'warning',            // הופך את האייקון לסימן קריאה צהוב
        title: 'שים לב',            // כותרת שמשדרת צורך בזהירות
        text: text,                 // התוכן שמופיע מתחת לכותרת
        confirmButtonColor: '#ff9a8b', // צבע הכפתור (מותאם לעיצוב הכתום-אפרסק שלך)
        confirmButtonText: 'הבנתי'  // שינוי הטקסט שמופיע על הכפתור
      });
  }
}