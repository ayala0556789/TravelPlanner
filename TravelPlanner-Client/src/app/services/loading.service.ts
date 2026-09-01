import { Injectable, signal } from '@angular/core';

// מגדירים שזה שירות שזמין בכל האפליקציה (Singleton)
@Injectable({ providedIn: 'root' })
export class LoadingService {
  // סיגנל שכל האפליקציה יכולה להקשיב לו
  
  //משתנה השומר האם להציג את העיגול המסתובב
  loading = signal<boolean>(false);
  //תוכן ההודעה
  message = signal<string>('טוען...');

  show(msg: string = 'טוען...') { 
    //אם האפליקציה כבר מציגה הודעה מיוחדת
    //לא נציג א הברירת מחדל
    if (this.loading() && msg === 'מעבד נתונים מהשרת...') {
      return;
    }
    this.message.set(msg);
    this.loading.set(true); 
  }
  
  hide() { 
    //מסתיימת לא משנה אם הצליחה http יופעל ברגע שקריאת ה 
    this.loading.set(false); 
  }
}