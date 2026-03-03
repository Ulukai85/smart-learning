import { Component, inject, OnInit, signal } from '@angular/core';
import { ReviewService } from '../../services/review-service';
import { StreakDto } from '../../models/xpTransaction.model';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styles: ``,
})
export class Dashboard implements OnInit {
  private reviewService = inject(ReviewService);
  streakData = signal<StreakDto | null>(null)

  ngOnInit(): void {
    this.loadStreakData()
  }

  loadStreakData() {
    this.reviewService.fetchStreakData().subscribe({
      next: data => {
        this.streakData.set(data);
        console.log('data:', data)
      },
      error: err => console.log('Err: ', err)
    })
  }
}
