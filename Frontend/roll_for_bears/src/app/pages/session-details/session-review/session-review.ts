import { Component, Input } from '@angular/core';
import { SessionReviewDto } from '../../../shared/dtos/session-review.dto';

@Component({
  imports: [],
  selector: 'app-session-review',
  styleUrl: './session-review.css',
  templateUrl: './session-review.html',
})
export class SessionReview {
  @Input({required: true}) review!: SessionReviewDto;

  readonly stars = [1, 2, 3, 4, 5];


}
