import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // 1. ננסה לשלוף את הטוקן מה-LocalStorage
  const token = localStorage.getItem('token');

  // 2. אם יש טוקן, ניצור
  //  "שיבוט" 
  // (Clone) 
  // של הבקשה ונוסיף לו את ה-Header
  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    
    // נשלח את הבקשה המשובטת לשרת
    return next(clonedReq);
  }

  // 3. אם אין טוקן (למשל בדף ההתחברות), נשלח את הבקשה המקורית כפי שהיא
  return next(req);
};