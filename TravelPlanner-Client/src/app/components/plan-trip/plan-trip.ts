import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TripService } from '../../services/trip.service';
import { Router } from '@angular/router';
import { LoadingService } from '../../services/loading.service.js';
import { AlertsService } from '../../services/alerts.service';
import { ProfileService } from '../../services/profile.service';

@Component({
  selector: 'app-plan-trip',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './plan-trip.html',
  styleUrls: ['./plan-trip.scss']
})
export class PlanTripComponent {
  private fb = inject(FormBuilder);
  private tripService = inject(TripService);
  private router = inject(Router);
  private loadingService = inject(LoadingService);
  private alertService = inject(AlertsService);
  private profileService = inject(ProfileService);

  tripCategory: string = 'general';
  childrenAges: number[] = []; // גילאי הילדים מהפרופיל


  planForm: FormGroup = this.fb.group({
    destination: ['', Validators.required],
    travelDate: ['', Validators.required],
    durationInHours: [5, [Validators.required, Validators.min(1), Validators.max(24)]],
    category: ['general', Validators.required],
    budget: ['free', Validators.required],        // : 'free' / 'paid'
    selectedChildrenAges: [[]]                     // : מערך גילאים שנבחרו
  });


  ngOnInit() {
    this.profileService.getProfile().subscribe(user => {
      this.childrenAges = user.childrenAges || [];
    });
  }

  toggleChild(age: number, checked: boolean) {
    const current = this.planForm.value.selectedChildrenAges as number[];
    const updated = checked ? [...current, age] : current.filter(a => a !== age);
    this.planForm.patchValue({ selectedChildrenAges: updated });
  }

  onSubmit() {
    if (this.planForm.valid) {

      this.loadingService.show('ה-AI שלנו בונה עבורך את המסלול המושלם... 🌍');

      const tripDto = {
        destination: this.planForm.value.destination,
        durationInHours: Number(this.planForm.value.durationInHours),
        // המרה לפורמט ISO - זה הפתרון ל-400 Bad Request!
        travelDate: new Date(this.planForm.value.travelDate).toISOString(),
        category: this.planForm.value.category,
        budget: this.planForm.value.budget,
        selectedChildrenAges: this.planForm.value.selectedChildrenAges,
      };
      console.log('Sending Payload:', tripDto);

      this.tripService.createTrip(tripDto).subscribe({
        next: (tripId) => {

          // אחרי שהטיול נוצר ב-DB, נעבור לדף המפה עם ה-ID של הטיול
          this.router.navigate(['/trip-map', tripId]);
        },

        error: (err) => {
          console.error(err);
          const message = err.error?.errors
            ? JSON.stringify(err.error.errors)
            : 'שגיאה בנתונים. ודאי שכל השדות מלאים.';
          this.alertService.showError(message);
          this.alertService.showError('שגיאה ביצירת הטיול. ודאי שאת מחוברת.');
        }
      });
    }
  }
}