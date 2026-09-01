import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { LoadingService } from '../services/loading.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  // ברגע שיוצאת קריאת HTTP - מפעילים את ה-Loader הגלובלי
  loadingService.show('מעבד נתונים מהשרת...');

  return next(req).pipe(
    // הפונקציה finalize תרוץ תמיד בסיום הקריאה - בין אם היא הצליחה ובין אם נכשלה (שגיאה)
    finalize(() => {
      //מכבה את העיול של טוען נתונים
      loadingService.hide();
    })
  );
};