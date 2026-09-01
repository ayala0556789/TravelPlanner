import { Injectable, inject } from '@angular/core';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TripService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7071/api/Trips'; // ודאי שזה הפורט שלך



 // 1. יצירת טיול חדש
 createTrip(tripData: any): Observable<number> {


// 3. שליחת הבקשה עם ה-Headers
return this.http.post<number>(this.apiUrl, tripData);
}


  // שליפת טיול ספציפי לפי ID (כולל המקומות שלו)
  getTripById(id: number, userId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}?userId=${userId}`);
  }

  // עדכון סדר המקומות (Drag & Drop)
  updateOrder(tripId: number, places: any[]): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${tripId}/order`, places);
  }

  // שליפת היסטוריית טיולים
  getUserTrips(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

}