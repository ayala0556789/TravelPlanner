import { Component, OnInit, inject ,ChangeDetectorRef} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { TripService } from '../../services/trip.service';
import { AuthService } from '../../services/auth'
import { PlaceCardComponent } from './place-card/place-card'
import { LoadingService } from '../../services/loading.service.js';
import { environment } from '../../../environments/environment.development';

//היא אומרת לאנגולר שגוגל קיים ב-index.html
declare const google: any;

@Component({
  selector: 'app-trip-map',
  standalone: true,
  imports: [CommonModule, PlaceCardComponent],
  templateUrl: './trip-map.html', 
  styleUrls: ['./trip-map.scss']   
})
export class TripMapComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private tripService = inject(TripService);
  private authService = inject(AuthService);
  private loadingService = inject(LoadingService);

  // 2. הזרקת השירות שמעיר את המסך
  private cdr = inject(ChangeDetectorRef);

  tripId!: number;
  tripData: any;


  private loadGoogleMapsScript() {
    // בדיקה אם הסקריפט כבר נטען קודם כדי לא להכביד על הרשת
    if (document.getElementById('google-maps-script')) {
      this.initMap();
      return;
    }

    const script = document.createElement('script');
    script.id = 'google-maps-script';
    // שליפת המפתח ממשתנה הסביבה בצורה מאובטחת
    script.src = `https://maps.googleapis.com/maps/api/js?key=${environment.googleMapsApiKey}`;
    script.async = true;
    script.defer = true;

    // ברגע שהסקריפט מסיים לרדת מהשרתים של גוגל, נצייר את המפה
    script.onload = () => {
      this.initMap();
    };

    // הזרקת הסקריפט לדפדפן מאחורי הקלעים
    document.head.appendChild(script);
  }

  initMap() {
    // מוודאים שיש נתונים
    if (!this.tripData || !this.tripData.places || this.tripData.places.length === 0) {
      console.warn('No places to show');
      this.cdr.detectChanges();
      return;
    }

    const mapElement = document.getElementById('map');
    if (!mapElement) {
      console.error('Map div not found');
      return;
    }

    try {
      const places = this.tripData.places;
      const mapOptions = {
        center: { lat: Number(places[0].latitude), lng: Number(places[0].longitude) },
        zoom: 13,
        mapId: 'DEMO_MAP_ID'
      };

      const map = new google.maps.Map(mapElement, mapOptions);

      places.forEach((place: any, i: number) => {
        new google.maps.Marker({
          position: { lat: Number(place.latitude), lng: Number(place.longitude) },
          map: map,
          label: (i + 1).toString()
        });
      });

      console.log('Map drawing finished');
      this.cdr.detectChanges();

    } 
    catch (e) {
      console.error('Map drawing error', e);
    }
  }

  ngOnInit() {
    console.log('ngOnInit', this.tripData);

    this.tripId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.tripId) {
      this.loadTripDetails();
    }
  }

  loadTripDetails() {

    this.loadingService.show('ה-AI שלנו בונה עבורך את המסלול המושלם... 🌍');

    console.log('loadTripDetails', this.tripData);

    //של המשתמש המחובר  ID שליפת ה 
    const userId = this.authService.getUserId();

    this.tripService.getTripById(this.tripId, userId).subscribe({
      next: (data) => {
        this.tripData = data;
        console.log('Trip loaded:', data);
        this.cdr.detectChanges();

        setTimeout(() => {
          // this.initMap();
          this.loadGoogleMapsScript();
          this.cdr.detectChanges();
          this.loadingService.hide();
        }, 100);
      },
      error: (err) => {
        console.error('Error loading trip:', err);
        this.cdr.detectChanges();
        this.loadingService.hide();
      }
    });
  }

  handlePlaceClick(placeName: string) {
    alert(`לחצת על המקום: ${placeName}`);
  }

}