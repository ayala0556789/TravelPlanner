import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AlertsService } from '../services/alerts.service';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const alertService=inject(AlertsService)//למה אי אפשר להשתמש בו? צריך שהוא יהיה פריואיט
  
  // בדיקה פשוטה: האם שמרנו טוקן כשהמשתמש התחבר או נרשם?
  const token = localStorage.getItem('token'); 

  if (token) {
    return true; // יש טוקן! תן לו להיכנס לדף הטיולים
  } 

  else {
    alertService.showSuccess('אופס! נראה שעדיין לא התחברת. כדי לתכנן טיול צריך להזדהות.')
    // אין טוקן - המשתמש לא מחובר
    alertService.showWarning('כדי לגשת לאזור האישי צריך להתחבר קודם.');
    router.navigate(['/login']); // שלח אותו להתחבר
    return false; // חסום את הגישה לדף
  }
};