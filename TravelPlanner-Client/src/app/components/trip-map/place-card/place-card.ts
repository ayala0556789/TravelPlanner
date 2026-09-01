import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-place-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './place-card.html',
  styleUrl: './place-card.scss'
})
export class PlaceCardComponent {
  // קבלת המידע מהאב
  @Input() place: any;
  @Input() index!: number;

  // שליחת האירוע לאב
  @Output() placeClicked = new EventEmitter<string>();

  onCardClick() {
    // משדר לאב את שם המקום שנלחץ
    this.placeClicked.emit(this.place.placeName);
  }
}