import { Component, signal ,inject,OnInit} from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.js';
import { Router } from '@angular/router';
import { WeatherService } from './services/weather.service'
import { HttpClientModule } from '@angular/common/http';
import { LoadingService } from './services/loading.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit{
  protected readonly title = signal('TravelPlanner-Client');
  public authService = inject(AuthService);
  private router = inject(Router);
  public weatherService = inject(WeatherService); // הזרקת שירות מזג האוויר
  public loadingService = inject(LoadingService);

  ngOnInit() {
    // טעינת מזג האוויר בטעינת האפליקציה
    this.weatherService.fetchCurrentWeather();
  }
  onLogout() {
    this.authService.logout();
    this.router.navigate(['/home'])
    }
}
