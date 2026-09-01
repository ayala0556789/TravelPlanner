import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  private http = inject(HttpClient);
  
  // נשתמש ב-Signals כדי לעדכן את התצוגה בצורה חלקה
  public currentTemp = signal<number | null>(null);
  public weatherIcon = signal<string>('💤');

  // פונקציה להבאת מזג אוויר לפי מיקום (קו רוחב ואורך)
  async fetchCurrentWeather(lat: number = 32.0853, lon: number = 34.7818) { // ברירת מחדל תל אביב
    const url = `https://api.open-meteo.com/v1/forecast?latitude=${lat}&longitude=${lon}&current_weather=true`;
    
    try {
      const data: any = await firstValueFrom(this.http.get(url));
      this.currentTemp.set(Math.round(data.current_weather.temperature));
      this.weatherIcon.set(this.getWeatherEmoji(data.current_weather.weathercode));
    } 
    catch (error) {
      console.error('Error fetching weather', error);
    }
  }

  // המרה של קוד מזג האוויר לאמוג'י מתאים
  private getWeatherEmoji(code: number): string {
    if (code === 0) return '☀️'; // שמיים בהירים
    if (code <= 3) return '🌤️'; // מעונן חלקית
    if (code >= 61 && code <= 67) return '🌧️'; // גשם
    if (code >= 71 && code <= 77) return '❄️'; // שלג
    if (code >= 95) return '⛈️'; // סופת רעמים
    return '☁️'; // מעונן
  }
}