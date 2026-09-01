import { Component } from '@angular/core';
import { Router,RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule], 
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent {
  // הזרקת ה-Router כדי שנוכל לעבור דפים
  constructor(private router: Router) { }

  startPlanning() {
    const token = localStorage.getItem('token');

    if (token) {
      this.router.navigate(['/plan-trip']).then(nav => {
        console.log('Navigation success:', nav); 
      }).catch(err => console.error('Navigation error:', err));
    } else {
      const goRegister = confirm("אופס, עדיין לא נרשמת? התחברי כדי להתחיל לתכנן!");
      if (goRegister) {
        this.router.navigate(['/register']);
      }
    }

  }
}